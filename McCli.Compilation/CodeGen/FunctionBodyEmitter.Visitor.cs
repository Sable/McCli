﻿using McCli.Compilation.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	partial class FunctionBodyEmitter : Visitor
	{
		#region Fields
		private Label? continueTargetLabel;
		private Label? breakTargetLabel;
		#endregion

		#region Methods
		public override void VisitLiteral(Literal literal)
		{
			using (BeginEmitStore(literal.Target))
			{
				var sourceRepr = MRepr.FromCliType(literal.Value.GetType());
				if (sourceRepr.IsPrimitive && !sourceRepr.IsComplex)
				{
					if (sourceRepr.Type == MPrimitiveClass.Double) ilGenerator.Emit(OpCodes.Ldc_R8, (double)literal.Value);
					else if (sourceRepr.Type == MPrimitiveClass.Single) ilGenerator.Emit(OpCodes.Ldc_R4, (float)literal.Value);
					else if (sourceRepr.Type == MPrimitiveClass.UInt64) ilGenerator.Emit(OpCodes.Ldc_I8, unchecked((long)(ulong)literal.Value));
					else if (sourceRepr.Type == MPrimitiveClass.Int64) ilGenerator.Emit(OpCodes.Ldc_I8, (long)literal.Value);
					else
					{
						// Should be convertible to int
						int value = unchecked((int)Convert.ToInt64(literal.Value));
						ilGenerator.Emit(OpCodes.Ldc_I4, value); // TODO: Use short form opcodes if possible
					}
				}
				else
				{
					throw new NotImplementedException();
				}

				EmitConversion(sourceRepr, literal.Target.StaticRepr);
			}
		}

		public override void VisitCopy(Copy copy)
		{
			Contract.Requires(copy.Value.StaticRepr == copy.Target.StaticRepr);

			using (BeginEmitStore(copy.Target))
			{
				EmitLoad(copy.Value);
				EmitCloneIfBoxed(copy.Value.StaticRepr);

				EmitConversion(copy.Value.StaticRepr, copy.Target.StaticRepr);
			}
		}

		public override void VisitStaticCall(StaticCall staticCall)
		{
			Contract.Assert(staticCall.Targets.Length == 1);

			var argumentTypes = staticCall.Arguments.Select(a => a.StaticRepr);
			var function = functionLookup(staticCall.Name, argumentTypes);

			using (BeginEmitStore(staticCall.Targets[0]))
			{
				for (int i = 0; i < staticCall.Arguments.Length; ++i)
				{
					var argument = staticCall.Arguments[i];
					EmitLoad(argument);
					EmitConversion(argument.StaticRepr, function.InputTypes[i]);
				}

				ilGenerator.Emit(OpCodes.Call, function.Method);
				EmitConversion(function.OutputType, staticCall.Targets[0].StaticRepr);
			}
		}

		public override void VisitLoadCall(LoadCall loadCall)
		{
			Contract.Assert(!loadCall.Cell);
			Contract.Assert(loadCall.Targets.Length == 1);

			var target = loadCall.Targets[0];
			var subjectType = loadCall.Subject.StaticRepr;
			Contract.Assert(subjectType.IsArray);

			using (BeginEmitStore(target))
			{
				EmitLoad(loadCall.Subject);

				if (loadCall.Arguments.Length == 0)
				{
					// "foo = array()", same as "foo = array"
					EmitCloneIfBoxed(loadCall.Subject.StaticRepr);
				}
				else
				{
					var method = typeof(Utilities).GetMethods(BindingFlags.Public | BindingFlags.Static)
						.FirstOrDefault(m => m.Name == "Subsref" && m.GetParameters().Length == loadCall.Arguments.Length + 1);
					if (method.IsGenericMethodDefinition)
						method = method.MakeGenericMethod(subjectType.Type.CliType);

					var arrayType = subjectType.WithPrimitiveForm(MPrimitiveForm.Array);
					EmitConversion(subjectType, arrayType);

					foreach (var argument in loadCall.Arguments)
					{
						EmitLoad(argument);
						EmitConversion(argument.StaticRepr, arrayType);
					}

					ilGenerator.Emit(OpCodes.Call, method);
					EmitConversion(MRepr.FromCliType(method.ReturnType), target.StaticRepr);
				}
			}
		}

		public override void VisitStoreIndexed(StoreIndexed storeIndexed)
		{
			Contract.Assert(!storeIndexed.Cell);

			throw new NotImplementedException();
		}

		public override void VisitIf(If @if)
		{
			if (@if.Then.Length == 0 && @if.Else.Length == 0) return;

			EmitLoad(@if.Condition);
			EmitConversion(@if.Condition.StaticRepr, MRepr.Any);
			ilGenerator.Emit(OpCodes.Call, typeof(Utilities).GetMethod("IsTrue"));

			var endLabel = ilGenerator.DefineLabel();
			if (@if.Then.Length == 0)
			{
				ilGenerator.Emit(OpCodes.Brtrue, endLabel);
				EmitStatements(@if.Else);
			}
			else if (@if.Else.Length == 0)
			{
				ilGenerator.Emit(OpCodes.Brfalse, endLabel);
				EmitStatements(@if.Then);
			}
			else
			{
				var elseLabel = ilGenerator.DefineLabel();
				ilGenerator.Emit(OpCodes.Brfalse, elseLabel);
				EmitStatements(@if.Then);
				ilGenerator.Emit(OpCodes.Br, endLabel);
				ilGenerator.MarkLabel(elseLabel);
				EmitStatements(@if.Else);
			}

			ilGenerator.MarkLabel(endLabel);
		}

		public override void VisitWhile(While @while)
		{
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;

			continueTargetLabel = ilGenerator.DefineLabel();
			breakTargetLabel = ilGenerator.DefineLabel();

			// Condition
			ilGenerator.MarkLabel(continueTargetLabel.Value);
			EmitLoad(@while.Condition);
			EmitConversion(@while.Condition.StaticRepr, MRepr.Any);
			ilGenerator.Emit(OpCodes.Call, typeof(Utilities).GetMethod("IsTrue"));
			ilGenerator.Emit(OpCodes.Brfalse, breakTargetLabel.Value);

			// Body
			EmitStatements(@while.Body);
			ilGenerator.Emit(OpCodes.Br, continueTargetLabel.Value);

			// End
			ilGenerator.MarkLabel(breakTargetLabel.Value);

			continueTargetLabel = previousContinueTargetLabel;
			breakTargetLabel = previousBreakTargetLabel;
		}

		public override void VisitJump(Jump jump)
		{
			if (jump.Kind == JumpKind.Continue)
			{
				Contract.Assert(continueTargetLabel.HasValue);
				ilGenerator.Emit(OpCodes.Br, continueTargetLabel.Value);
			}
			else if (jump.Kind == JumpKind.Break)
			{
				Contract.Assert(breakTargetLabel.HasValue);
				ilGenerator.Emit(OpCodes.Br, breakTargetLabel.Value);
			}
			else
			{
				// TODO: Support return
				throw new NotImplementedException();
			}
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException();
		}

		private void EmitCloneIfBoxed(MRepr type)
		{
			var sourceType = type.CliType;
			if (typeof(MValue).IsAssignableFrom(sourceType))
			{
				var deepCloneMethod = sourceType.GetMethod("DeepClone");
				ilGenerator.Emit(OpCodes.Callvirt, deepCloneMethod);
				if (!sourceType.IsAssignableFrom(deepCloneMethod.ReturnType))
					ilGenerator.Emit(OpCodes.Castclass, sourceType);
			}
		}
		#endregion
	}
}
