using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	/// <summary>
	/// Interface for objects which consume raw CIL instructions.
	/// </summary>
	public abstract class RawInstructionSink : IDisposable
	{
		/// <summary>
		/// Writes a given CIL instruction to this sink.
		/// </summary>
		/// <param name="instruction">The instruction to be written.</param>
		public abstract void Instruction(RawInstruction instruction);

		/// <summary>
		/// Closes this instruction sink and releases all associated resources.
		/// </summary>
		public virtual void Dispose() { }

		/// <summary>
		/// Writes any CIL instruction to this sink, except <c>switch</c>.
		/// </summary>
		/// <param name="opcode">The instruction opcode.</param>
		/// <param name="operand">The instruction inline operand (ignored if not needed).</param>
		public void Instruction(Opcode opcode, NumericalOperand operand)
		{
			Instruction(new RawInstruction(opcode, operand));
		}

		/// <summary>
		/// Writes a <c>switch</c> CIL instruction to this sink.
		/// </summary>
		/// <param name="jumpTable">The switch's jump table.</param>
		public void Switch(int[] jumpTable)
		{
			Instruction(RawInstruction.CreateSwitch(jumpTable));
		}

		public void Instruction(Opcode opcode)
		{
			Instruction(opcode, default(NumericalOperand));
		}

		public void LoadInt32(int value)
		{
			Instruction(Opcode.LoadInt32(value), value);
		}

		public void LoadInt64(long value)
		{
			Instruction(Opcode.Ldc_I8, value);
		}

		#region Floats
		public void LoadFloat32(float value)
		{
			Instruction(Opcode.Ldc_R4, value);
		}

		public void LoadFloat64(double value)
		{
			Instruction(Opcode.Ldc_R8, value);
		}
		
		public void LoadFloat(float value)
		{
			LoadFloat(value, Opcode.Conv_R4);
		}

		public void LoadFloat(double value)
		{
			float floatValue = (float)value;
			if (floatValue == value)
			{
				// We can actually encode it as a float
				LoadFloat(floatValue, Opcode.Conv_R8);
				return;
			}

			Instruction(Opcode.Ldc_R8, value);
		}

		private void LoadFloat(float value, Opcode intToFloatConversionOpcode)
		{
			if (value >= -1 && value <= 8)
			{
				int intValue = (int)value;
				if (intValue == value)
				{
					// We can actually encode it as an int followed by a conversion
					var opcode = Opcode.LoadInt32(intValue);
					Contract.Assert(opcode.ConstantValue == intValue);
					Instruction(opcode);
					Instruction(intToFloatConversionOpcode);
					return;
				}
			}

			Instruction(Opcode.Ldc_R4, value);
		}
		#endregion

		public void LoadNull()
		{
			Instruction(Opcode.Ldnull);
		}

		public void Load(VariableLocation location)
		{
			Instruction(Opcode.Load(location), location.Index);
		}

		public void LoadAddress(VariableLocation location)
		{
			Instruction(Opcode.LoadAddress(location), location.Index);
		}

		public void Store(VariableLocation location)
		{
			Instruction(Opcode.Store(location), location.Index);
		}

		public void LoadLocal(int index)
		{
			Contract.Requires(VariableLocation.IsValidIndex(index));
			Instruction(Opcode.LoadLocal(index), index);
		}

		public void LoadLocalAddress(int index)
		{
			Contract.Requires(VariableLocation.IsValidIndex(index));
			Instruction(Opcode.LoadLocalAddress(index), index);
		}

		public void StoreLocal(int index)
		{
			Contract.Requires(VariableLocation.IsValidIndex(index));
			Instruction(Opcode.StoreLocal(index), index);
		}

		public void LoadArgument(int index)
		{
			Contract.Requires(VariableLocation.IsValidIndex(index));
			Instruction(Opcode.LoadArgument(index), index);
		}

		public void LoadArgumentAddress(int index)
		{
			Contract.Requires(VariableLocation.IsValidIndex(index));
			Instruction(Opcode.LoadArgumentAddress(index), index);
		}

		public void StoreArgument(int index)
		{
			Contract.Requires(VariableLocation.IsValidIndex(index));
			Instruction(Opcode.StoreArgument(index), index);
		}

		public void LoadToken(MetadataToken token)
		{
			Instruction(Opcode.Ldtoken, token);
		}

		public void Compare(Comparison comparison)
		{
			var opcode = Opcode.Compare(comparison);
			if (opcode == null) throw new ArgumentException("comparison");
			Instruction(opcode);
		}

		public void LoadIndirect(DataType type)
		{
			var opcode = Opcode.LoadIndirect(type);
			if (opcode == null) throw new ArgumentException("type");
			Instruction(opcode);
		}

		public void StoreIndirect(DataType type)
		{
			var opcode = Opcode.StoreIndirect(type);
			if (opcode == null) throw new ArgumentException("type");
			Instruction(opcode);
		}

		public void Pop() { Instruction(Opcode.Pop); }
		public void Dup() { Instruction(Opcode.Dup); }
		public void Ret() { Instruction(Opcode.Ret); }
	}
}
