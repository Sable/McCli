using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McCli
{
	/// <summary>
	/// Indexes <see cref="MType"/> and <see cref="MClass"/> instances for lookup.
	/// </summary>
	/// <remarks>
	/// Not in <see cref="MType"/>/<see cref="MClass"/> to ensure safe static initializer ordering.
	/// </remarks>
	internal static class MTypeLookup
	{
		#region Fields
		private static readonly Dictionary<MClassKinds, MClass> byClassKind;
		private static readonly Dictionary<string, MClass> byClassName;
		private static readonly Dictionary<Type, MType> byCliType;
		#endregion

		#region Constructors
		static MTypeLookup()
		{
			byClassKind = new Dictionary<MClassKinds, MClass>();
			byClassName = new Dictionary<string, MClass>();
			byCliType = new Dictionary<Type, MType>();

			foreach (var field in typeof(MClass).GetFields())
			{
				if (field.IsStatic && typeof(MClass).IsAssignableFrom(field.FieldType))
				{
					var @class = (MClass)field.GetValue(null);

					byClassKind.Add(@class.Kind, @class);
					byClassName.Add(@class.Name, @class);
					byCliType.Add(@class.CliType, @class);

					if ((@class.Kind & MClassKinds.SupportsComplexMask) != 0)
					{
						var complexType = ((MPrimitiveClass)@class).Complex;
						byCliType.Add(complexType.CliType, complexType);
					}
				}
			}
		}
		#endregion

		#region Methods
		public static MClass ByClassKind(MClassKinds kind)
		{
			MClass @class;
			byClassKind.TryGetValue(kind, out @class);
			return @class;
		}

		public static MClass ByClassName(string name)
		{
			Contract.Requires(name != null);

			MClass @class;
			byClassName.TryGetValue(name, out @class);
			return @class;
		}

		public static MType ByCliType(Type type)
		{
			Contract.Requires(type != null);

			MType mtype;
			byCliType.TryGetValue(type, out mtype);
			return mtype;
		}
		#endregion
	}
}
