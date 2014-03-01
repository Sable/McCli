using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	partial class MethodBodyVerifier
	{
		private struct VisitorParam
		{
			public readonly MethodBodyVerifier This;
			public readonly NumericalOperand Operand;

			public VisitorParam(MethodBodyVerifier @this, NumericalOperand operand)
			{
				Contract.Requires(@this != null);
				this.This = @this;
				this.Operand = operand;
			}
		}

		private sealed class Visitor : OpcodeVisitor<VisitorParam>
		{
			public static readonly Visitor Instance = new Visitor();

			#region Nop Opcodes
			public override void VisitBreak(VisitorParam param) { }
			public override void VisitNop(VisitorParam param) { }
			#endregion

			#region Opcodes Requiring Symbolic Overloads
			public override void VisitBranch(BranchOpcode opcode, VisitorParam param)
			{
				throw RequiresSymbolicOverload(opcode);
			}

			public override void VisitCall(CallOpcode opcode, VisitorParam param)
			{
				throw RequiresSymbolicOverload(opcode);
			}

			public override void VisitFieldReference(FieldReferenceOpcode opcode, VisitorParam param)
			{
				throw RequiresSymbolicOverload(opcode);
			}

			public override void VisitCast(CastOpcode opcode, VisitorParam param)
			{
				throw RequiresSymbolicOverload(opcode);
			}

			public override void VisitSizeof(VisitorParam param)
			{
				throw RequiresSymbolicOverload(Opcode.Sizeof);
			}
			#endregion

			public override void VisitVariableReference(VariableReferenceOpcode opcode, VisitorParam param)
			{
				Type variableType;
				if (opcode.IsLocal)
				{
					uint index = (uint?)opcode.ConstantIndex ?? param.Operand.UIntValue;
					if (index >= (uint)param.This.locals.Count)
						throw Error("Reference to undeclared local {0}.", index);

					variableType = param.This.locals[(int)index].Type;
				}
				else
				{
					// parameters
					throw new NotImplementedException();
				}

				switch (opcode.ReferenceKind)
				{
					case LocationReferenceKind.Load:
						param.This.Push(variableType);
						break;

					case LocationReferenceKind.LoadAddress:
						param.This.Push(variableType.MakeByRefType());
						break;

					case LocationReferenceKind.Store:
						param.This.PopAssignableTo(opcode, variableType);
						break;

					default:
						throw new NotImplementedException();
				}
			}

			public override void VisitDup(VisitorParam param)
			{
				var top = param.This.PopStack(Opcode.Dup);
				param.This.stack.Add(top);
				param.This.stack.Add(top);
			}

			public override void VisitPop(VisitorParam param)
			{
				param.This.PopStack(Opcode.Pop);
			}

			public override void VisitFallback(Opcode opcode, VisitorParam param)
			{
				throw new NotImplementedException();
			}
		}
	}
}
