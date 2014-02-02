using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compilation
{
	public sealed class FunctionTable
	{
		private struct GroupKey : IEquatable<GroupKey>
		{
			public readonly string Name;
			public readonly int InputArity;

			public GroupKey(string name, int arity)
			{
				this.Name = name;
				this.InputArity = arity;
			}

			public bool Equals(GroupKey other)
			{
				return Name == other.Name && InputArity == other.InputArity;
			}

			public override bool Equals(object obj)
			{
				return obj is GroupKey && Equals((GroupKey)obj);
			}

			public override int GetHashCode()
			{
				return Name.GetHashCode() ^ InputArity;
			}
		}

		#region Fields
		private readonly Dictionary<GroupKey, MethodInfo> functions = new Dictionary<GroupKey, MethodInfo>();
		#endregion

		#region Methods
		public void AddStatic(Type type)
		{
			Contract.Requires(type != null);

			foreach (var method in type.GetTypeInfo().DeclaredMethods)
				if (method.IsPublic && method.IsStatic)
					Add(method);
		}

		public void Add(MethodInfo method)
		{
			Contract.Requires(method != null);

			// TODO: Support variadic arguments
			// TODO: Ignore output arguments in arity
			var key = new GroupKey(method.Name, method.GetParameters().Length);
			
			// TODO: Support function groups
			functions.Add(key, method);
		}

		public MethodInfo Lookup(string name, ImmutableArray<MType> argumentTypes)
		{
			Contract.Requires(name != null);

			var key = new GroupKey(name, argumentTypes.Length);
			return functions[key];
		}
		#endregion
	}
}
