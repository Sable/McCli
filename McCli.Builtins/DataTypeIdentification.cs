using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace McCli.Builtins
{
	/// <summary>
	/// Implements MatLab builtins for data type identification.
	/// http://www.mathworks.com/help/matlab/data-type-identification.html
	/// </summary>
	[MatlabLibrary]
	public static class DataTypeIdentification
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
			var classNameString = PseudoBuiltins.CharArrayToString(className);
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
	}
}
