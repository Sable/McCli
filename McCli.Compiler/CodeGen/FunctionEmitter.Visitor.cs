using CliKit;
using CliKit.IO;
using McCli.Compiler.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CodeGen
{
	partial class FunctionEmitter : Visitor
	{
		#region Fields
		private Label continueTargetLabel;
		private Label breakTargetLabel;
		#endregion

		#region Methods
		public override void VisitLiteral(Literal node)
		{
			using (BeginEmitStore(node.Target))
			{
				MRepr sourceRepr;
				if (node.Value is double)
				{
					cil.LoadFloat64((double)node.Value);
					sourceRepr = MClass.Double.ScalarRepr;
				}
				else if (node.Value is char)
				{
					cil.LoadInt32((int)(char)node.Value);
					sourceRepr = MClass.Char.ScalarRepr;
				}
				else
				{
					throw new NotImplementedException();
				}

				EmitConversion(sourceRepr, node.Target.StaticRepr);
			}
		}

		public override void VisitCopy(Copy node)
		{
			Contract.Assert(node.Value.StaticRepr.Type == node.Target.StaticRepr.Type);

			using (BeginEmitStore(node.Target))
			{
				EmitLoad(node.Value);
				EmitCloneIfNeeded(node.Value.StaticRepr);

				EmitConversion(node.Value.StaticRepr, node.Target.StaticRepr);
			}
		}

		public override void VisitStaticCall(StaticCall node)
		{
			// Resolve the function
			var argumentTypes = node.Arguments.Select(a => a.StaticRepr);
			var function = functionLookup(node.FunctionName, argumentTypes);
			var signature = function.Signature;
			Contract.Assert(node.Arguments.Length == signature.Inputs.Count);
			Contract.Assert(node.Targets.Length <= signature.Outputs.Count);

			// Prepare the eventual return value store
			if (signature.HasReturnValue && node.Targets.Length == 1)
				BeginEmitStore(node.Targets[0]);
			
			// Push the input arguments
			for (int i = 0; i < node.Arguments.Length; ++i)
			{
				var argument = node.Arguments[i];
				EmitLoad(argument);
				EmitConversion(argument.StaticRepr, signature.Inputs[i]);
			}

			if (signature.OutParameterCount > 0)
			{
				// Push the pointers to the output arguments
				for (int i = 0; i < signature.Outputs.Count; ++i)
				{
					if (i < node.Targets.Length)
					{
						// TODO: Handle byref targets
						cil.LoadAddress(GetLocalLocation(node.Targets[i]));
					}
					else
					{
						throw new NotImplementedException("Ignored outputs of a multi-output function.");
					}
				}
			}

			// Call the function
			cil.Call(function.Method);

			// Handle the return value, if any
			if (signature.HasReturnValue)
			{
				if (node.Targets.Length == 1)
				{
					EmitConversion(signature.Outputs[0], node.Targets[0].StaticRepr);
					EndEmitStore(node.Targets[0]);
				}
				else
				{
					// Return value ignored.
					Contract.Assert(node.Targets.Length == 0);
					cil.Pop();
				}
			}
		}

		public override void VisitLoadParenthesized(LoadParenthesized node)
		{
			Contract.Assert(node.Targets.Length == 1);

			var target = node.Targets[0];
			var subjectType = node.Subject.StaticRepr;
			Contract.Assert(subjectType.IsArray);

			using (BeginEmitStore(target))
			{
				if (node.Arguments.Length == 0)
				{
					// "foo = array()", same as "foo = array"
					EmitLoad(node.Subject);
					EmitCloneIfNeeded(node.Subject.StaticRepr);
				}
				else
				{
					var method = typeof(PseudoBuiltins).GetMethods(BindingFlags.Public | BindingFlags.Static)
						.FirstOrDefault(m => m.Name == "ArrayGet" && m.GetParameters().Length == node.Arguments.Length + 1);
					if (method.IsGenericMethodDefinition)
						method = method.MakeGenericMethod(subjectType.Type.CliType);

					var arrayType = subjectType.WithStructuralClass(MStructuralClass.Array);

					EmitLoad(node.Subject);
					EmitConversion(subjectType, arrayType);

					foreach (var argument in node.Arguments)
					{
						EmitLoad(argument);
						EmitConversion(argument.StaticRepr, MClass.Double.ArrayRepr);
					}

					cil.Call(method);
					EmitConversion(MRepr.FromCliType(method.ReturnType), target.StaticRepr);
				}
			}
		}

		public override void VisitStoreParenthesized(StoreParenthesized node)
		{
			var arrayRepr = node.Array.StaticRepr;
			if (arrayRepr.IsArray)
			{
				// TODO: should not clone the array for each subsasgn!
				using (BeginEmitStore(node.Array))
				{
					EmitLoad(node.Array);
					EmitCloneIfNeeded(arrayRepr);
				}

				EmitLoad(node.Array);
				EmitLoad(node.Indices[0]);
				EmitConversion(node.Indices[0].StaticRepr, MClass.Double.ArrayRepr);
				EmitLoad(node.Value);
				EmitConversion(node.Value.StaticRepr, arrayRepr);

				var method = typeof(PseudoBuiltins).GetMethods()
					.Single(m => m.Name == "ArraySet" && m.GetParameters().Length == node.Indices.Length + 2)
					.MakeGenericMethod(arrayRepr.Type.CliType);
				cil.Call(method);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public override void VisitIf(If node)
		{
			if (node.Then.Length == 0 && node.Else.Length == 0) return;

			EmitLoad(node.Condition);
			EmitConversion(node.Condition.StaticRepr, MRepr.Any);
			cil.Call(typeof(PseudoBuiltins).GetMethod("IsTrue"));

			var endLabel = cil.CreateLabel("if_end");
			if (node.Then.Length == 0)
			{
				cil.Branch(true, endLabel);
				EmitStatements(node.Else);
			}
			else if (node.Else.Length == 0)
			{
				cil.Branch(false, endLabel);
				EmitStatements(node.Then);
			}
			else
			{
				var elseLabel = cil.CreateLabel("else");
				cil.Branch(false, elseLabel);
				EmitStatements(node.Then);
				cil.Branch(endLabel);
				cil.MarkLabel(elseLabel);
				EmitStatements(node.Else);
			}

			cil.MarkLabel(endLabel);
		}

		public override void VisitWhile(While node)
		{
			// Declare loop labels, preserving parent ones
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;
			continueTargetLabel = cil.CreateLabel("while_continue");
			breakTargetLabel = cil.CreateLabel("while_break");

			// Condition
			cil.MarkLabel(continueTargetLabel);
			EmitLoad(node.Condition);
			EmitConversion(node.Condition.StaticRepr, MRepr.Any);
			cil.Call(typeof(PseudoBuiltins).GetMethod("IsTrue"));
			cil.Branch(false, breakTargetLabel);

			// Body
			EmitStatements(node.Body);
			cil.Branch(continueTargetLabel);

			// End
			cil.MarkLabel(breakTargetLabel);

			// Restore parent loop labels
			continueTargetLabel = previousContinueTargetLabel;
			breakTargetLabel = previousBreakTargetLabel;
		}

		public override void VisitFor(RangeFor node)
		{
			// Declare loop labels, preserving parent ones
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;
			continueTargetLabel = cil.CreateLabel("for_continue");
			breakTargetLabel = cil.CreateLabel("for_break");
			var conditionLabel = cil.CreateLabel("for_condition");

			var repr = new MRepr(node.Iterator.StaticRepr.Type, MStructuralClass.Scalar);
			Contract.Assert(repr.Class.IsNumeric && !repr.IsComplex);

			// Allocate the required temporaries
			using (var currentLocal = temporaryPool.Alloc(repr.CliType))
			using (var incrementLocal = temporaryPool.Alloc(repr.CliType))
			using (var upperBoundLocal = temporaryPool.Alloc(repr.CliType))
			{
				// Save the lower bound to a "current" variable
				EmitLoad(node.From);
				EmitConversion(node.From.StaticRepr, repr);
				cil.Store(currentLocal.Location);

				// Save the increment to a local variable
				if (node.Step == null)
				{
					Contract.Assert(repr.Type == MClass.Double);
					cil.LoadFloat64(1);
				}
				else
				{
					EmitLoad(node.Step);
					EmitConversion(node.Step.StaticRepr, repr);
				}
				cil.Store(incrementLocal.Location);

				// Save the upper bound to a local variable
				EmitLoad(node.To);
				EmitConversion(node.To.StaticRepr, repr);
				cil.Store(upperBoundLocal.Location);

				// Test the loop condition
				// condition: if (current > upper) goto break;
				cil.MarkLabel(conditionLabel);
				cil.Load(currentLocal.Location);
				cil.Load(upperBoundLocal.Location);
				cil.Branch(Comparison.GreaterThan, breakTargetLabel);

				// Setup the iterator variable (which can be modified inside the loop)
				cil.Load(currentLocal.Location);
				EmitConversion(repr, node.Iterator.StaticRepr);
				cil.Store(GetLocalLocation(node.Iterator));

				// body
				EmitStatements(node.Body);

				// Loop increment
				// continue: current += increment; goto condition;
				cil.MarkLabel(continueTargetLabel);
				cil.Load(currentLocal.Location);
				cil.Load(incrementLocal.Location);
				cil.Instruction(Opcode.Add);
				cil.Store(currentLocal.Location);
				cil.Branch(conditionLabel);
			}

			// break:
			cil.MarkLabel(breakTargetLabel);

			// Restore parent loop labels
			continueTargetLabel = previousContinueTargetLabel;
			breakTargetLabel = previousBreakTargetLabel;
		}

		public override void VisitJump(Jump node)
		{
			switch(node.Kind)
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
