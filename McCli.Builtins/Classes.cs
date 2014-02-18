using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins which deal with classes and type-testing.
	/// </summary>
	public static class Classes
	{
		public static string @class(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class.Name;
		}

		public static bool isa(MValue value, MFullArray<char> className)
		{
			Contract.Requires(value != null);
			Contract.Requires(className != null);

			var @class = value.Class;
			var classNameString = Utilities.AsString(className);
			if (@class.Name == classNameString) return true;

			// "categories"
			var classKind = @class.Kind;
			if (classNameString == "numeric" && (classKind & MClassKinds.NumericMask) != 0) return true;
			if (classNameString == "float" && (classKind & MClassKinds.FloatMask) != 0) return true;
			if (classNameString == "integer" && (classKind & MClassKinds.IntegerMask) != 0) return true;
			return false;
		}

		public static bool iscell(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class == MClass.Cell;
		}

		public static bool ischar(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class == MClass.Char;
		}

		public static bool islogical(MValue value)
		{
			Contract.Requires(value != null);
			return value.Class == MClass.Logical;
		}

		public static bool isfloat(MValue value)
		{
			Contract.Requires(value != null);
			return (value.Class.Kind & MClassKinds.FloatMask) != 0;
		}

		public static bool isinteger(MValue value)
		{
			Contract.Requires(value != null);
			return (value.Class.Kind & MClassKinds.IntegerMask) != 0;
		}

		public static bool isnumeric(MValue value)
		{
			Contract.Requires(value != null);
			return (value.Class.Kind & MClassKinds.NumericMask) != 0;
		}

		public static bool isreal(MValue value)
		{
			Contract.Requires(value != null);
			return !value.Repr.Type.IsComplex;
		}

		// TODO: When overloading is implemented,
		// add concrete-type versions of the is*** type checking builtins above
	}
}
