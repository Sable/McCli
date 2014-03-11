using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Base class for CIL opcodes.
	/// Instances are immutable and reference comparable.
	/// </summary>
	public partial class Opcode
	{
		#region Fields
		protected OpCode opcode;
		#endregion

		#region Constructors
		internal Opcode(OpCode opcode)
		{
			this.opcode = opcode;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of this opcode.
		/// </summary>
		public string Name
		{
			get { return opcode.Name; }
		}

		/// <summary>
		/// Gets the code of this opcode.
		/// </summary>
		public OpcodeValue Value
		{
			get { return unchecked((OpcodeValue)opcode.Value); }
		}

		/// <summary>
		/// Gets the number of values popped from the evaluation stack by this opcode,
		/// or <c>null</c> if this opcode pops a variable number of values.
		/// </summary>
		public int? PopCount
		{
			get { return -ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPop); }
		}

		/// <summary>
		/// Gets the number of values pushed on the evaluation stack by this opcode,
		/// or <c>null</c> if this opcode pushes a variable number of values.
		/// </summary>
		public int? PushCount
		{
			get { return ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPush); }
		}

		/// <summary>
		/// Gets the change in evaluation stack depth incurred by the execution of an instruction
		/// with this opcode, or <c>null</c> if this opcode pushes or pops a variable number of values.
		/// </summary>
		public int? StackDepthDelta
		{
			get
			{
				return ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPop)
					+ ReflectionEmitEnums.GetStackDelta(opcode.StackBehaviourPush);
			}
		}

		/// <summary>
		/// Gets the kind of inline operand that accompanies this opcode.
		/// </summary>
		public OperandKind OperandKind
		{
			get { return (OperandKind)opcode.OperandType; }
		}

		/// <summary>
		/// Gets the size in bytes of an instruction with this opcode,
		/// or <c>null</c> if instructions are variable-sized.
		/// </summary>
		public int? InstructionSizeInBytes
		{
			get { return Value.GetByteCount() + OperandKind.GetSizeInBytes(); }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Gets the <see cref="System.Reflection.OpCode"/> for this opcode.
		/// </summary>
		/// <param name="opCode">The BCL opcode.</param>
		public void GetReflectionEmitOpCode(out OpCode opCode)
		{
			opCode = this.opcode;
		}

		/// <summary>
		/// Calls a visitor's visit method for the runtime type of this opcode.
		/// </summary>
		/// <typeparam name="T">The visitor parameter type.</typeparam>
		/// <param name="visitor">The visitor.</param>
		/// <param name="param">The parameter to be provided to the visitor's visit method.</param>
		public virtual void Accept<T>(OpcodeVisitor<T> visitor, T param)
		{
			Contract.Requires(visitor != null);

			switch (Value)
			{
				case OpcodeValue.Break: visitor.VisitBreak(param); break;
				case OpcodeValue.Dup: visitor.VisitDup(param); break;
				case OpcodeValue.Nop: visitor.VisitNop(param); break;
				case OpcodeValue.Pop: visitor.VisitPop(param); break;
				case OpcodeValue.Sizeof: visitor.VisitSizeof(param); break;
				case OpcodeValue.Ret: visitor.VisitReturn(param); break;

				case OpcodeValue.Throw:
				case OpcodeValue.Rethrow:
					visitor.VisitThrowOrRethrow(this, param);
					break;

				default:
					visitor.VisitOther(this, param);
					break;
			}
		}

		public override string ToString()
		{
			return Name;
		}
		#endregion
	}
}
