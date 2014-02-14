﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Represents an opcode's operand in the instruction stream.
	/// </summary>
	public struct Operand
	{
		#region Fields
		public static readonly Operand None = new Operand();

		private readonly Numeric64 numeric;
		private readonly object @object;
		#endregion

		#region Constructors
		private Operand(Numeric64 numeric)
		{
			this.numeric = numeric;
			this.@object = null;
		}

		private Operand(object @object)
		{
			this.numeric = default(Numeric64);
			this.@object = @object;
		}
		#endregion

		#region Properties
		public long Int64Constant
		{
			get { return numeric.Int64; }
		}

		public int Int
		{
			get { return checked((int)numeric.Int64); }
		}

		public int Index
		{
			get { return checked((ushort)numeric.UInt64); }
		}
		#endregion
	}
}
