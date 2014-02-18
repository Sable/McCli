using CliKit;
using CliKit.IO;
using McCli.Compilation.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation.CodeGen
{
	partial class FunctionBodyEmitter : Visitor
	{
		#region Fields
		private Label continueTargetLabel;
		private Label breakTargetLabel;
		#endregion

		#region Methods
		public override void VisitLiteral(Literal literal)
		{
			using (BeginEmitStore(literal.Target))
			{
				MRepr sourceRepr;
				if (literal.Value is double)
				{
					cil.LoadFloat64((double)literal.Value);
					sourceRepr = MClass.Double.ScalarRepr;
				}
				else if (literal.Value is char)
				{
					cil.LoadInt32((int)(char)literal.Value);
					sourceRepr = MClass.Char.ScalarRepr;
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
				EmitCloneIfNeeded(copy.Value.StaticRepr);

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

				cil.Call(function.Method);
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
					EmitCloneIfNeeded(loadCall.Subject.StaticRepr);
				}
				else
				{
					var method = typeof(Utilities).GetMethods(BindingFlags.Public | BindingFlags.Static)
						.FirstOrDefault(m => m.Name == "Subsref" && m.GetParameters().Length == loadCall.Arguments.Length + 1);
					if (method.IsGenericMethodDefinition)
						method = method.MakeGenericMethod(subjectType.Type.CliType);

					var arrayType = subjectType.WithStructuralClass(MStructuralClass.Array);
					EmitConversion(subjectType, arrayType);

					foreach (var argument in loadCall.Arguments)
					{
						EmitLoad(argument);
						EmitConversion(argument.StaticRepr, arrayType);
					}

					cil.Call(method);
					EmitConversion(MRepr.FromCliType(method.ReturnType), target.StaticRepr);
				}
			}
		}

		public override void VisitStoreIndexed(StoreIndexed storeIndexed)
		{
			Contract.Assert(!storeIndexed.Cell);

			var arrayRepr = storeIndexed.Array.StaticRepr;
			if (arrayRepr.IsArray && storeIndexed.Indices.Length == 1)
			{
				// TODO: should not clone the array for each subsasgn!
				using (BeginEmitStore(storeIndexed.Array))
				{
					EmitLoad(storeIndexed.Array);
					EmitCloneIfNeeded(arrayRepr);
				}

				EmitLoad(storeIndexed.Array);
				EmitLoad(storeIndexed.Indices[0]);
				EmitConversion(storeIndexed.Indices[0].StaticRepr, MClass.Double.ArrayRepr);
				EmitLoad(storeIndexed.Value);
				EmitConversion(storeIndexed.Value.StaticRepr, arrayRepr);

				var method = typeof(Utilities).GetMethod("Subsasgn")
					.MakeGenericMethod(arrayRepr.Type.CliType);
				cil.Call(method);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public override void VisitIf(If @if)
		{
			if (@if.Then.Length == 0 && @if.Else.Length == 0) return;

			EmitLoad(@if.Condition);
			EmitConversion(@if.Condition.StaticRepr, MRepr.Any);
			cil.Call(typeof(Utilities).GetMethod("IsTrue"));

			var endLabel = cil.CreateLabel("if_end");
			if (@if.Then.Length == 0)
			{
				cil.Branch(true, endLabel);
				EmitStatements(@if.Else);
			}
			else if (@if.Else.Length == 0)
			{
				cil.Branch(false, endLabel);
				EmitStatements(@if.Then);
			}
			else
			{
				var elseLabel = cil.CreateLabel("else");
				cil.Branch(false, elseLabel);
				EmitStatements(@if.Then);
				cil.Branch(endLabel);
				cil.MarkLabel(elseLabel);
				EmitStatements(@if.Else);
			}

			cil.MarkLabel(endLabel);
		}

		public override void VisitWhile(While @while)
		{
			// Declare loop labels, preserving parent ones
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;
			continueTargetLabel = cil.CreateLabel("while_continue");
			breakTargetLabel = cil.CreateLabel("while_break");

			// Condition
			cil.MarkLabel(continueTargetLabel);
			EmitLoad(@while.Condition);
			EmitConversion(@while.Condition.StaticRepr, MRepr.Any);
			cil.Call(typeof(Utilities).GetMethod("IsTrue"));
			cil.Branch(false, breakTargetLabel);

			// Body
			EmitStatements(@while.Body);
			cil.Branch(continueTargetLabel);

			// End
			cil.MarkLabel(breakTargetLabel);

			// Restore parent loop labels
			continueTargetLabel = previousContinueTargetLabel;
			breakTargetLabel = previousBreakTargetLabel;
		}

		public override void VisitFor(For @for)
		{
			var collectionRepr = @for.Collection.StaticRepr;
			Contract.Assert(collectionRepr.StructuralClass == MStructuralClass.Array);
			
			// Declare loop labels, preserving parent ones
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;
			continueTargetLabel = cil.CreateLabel("for_continue");
			breakTargetLabel = cil.CreateLabel("for_break");
			var conditionLabel = cil.CreateLabel("for_condition");

			// Allocate the required temporaries
			using (var arrayCopyLocalAllocation = temporaryPool.Alloc(collectionRepr.CliType))
			using (var countLocalAllocation = temporaryPool.Alloc(typeof(int)))
			using (var indexLocalAllocation = temporaryPool.Alloc(typeof(int)))
			{
				// arrayCopy = clone(array)
				EmitLoad(@for.Collection);
				EmitCloneIfNeeded(collectionRepr);
				cil.Store(arrayCopyLocalAllocation.Location);

				// count = slicecount(arrayCopy)
				cil.Load(arrayCopyLocalAllocation.Location);
				cil.Call(typeof(Utilities).GetMethod("GetForSliceCount"));
				cil.Store(countLocalAllocation.Location);

				// index = 0
				cil.LoadInt32(0);
				cil.Store(indexLocalAllocation.Location);

				// condition:
				cil.MarkLabel(conditionLabel);

				// if (index >= count) goto break;
				cil.Load(indexLocalAllocation.Location);
				cil.Load(countLocalAllocation.Location);
				cil.Compare(Comparison.LessThan);
				cil.Branch(false, breakTargetLabel);

				// slice = getslice(arrayCopy, index)
				using (BeginEmitStore(@for.Iterator))
				{
					cil.Load(arrayCopyLocalAllocation.Location);
					cil.Load(indexLocalAllocation.Location);
					cil.Call(typeof(Utilities).GetMethod("GetForSlice").MakeGenericMethod(collectionRepr.Type.CliType));
				}

				// body
				EmitStatements(@for.Body);

				// continue:
				cil.MarkLabel(continueTargetLabel);

				// index++
				cil.Load(indexLocalAllocation.Location);
				cil.LoadInt32(1);
				cil.Instruction(Opcode.Add);
				cil.Store(indexLocalAllocation.Location);

				// goto condition
				cil.Branch(conditionLabel);
			}

			// break:
			cil.MarkLabel(breakTargetLabel);

			// Restore parent loop labels
			continueTargetLabel = previousContinueTargetLabel;
			breakTargetLabel = previousBreakTargetLabel;
		}

		public override void VisitJump(Jump jump)
		{
			switch(jump.Kind)
			{
				case JumpKind.Continue: cil.Branch(continueTargetLabel); break;
				case JumpKind.Break: cil.Branch(breakTargetLabel); break;
				case JumpKind.Return: cil.Branch(returnTargetLabel); break;
				default: throw new NotImplementedException();
			}
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException(string.Format("Visiting {0} IR nodes.", node.GetType().Name));
		}

		private void EmitCloneIfNeeded(MRepr type)
		{
			var sourceType = type.CliType;
			if (typeof(MValue).IsAssignableFrom(sourceType))
			{
				var deepCloneMethod = sourceType.GetMethod("DeepClone");
				cil.Call(deepCloneMethod);
				if (!sourceType.IsAssignableFrom(deepCloneMethod.ReturnType))
					cil.Instruction(Opcode.Castclass, sourceType);
			}
		}
		#endregion
	}
}
