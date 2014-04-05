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
		#region Nested Types
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

		[Flags]
		private enum CoercionFlags
		{
			// From least to most "costly"
			None = 0,
			IntegerToFloat = 1 << 0,
			RealToComplex = 1 << 1,
			StructuralClassChange = 1 << 2,
			ToAny = unchecked((int)0xFFFFFFFF)
		}
		#endregion

		#region Fields
		private readonly Dictionary<GroupKey, List<FunctionMethod>> functions
			= new Dictionary<GroupKey, List<FunctionMethod>>();
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
				if (method.IsPublic && method.IsStatic)
					AddMethod(method);
		}

		public void AddMethod(MethodInfo method)
		{
			Contract.Requires(method != null);

			if (method.IsGenericMethodDefinition)
			{
				foreach (var instantiation in InstantiateGenericMethod(method))
					AddMethod(instantiation);
				return;
			}

			// TODO: Support variadic arguments
			// TODO: Support variadic return parameters
			var function = new FunctionMethod(method);
			AddFunction(function);
		}

		public void AddFunction(FunctionMethod function)
		{
			Contract.Requires(function != null);

			var key = new GroupKey(function.Name, function.Signature.Inputs.Count);
			
			List<FunctionMethod> overloads;
			if (!functions.TryGetValue(key, out overloads))
			{
				overloads = new List<FunctionMethod>();
				functions.Add(key, overloads);
			}

			overloads.Add(function);
		}

		public FunctionMethod Lookup(string name, ImmutableArray<MRepr> inputs)
		{
			Contract.Requires(name != null);

			var key = new GroupKey(name, inputs.Length);

			FunctionMethod bestOverload = null;

			// Perform overload resolution based on the required input coercions
			List<FunctionMethod> overloads;
			if (functions.TryGetValue(key, out overloads))
			{
				var leastCoercionFlags = CoercionFlags.None;
				foreach (var overload in overloads)
				{
					var overloadCoercionFlags = GetCoercion(overload, inputs);
					if (!overloadCoercionFlags.HasValue) continue; // Not admissible

					if (bestOverload == null || overloadCoercionFlags.Value < leastCoercionFlags)
					{
						// TODO: assert we don't have multiple overloads with the same minimal admissibility
						bestOverload = overload;
						leastCoercionFlags = overloadCoercionFlags.Value;
					}
				}
			}

			// Make sure we have an admissible overload
			if (bestOverload == null)
			{
				var message = new StringBuilder();
				message.Append("No admissible overload for function ")
					.Append(name)
					.Append(" with input types (");
				for (int i = 0; i < inputs.Length; ++i)
				{
					if (i >= 1) message.Append(", ");
					message.Append(inputs[i].ToString());
				}

				message.Append(')');

				throw new KeyNotFoundException(message.ToString());
			}

			return bestOverload;
		}

		private static CoercionFlags? GetCoercion(FunctionMethod overload, ImmutableArray<MRepr> inputs)
		{
			var coercionFlags = CoercionFlags.None;
			for (int i = 0; i < inputs.Length; ++i)
			{
				MRepr provided = inputs[i];
				MRepr expected = overload.Signature.Inputs[i];

				if (provided == expected) continue;

				if (expected.IsAny)
				{
					coercionFlags |= CoercionFlags.ToAny;
					continue;
				}

				// Check structural class compatibility
				if (provided.StructuralClass != expected.StructuralClass)
				{
					// TODO: Check the subtyping relation of the structural classes
					coercionFlags |= CoercionFlags.StructuralClassChange;
				}

				// Check complex attribute compatibility
				if (provided.IsComplex != expected.IsComplex)
				{
					if (provided.IsComplex) return null; // Complex to real
					coercionFlags |= CoercionFlags.RealToComplex;
				}

				// Check class compatibility.
				if (provided.Class != expected.Class)
				{
					if (provided.Class.IsInteger && expected.Class.IsReal)
					{
						coercionFlags |= CoercionFlags.IntegerToFloat;
					}
					else
					{
						return null;
					}
				}
			}

			return coercionFlags;
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
			if (genericMType == null)
			{
				var message = string.Format(CultureInfo.InvariantCulture,
					"Generic method {0} must have generic parameter attribute {1} for instantiation as MatLab functions.",
					method.Name, typeof(GenericMTypeAttribute).Name);
				throw new ArgumentException(message, "method");
			}

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
