using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.IR
{
	public sealed class CompilationUnit
	{
		#region Fields
		private readonly IReadOnlyDictionary<string, Function> functions;
		private readonly Function entryPoint;
		#endregion

		#region Constructors
		public CompilationUnit(IEnumerable<Function> functions, Function entryPoint)
		{
			Contract.Requires(functions != null);
			Contract.Requires(entryPoint != null);

			this.functions = functions.ToDictionary(f => f.Name, StringComparer.Ordinal);
			this.entryPoint = entryPoint;

			if (!this.functions.Values.Contains(entryPoint))
				throw new ArgumentException("The entry point is not among the compilation unit's functions.");
		}
		#endregion

		#region Properties
		public IReadOnlyDictionary<string, Function> Functions
		{
			get { return functions; }
		}

		public Function EntryPoint
		{
			get { return entryPoint; }
		}
		#endregion

		#region Methods
		#endregion
	}
}
