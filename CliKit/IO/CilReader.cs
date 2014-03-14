using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliKit.Cil;

namespace CliKit.IO
{
	/// <summary>
	/// Provides methods to read CIL instruction streams.
	/// </summary>
	public static class CilReader
	{
		/// <summary>
		/// Reads CIL instructions from a given data stream.
		/// </summary>
		/// <param name="stream">The source data stream.</param>
		/// <returns>A lazy sequence of resulting CIL instructions, keyed by their byte offsets.</returns>
		public static IEnumerable<RawInstruction> ReadInstructions(Stream stream)
		{
			Contract.Requires(stream != null && stream.CanRead);
			return Read_Unchecked(stream);
		}

		/// <summary>
		/// Reads a single CIL instruction from a given data stream.
		/// </summary>
		/// <param name="stream">The source data stream.</param>
		/// <param name="instruction">The instruction that was read, undefined if the method returns zero.</param>
		/// <returns>A value indicating if an instruction was read, <c>false</c> if the end of the stream was reached.</returns>
		public static bool ReadInstruction(Stream stream, out RawInstruction instruction)
		{
			Contract.Requires(stream != null && stream.CanRead);

			int firstOpcodeByte = stream.ReadByte();
			if (unchecked((byte)firstOpcodeByte) != firstOpcodeByte)
			{
				instruction = default(RawInstruction);
				return false;
			}

			OpcodeValue opcodeValue;
			if (OpcodeValueEnum.IsFirstOfTwoBytes((byte)firstOpcodeByte))
				opcodeValue = (OpcodeValue)((firstOpcodeByte << 8) | stream.ReadUInt8());
			else
				opcodeValue = (OpcodeValue)firstOpcodeByte;

			var opcode = Opcode.FromValue(opcodeValue);
			if (opcode == Opcode.Switch)
			{
				// Read jump table
				int[] jumpTable = new int[stream.ReadUInt32()];
				for (int i = 0; i < jumpTable.Length; ++i)
					jumpTable[i] = stream.ReadInt32();
				instruction = RawInstruction.CreateSwitch(jumpTable);
			}
			else
			{
				NumericalOperand operand;
				switch ((int)opcode.OperandKind.GetSizeInBytes())
				{
					case 0: operand = default(NumericalOperand); break;
					case 1: operand = stream.ReadInt8(); break;
					case 2: operand = stream.ReadInt16(); break;
					case 4: operand = stream.ReadInt32(); break;
					case 8: operand = stream.ReadInt64(); break;
					default: throw new NotSupportedException("Unexpected opcode operand size");
				}
				instruction = new RawInstruction(opcode, operand);
			}

			return true;
		}

		private static IEnumerable<RawInstruction> Read_Unchecked(Stream stream)
		{
			RawInstruction instruction;
			while (ReadInstruction(stream, out instruction))
				yield return instruction;
		}

		private static byte ReadUInt8(this Stream stream)
		{
			var value = stream.ReadByte();
			if (unchecked((byte)value) != value) throw new EndOfStreamException();
			return unchecked((byte)value);
		}

		private static sbyte ReadInt8(this Stream stream)
		{
			return unchecked((sbyte)stream.ReadUInt8());
		}

		private static ushort ReadUInt16(this Stream stream)
		{
			return unchecked((ushort)(stream.ReadUInt8() | (stream.ReadUInt8() << 8)));
		}

		private static short ReadInt16(this Stream stream)
		{
			return unchecked((short)stream.ReadUInt16());
		}

		private static uint ReadUInt32(this Stream stream)
		{
			return unchecked((uint)((uint)stream.ReadUInt16() | ((uint)stream.ReadUInt16() << 16)));
		}

		private static int ReadInt32(this Stream stream)
		{
			return unchecked((int)stream.ReadUInt32());
		}

		private static ulong ReadUInt64(this Stream stream)
		{
			return unchecked((ulong)((ulong)stream.ReadUInt32() | ((ulong)stream.ReadUInt32() << 32)));
		}

		private static long ReadInt64(this Stream stream)
		{
			return unchecked((long)stream.ReadUInt64());
		}
	}
}
