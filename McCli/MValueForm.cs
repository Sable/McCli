using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Represents the way a value is represented at runtime.
	/// </summary>
	public struct MValueForm
	{
		#region Fields
		public static readonly MValue Any = default(MValue);
		
		private readonly MClass @class;
		private readonly MClassAttributes classAttributes;
		#endregion

		#region Constructors
		public MValueForm(MClass @class, bool complex)
		{
			Contract.Requires(@class != null);
			Contract.Requires(!complex || (@class.SupportedAttributes & MClassAttributes.Complex) != 0);

			this.@class = @class;
			this.classAttributes = complex ? MClassAttributes.Complex : MClassAttributes.None;
		}
		#endregion

		#region Properties
		public MClass Class
		{
			get { return @class; }
		}

		public bool IsComplex
		{
			get { return (classAttributes & MClassAttributes.Complex) != 0; }
		}

		public MClassAttributes ClassAttributes
		{
			get { return classAttributes; }
		}
		#endregion

		#region Methods
		#endregion
	}
}
