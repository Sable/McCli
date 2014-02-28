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

		internal OperandType OperandType
		{
			get { return opcode.OperandType; }
		}

		/// <summary>
		/// Gets the size in bytes of any inline operand this opcode may have.
		/// Returns <c>null</c> if the opcode has a variable-sized inline operand.
		/// </summary>
		public int? OperandSizeInBytes
		{
			get
			{
				switch (opcode.OperandType)
				{
					case OperandType.InlineNone:
						return 0;

					case OperandType.ShortInlineBrTarget:
					case OperandType.ShortInlineI:
					case OperandType.ShortInlineVar:
						return 1;

					case OperandType.InlineVar:
						return 2;

					case OperandType.InlineI:
					case OperandType.ShortInlineR:
					case OperandType.InlineTok:
					case OperandType.InlineSig:
					case OperandType.InlineString:
					case OperandType.InlineType:
					case OperandType.InlineField:
					case OperandType.InlineMethod:
					case OperandType.InlineBrTarget:
						return 4;

					case OperandType.InlineI8:
					case OperandType.InlineR:
						return 8;

					case OperandType.InlineSwitch:
						return null; // Variable-length jump table

					default: throw new NotSupportedException();
				}
			}
		}

		/// <summary>
		/// Gets the size in bytes of an instruction with this opcode,
		/// or <c>null</c> if instructions are variable-sized.
		/// </summary>
		public int? InstructionSizeInBytes
		{
			get { return Value.GetByteCount() + OperandSizeInBytes; }
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
				case OpcodeValue.Nop: visitor.VisitNop(param); break;
				case OpcodeValue.Sizeof: visitor.VisitSizeof(param); break;
				case OpcodeValue.Ret: visitor.VisitReturn(param); break;

				case OpcodeValue.Throw:
				case OpcodeValue.Rethrow:
					visitor.VisitThrowOrRethrow(this, param);
					break;

				case OpcodeValue.Pop:
				case OpcodeValue.Dup:
					visitor.VisitPopOrDup(this, param);
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
