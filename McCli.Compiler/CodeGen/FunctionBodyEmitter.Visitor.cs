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
	partial class FunctionBodyEmitter : Visitor
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
			Contract.Requires(node.Value.StaticRepr == node.Target.StaticRepr);

			using (BeginEmitStore(node.Target))
			{
				EmitLoad(node.Value);
				EmitCloneIfNeeded(node.Value.StaticRepr);

				EmitConversion(node.Value.StaticRepr, node.Target.StaticRepr);
			}
		}

		public override void VisitStaticCall(StaticCall node)
		{
			Contract.Assert(node.Targets.Length == 1);

			var argumentTypes = node.Arguments.Select(a => a.StaticRepr);
			var function = functionLookup(node.Name, argumentTypes);

			using (BeginEmitStore(node.Targets[0]))
			{
				for (int i = 0; i < node.Arguments.Length; ++i)
				{
					var argument = node.Arguments[i];
					EmitLoad(argument);
					EmitConversion(argument.StaticRepr, function.InputTypes[i]);
				}

				cil.Call(function.Method);
				EmitConversion(function.OutputType, node.Targets[0].StaticRepr);
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
				EmitLoad(node.Subject);

				if (node.Arguments.Length == 0)
				{
					// "foo = array()", same as "foo = array"
					EmitCloneIfNeeded(node.Subject.StaticRepr);
				}
				else
				{
					var method = typeof(Utilities).GetMethods(BindingFlags.Public | BindingFlags.Static)
						.FirstOrDefault(m => m.Name == "Subsref" && m.GetParameters().Length == node.Arguments.Length + 1);
					if (method.IsGenericMethodDefinition)
						method = method.MakeGenericMethod(subjectType.Type.CliType);

					var arrayType = subjectType.WithStructuralClass(MStructuralClass.Array);
					EmitConversion(subjectType, arrayType);

					foreach (var argument in node.Arguments)
					{
						EmitLoad(argument);
						EmitConversion(argument.StaticRepr, arrayType);
					}

					cil.Call(method);
					EmitConversion(MRepr.FromCliType(method.ReturnType), target.StaticRepr);
				}
			}
		}

		public override void VisitStoreParenthesized(StoreParenthesized node)
		{
			var arrayRepr = node.Array.StaticRepr;
			if (arrayRepr.IsArray && node.Indices.Length == 1)
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

				var method = typeof(Utilities).GetMethod("Subsasgn")
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
			cil.Call(typeof(Utilities).GetMethod("IsTrue"));

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
			cil.Call(typeof(Utilities).GetMethod("IsTrue"));
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
			var collectionRepr = node.LowerBound.StaticRepr;

			// Declare loop labels, preserving parent ones
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;
			continueTargetLabel = cil.CreateLabel("for_continue");
			breakTargetLabel = cil.CreateLabel("for_break");
			var conditionLabel = cil.CreateLabel("for_condition");

			// Allocate the required temporaries
			using (var arrayLocalAllocation = temporaryPool.Alloc(collectionRepr.CliType))
			using (var countLocalAllocation = temporaryPool.Alloc(typeof(int)))
			using (var indexLocalAllocation = temporaryPool.Alloc(typeof(int)))
			{
				// array = colon(lowerBound, increment, upperBound)
				EmitLoad(node.LowerBound);
				EmitCloneIfNeeded(collectionRepr);
				cil.Store(arrayLocalAllocation.Location);

				// count = slicecount(arrayCopy)
				cil.Load(arrayLocalAllocation.Location);
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
				using (BeginEmitStore(node.Iterator))
				{
					cil.Load(arrayLocalAllocation.Location);
					cil.Load(indexLocalAllocation.Location);
					cil.Call(typeof(Utilities).GetMethod("GetForSlice").MakeGenericMethod(collectionRepr.Type.CliType));
				}

				// body
				EmitStatements(node.Body);

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

			throw new NotImplementedException("Repair for loops to make them range-based.");
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
