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
		private readonly Dictionary<GroupKey, FunctionInfo> functions = new Dictionary<GroupKey, FunctionInfo>();
		#endregion

		#region Methods
		public void AddStatic(Type type)
		{
			Contract.Requires(type != null);

			foreach (var method in type.GetTypeInfo().DeclaredMethods)
				if (method.IsPublic && method.IsStatic && !method.IsGenericMethodDefinition)
					Add(method);
		}

		public void Add(MethodInfo method)
		{
			Contract.Requires(method != null);

			// TODO: Support variadic arguments
			// TODO: Support multiple return parameters
			// TODO: Support variadic return parameters
			// TODO: Support overloading
			var key = new GroupKey(method.Name, method.GetParameters().Length);
			var functionInfo = new FunctionInfo(method);
			functions.Add(key, functionInfo);
		}

		public FunctionInfo Lookup(string name, ImmutableArray<MRepr> argumentTypes)
		{
			Contract.Requires(name != null);

			var key = new GroupKey(name, argumentTypes.Length);
			return functions[key];
		}

		/// <summary>
		/// Fixes the type arguments of a generic method implementing a MatLab function.
		/// </summary>
		/// <param name="method">The generic method for a MatLab function.</param>
		/// <returns>A sequence of instantiation with the different MatLab types supported by the method.</returns>
		public static IEnumerable<MethodInfo> InstantiateGenericMethod(MethodInfo method)
		{
			Contract.Requires(method != null && method.IsGenericMethodDefinition);

			var genericArguments = method.GetGenericArguments();
			Contract.Assert(genericArguments.Length == 1);

			var genericMType = genericArguments[0].GetCustomAttribute<GenericMTypeAttribute>();
			Contract.Assert(genericMType != null);

			for (int i = 1; i < 0x40000000; i <<= 1)
			{
				var classKind = (MClassKinds)i;
				if ((classKind & genericMType.ClassKinds) == 0) continue;

				var @class = MClass.FromKind(classKind);
				yield return method.MakeGenericMethod(@class.CliType);

				if (genericMType.AllowComplex && (classKind & MClassKinds.SupportsComplexMask) != 0)
					yield return method.MakeGenericMethod(((MPrimitiveClass)@class).Complex.CliType);
			}
		}
		#endregion
	}
}
