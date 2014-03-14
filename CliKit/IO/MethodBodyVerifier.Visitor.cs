using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliKit.Cil;

namespace CliKit.IO
{
	partial class MethodBodyVerifier
	{
		private struct VisitorParam
		{
			public readonly MethodBodyVerifier This;
			public readonly NumericalOperand NumericalOperand;
			public readonly object SymbolicOperand;

			public VisitorParam(MethodBodyVerifier @this, NumericalOperand operand)
			{
				Contract.Requires(@this != null);
				
				this.This = @this;
				this.NumericalOperand = operand;
				this.SymbolicOperand = null;
			}

			public VisitorParam(MethodBodyVerifier @this, object symbolicOperand)
			{
				Contract.Requires(@this != null);
				Contract.Requires(symbolicOperand != null);

				this.This = @this;
				this.NumericalOperand = default(NumericalOperand);
				this.SymbolicOperand = symbolicOperand;
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

			public override void VisitArithmetic(ArithmeticOpcode opcode, VisitorParam param)
			{
				if (opcode.IsUnary)
				{
					var operand = param.This.stack.Pop(opcode);
					var resultType = opcode.Operation.GetResult(operand.DataType);
					if (!resultType.HasValue)
						throw Error("{0} expects an integral stack operand, but the top of the stack has type {1}.", opcode.Name, operand.DataType);

					param.This.stack.Push(resultType.Value);
				}
				else
				{
					param.This.stack.RequireSize(opcode, 2);
					var lhs = param.This.stack.Pop(opcode);
					var rhs = param.This.stack.Pop(opcode);

					bool verifiable;
					var resultType = opcode.Operation.GetResult(lhs.DataType, rhs.DataType, out verifiable);
					if (!resultType.HasValue)
						throw Error("{0} cannot operate on stack operands of type {1} and {2}.", opcode.Name, lhs.DataType, rhs.DataType);

					// TODO: Assert verifiable?
					param.This.stack.Push(resultType.Value);
				}
			}

			public override void VisitComparison(ComparisonOpcode opcode, VisitorParam param)
			{
				param.This.stack.RequireSize(opcode, 2);
				var rhs = param.This.stack.Pop(opcode);
				var lhs = param.This.stack.Pop(opcode);

				// TODO: Implement type verification!

				param.This.stack.Push(DataType.Int32);
			}

			public override void VisitConversion(ConversionOpcode opcode, VisitorParam param)
			{
				var value = param.This.stack.Pop(opcode);

				bool verifiable;
				var conversion = Conversions.Get(value.DataType, opcode.TargetDataType, out verifiable);
				if (!conversion.HasValue)
					throw Error("{0} cannot convert {1} to {2}.", opcode.Name, value.DataType, opcode.TargetDataType);

				// TODO: Assert verifiable?
				param.This.stack.Push(opcode.TargetDataType);
			}

			public override void VisitDup(VisitorParam param)
			{
				var top = param.This.stack.Pop(Opcode.Dup);
				param.This.stack.Push(top);
				param.This.stack.Push(top);
			}

			public override void VisitLoadConstant(LoadConstantOpcode opcode, VisitorParam param)
			{
				switch (opcode.Value)
				{
					case OpcodeValue.Ldnull: param.This.stack.PushNull(); break;
					case OpcodeValue.Ldstr: throw RequiresSymbolicOverload(opcode);
					default: param.This.stack.Push(opcode.DataType); break;
				}
			}

			public override void VisitIndirectReference(IndirectReferenceOpcode opcode, VisitorParam param)
			{
				var dataType = opcode.DataType;

				if (opcode.IsLoad)
				{
					var entry = param.This.stack.Pop(opcode);
					if (!entry.DataType.IsPointer())
						throw Error("{0} expects a pointer but the top of the stack has type {1}.", opcode.Name, entry.DataType);
					
					var ctsType = dataType.HasValue ? dataType.Value.ToStackType().ToCtsType() : (Type)param.SymbolicOperand;
					Contract.Assert(ctsType != null);

					param.This.stack.Push(ctsType);
				}
				else
				{
					param.This.stack.RequireSize(opcode, 2);
					var value = param.This.stack.Pop(opcode);
					var target = param.This.stack.Pop(opcode);

					if (!target.DataType.IsPointer())
						throw Error("{0} expects a pointer but the stack has type {1}.", opcode.Name, target.DataType);

					// TODO: Assignment compatibility tests
				}
			}

			public override void VisitPop(VisitorParam param)
			{
				param.This.stack.Pop(Opcode.Pop);
			}

			public override void VisitVariableReference(VariableReferenceOpcode opcode, VisitorParam param)
			{
				Type variableType;
				int index = opcode.MacroIndex ?? param.NumericalOperand.Int;
				if (opcode.IsLocal)
				{
					if (index >= param.This.locals.Count)
						throw Error("Reference to undeclared local {0}.", index);

					variableType = param.This.locals[index].Type;
				}
				else
				{
					if (index >= param.This.argumentTypes.Length)
						throw Error("Reference to out-of-bound argument {0}.", index);

					variableType = param.This.argumentTypes[index];
				}

				switch (opcode.ReferenceKind)
				{
					case LocationReferenceKind.Load:
						param.This.stack.Push(variableType);
						break;

					case LocationReferenceKind.LoadAddress:
						if (variableType.IsByRef)
						{
							throw Error("Cannot load the address of byref variable {0}.",
								param.This.GetVariableName(opcode.VariableKind, (int)index));
						}

						param.This.stack.PushManagedPointer(variableType.MakeByRefType(), mutable: true);
						break;

					case LocationReferenceKind.Store:
						param.This.stack.PopAssignableTo(opcode, variableType);
						break;

					default:
						throw new NotImplementedException();
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

			public override void VisitFallback(Opcode opcode, VisitorParam param)
			{
				throw new NotImplementedException();
			}
		}
	}
}
