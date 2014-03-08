using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	partial class Opcode
	{
		private static class Lookup
		{
			internal static readonly Opcode[] oneByte;
			internal static readonly Dictionary<ushort, Opcode> twoByte;

			static Lookup()
			{
				oneByte = new Opcode[256];
				twoByte = new Dictionary<ushort, Opcode>();

				foreach (var field in typeof(Opcode).GetFields())
				{
					if (field.IsStatic && typeof(Opcode).IsAssignableFrom(field.FieldType))
					{
						var opcode = (Opcode)field.GetValue(null);
						if (opcode.Value.GetByteCount() == 1)
							oneByte[(byte)opcode.Value] = opcode;
						else
							twoByte.Add((ushort)opcode.Value, opcode);
					}
				}
			}
		}

		/// <summary>
		/// Obtains the <see cref="Opcode"/> instance for the given value.
		/// </summary>
		/// <param name="value">The value of the opcode.</param>
		/// <returns>The corresponding <see cref="Opcode"/> instance, or <c>null</c> if there is none.</returns>
		public static Opcode FromValue(OpcodeValue value)
		{
			if (value.GetByteCount() == 1)
			{
				return Lookup.oneByte[(int)value];
			}
			else
			{
				Opcode opcode;
				Lookup.twoByte.TryGetValue((ushort)value, out opcode);
				return opcode;
			}
		}
	}
}
