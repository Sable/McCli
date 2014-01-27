using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Describes a method's parameter or return parameter.
	/// </summary>
	public struct ParameterDescriptor
	{
		#region Fields
		public static readonly ParameterDescriptor VoidReturn = new ParameterDescriptor(typeof(void), ParameterAttributes.Retval, null);

		private readonly Type type;
		private readonly ParameterAttributes attributes;
		private readonly string name;
		#endregion

		#region Constructors
		public ParameterDescriptor(Type type, ParameterAttributes attributes, string name)
		{
			Contract.Requires(type != null);

			this.type = type;
			this.attributes = attributes;
			this.name = name;
		}

		public ParameterDescriptor(Type type, string name) : this(type, ParameterAttributes.None, name) { }
		public ParameterDescriptor(Type type, ParameterAttributes attributes) : this(type, attributes, null) { }
		public ParameterDescriptor(Type type) : this(type, ParameterAttributes.None, null) { }
		#endregion

		#region Properties
		public Type Type
		{
			get { return type ?? typeof(void); }
		}

		public ParameterAttributes Attributes
		{
			get { return attributes; }
		}

		public string Name
		{
			get { return name; }
		}
		#endregion

		#region Methods
		#endregion
	}
}
