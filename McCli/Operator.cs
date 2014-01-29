using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	public sealed class Operator
	{
		#region Fields
		public static readonly Operator Plus = new Operator(null, "+", null, "plus", 0);
		public static readonly Operator Minus = new Operator(null, "-", null, "minus", 0);

		private readonly string prefix;
		private readonly ImmutableArray<string> infixes;
		private readonly string suffix;
		private readonly string symbols;
		private readonly string functionName;
		private readonly int precedence;
		#endregion

		#region Constructors
		private Operator(string prefix, ImmutableArray<string> infixes, string suffix, string functionName, int precedence)
		{
			Contract.Requires(functionName != null);
			Contract.Requires(precedence >= 0);

			this.prefix = prefix ?? string.Empty;
			this.infixes = infixes;
			this.suffix = suffix ?? string.Empty;
			this.functionName = functionName;
			this.precedence = precedence;

			this.symbols = prefix;
			foreach (var infix in infixes) symbols += prefix;
			symbols += suffix;
		}

		private Operator(string prefix, string infix, string suffix, string functionName, int precedence)
			: this(prefix, infix == null ? ImmutableArray<string>.Empty : ImmutableArray.Create(infix), suffix, functionName, precedence) { }
		#endregion

		#region Properties
		public string FunctionName
		{
			get { return functionName; }
		}

		public string Symbols
		{
			get { return symbols; }
		}

		public int Arity
		{
			get { return infixes.Length + 1; }
		}

		public bool HasPrefix
		{
			get { return prefix.Length > 0; }
		}

		public bool HasSuffix
		{
			get { return suffix.Length > 0; }
		}
		#endregion

		#region Methods
		public override string ToString()
		{
			return functionName;
		}
		#endregion
	}
}
