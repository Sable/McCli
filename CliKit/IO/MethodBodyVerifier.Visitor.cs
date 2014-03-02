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

			public override void VisitLoadConstant(LoadConstantOpcode opcode, VisitorParam param)
			{
				switch (opcode.Value)
				{
					case OpcodeValue.Ldnull: param.This.stack.PushNull(); break;
					case OpcodeValue.Ldstr: throw RequiresSymbolicOverload(opcode);
					default: param.This.stack.Push(opcode.DataType); break;
				}
			}

			public override void VisitReturn(VisitorParam param)
			{
				if (param.This.returnType == typeof(void))
				{
					if (param.This.StackSize > 0)
						throw Error("Returning from a void method with a non-empty stack.");
				}
				else
				{
					if (param.This.StackSize > 1)
						throw Error("Return instruction with more than one value on the stack.");

					param.This.stack.PopAssignableTo(Opcode.Ret, param.This.returnType);
				}
			}

			public override void VisitVariableReference(VariableReferenceOpcode opcode, VisitorParam param)
			{
				Type variableType;
				uint index = (uint?)opcode.ConstantIndex ?? param.Operand.UIntValue;
				if (opcode.IsLocal)
				{
					if (index >= (uint)param.This.locals.Count)
						throw Error("Reference to undeclared local {0}.", index);

					variableType = param.This.locals[(int)index].Type;
				}
				else
				{
					if (index >= (uint)param.This.argumentTypes.Length)
						throw Error("Reference to out-of-bound argument {0}.", index);

					variableType = param.This.argumentTypes[(int)index];
				}

				switch (opcode.ReferenceKind)
				{
					case LocationReferenceKind.Load:
						param.This.stack.Push(variableType);
						break;

					case LocationReferenceKind.LoadAddress:
						param.This.stack.PushManagedPointer(variableType.MakeByRefType(), mutable: true);
						break;

					case LocationReferenceKind.Store:
						param.This.stack.PopAssignableTo(opcode, variableType);
						break;

					default:
						throw new NotImplementedException();
				}
			}

			public override void VisitDup(VisitorParam param)
			{
				var top = param.This.stack.Pop(Opcode.Dup);
				param.This.stack.Push(top);
				param.This.stack.Push(top);
			}

			public override void VisitPop(VisitorParam param)
			{
				param.This.stack.Pop(Opcode.Pop);
			}

			public override void VisitFallback(Opcode opcode, VisitorParam param)
			{
				throw new NotImplementedException();
			}
		}
	}
}
