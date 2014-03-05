using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McCli
{
	/// <summary>
	/// Provides elementary operations on primitive types that can be used through generics.
	/// </summary>
	public static class MPrimitives<T>
	{
		#region Operations Structure
		internal struct Constants
		{
			public T Zero, One, MinValue, MaxValue;
		}

		internal struct Operations
		{
			public Func<T, double> ToDouble;
			public Func<double, T> FromDouble;

			public Func<T, T> Negate;
			public Func<T, T, T> Add;
			public Func<T, T, T> Subtract;
			public Func<T, T, T> Multiply;
			public Func<T, T, T> Divide;
			public Func<T, T, T> Remainder;

			public Func<T, T, bool> Equal;
			public Func<T, T, bool> NotEqual;
			public Func<T, T, bool> LessThan;
			public Func<T, T, bool> GreaterThan;
			public Func<T, T, bool> LessThanOrEqual;
			public Func<T, T, bool> GreaterThanOrEqual;
		}
		#endregion

		#region Fields
		public static readonly MClassKinds Kind;
		public static readonly T Zero, One, MinValue, MaxValue;
		private static readonly Operations operations;
		#endregion

		#region Constructors
		static MPrimitives()
		{
			object boxedConstants, boxedOperations;
			GetTypeInfo(typeof(T), out Kind, out boxedConstants, out boxedOperations);

			var constants = (Constants)boxedConstants;
			Zero = constants.Zero;
			One = constants.One;
			MinValue = constants.MinValue;
			MaxValue = constants.MaxValue;

			operations = (Operations)boxedOperations;
		}
		#endregion

		#region Methods
		public static double ToDouble(T value) { return operations.ToDouble(value); }
		public static T FromDouble(double value) { return operations.FromDouble(value); }

		public static T Negate(T value) { return operations.Negate(value); }
		public static T Add(T lhs, T rhs) { return operations.Add(lhs, rhs); }
		public static T Subtract(T lhs, T rhs) { return operations.Subtract(lhs, rhs); }
		public static T Multiply(T lhs, T rhs) { return operations.Multiply(lhs, rhs); }
		public static T Divide(T lhs, T rhs) { return operations.Divide(lhs, rhs); }
		public static T Remainder(T lhs, T rhs) { return operations.Remainder(lhs, rhs); }

		public static bool Equal(T lhs, T rhs) { return operations.Equal(lhs, rhs); }
		public static bool NotEqual(T lhs, T rhs) { return operations.NotEqual(lhs, rhs); }
		public static bool LessThan(T lhs, T rhs) { return operations.LessThan(lhs, rhs); }
		public static bool LessThanOrEqual(T lhs, T rhs) { return operations.LessThanOrEqual(lhs, rhs); }
		public static bool GreaterThan(T lhs, T rhs) { return operations.GreaterThan(lhs, rhs); }
		public static bool GreaterThanOrEqual(T lhs, T rhs) { return operations.GreaterThanOrEqual(lhs, rhs); }

		internal static void GetTypeInfo(Type t, out MClassKinds kind, out object constants, out object operations)
		{
			var primitiveClass = MClass.FromCliType(t) as MPrimitiveClass;
			kind = primitiveClass.Kind;
			switch (kind)
			{
				case MClassKinds.Double:
					constants = new MPrimitives<double>.Constants
					{
						Zero = 0,
						One = 1,
						MinValue = double.NegativeInfinity,
						MaxValue = double.PositiveInfinity,
					};
					operations = new MPrimitives<double>.Operations
					{
						ToDouble = x => x,
						FromDouble = x => x,

						Negate = x => -x,
						Add = (x, y) => x + y,
						Subtract = (x, y) => x - y,
						Multiply = (x, y) => x * y,
						Divide = (x, y) => x / y,
						Remainder = (x, y) => x % y,

						Equal = (x, y) => x == y,
						NotEqual = (x, y) => x != y,
						LessThan = (x, y) => x < y,
						LessThanOrEqual = (x, y) => x <= y,
						GreaterThan = (x, y) => x > y,
						GreaterThanOrEqual = (x, y) => x >= y,
					};
					return;

				default:
					throw new NotImplementedException();
			}
		}
		#endregion
	}
}
