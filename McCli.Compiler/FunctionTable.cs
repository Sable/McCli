using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler
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
		private readonly Dictionary<GroupKey, FunctionMethod> functions = new Dictionary<GroupKey, FunctionMethod>();
		#endregion

		#region Methods
		public void AddMethodsFromAssembly(Assembly assembly)
		{
			Contract.Requires(assembly != null);

			foreach (var type in assembly.ExportedTypes)
				if (type.GetCustomAttribute<MatlabLibraryAttribute>() != null)
					AddMethodsFromType(type);
		}

		public void AddMethodsFromType(Type type)
		{
			Contract.Requires(type != null);

			foreach (var method in type.GetTypeInfo().DeclaredMethods)
				if (method.IsPublic && method.IsStatic && !method.IsGenericMethodDefinition)
					AddMethod(method);
		}

		public void AddMethod(MethodInfo method)
		{
			Contract.Requires(method != null);

			// TODO: Support variadic arguments
			// TODO: Support multiple return parameters
			// TODO: Support variadic return parameters
			// TODO: Support overloading
			var key = new GroupKey(method.Name, method.GetParameters().Length);
			var functionInfo = new FunctionMethod(method);
			functions.Add(key, functionInfo);
		}

		public FunctionMethod Lookup(string name, ImmutableArray<MRepr> argumentTypes)
		{
			Contract.Requires(name != null);

			var key = new GroupKey(name, argumentTypes.Length);
			FunctionMethod function;
			if (!functions.TryGetValue(key, out function))
			{
				string message = string.Format(CultureInfo.InvariantCulture,
					"No function '{0}' taking {1} arguments.", name, argumentTypes.Length);
				throw new KeyNotFoundException(message);
			}

			return function;
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
