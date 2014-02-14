﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit
{
	public sealed class ArithmeticOpcode : Opcode
	{
		#region Fields
		private readonly ArithmeticOperation operation;
		#endregion

		#region Constructors
		internal ArithmeticOpcode(Emit.OpCode opcode, ArithmeticOperation operation)
			: base(opcode)
		{
			this.operation = operation;
		}
		#endregion

		#region Properties
		public ArithmeticOperation Operation
		{
			get { return operation; }
		}

		public bool IsUnary
		{
			get { return operation.IsUnary(); }
		}

		public bool IsBinary
		{
			get { return operation.IsBinary(); }
		}

		public bool IsUnsigned
		{
			get { return operation.IsUnsigned(); }
		}

		public bool IsOverflowChecked
		{
			get { return operation.IsOverflowChecked(); }
		}

		public bool IsValidOnFloats
		{
			get { return operation.IsValidOnFloats(); }
		}
		#endregion
	}
}