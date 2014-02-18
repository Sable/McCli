using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	/// <summary>
	/// Performs CIL verification of an method bodies.
	/// </summary>
	public abstract class MethodBodyVerifier : MethodBodyWriter
	{
		private struct StackEntry
		{
			public readonly DataType DataType;
			public readonly Type CtsType;

			public StackEntry(DataType dataType, Type ctsType)
			{
				Contract.Requires(ctsType != null);
				this.DataType = dataType;
				this.CtsType = ctsType;
			}
		}

		private struct LocalInfo
		{
			public Type Type;
			public string Name;
		}

		#region Fields
		private readonly MethodBodyWriter sink;
		private readonly List<LocalInfo> locals = new List<LocalInfo>();
		private readonly List<StackEntry> stack = new List<StackEntry>();
		#endregion

		#region Properties
		public int? StackDepth
		{
			get { return stack.Count; }
		}
		#endregion

		#region Methods
		public override int DeclareLocal(Type type, bool pinned, string name)
		{
			int index = locals.Count;
			locals.Add(new LocalInfo { Type = type, Name = name });

			if (sink != null)
			{
				int sinkLocalIndex = sink.DeclareLocal(type, pinned, name);
				Contract.Assert(sinkLocalIndex == index);
			}

			return index;
		}

		public override void Call(CallOpcode opcode, MethodBase method)
		{
			if (method.DeclaringType.IsGenericTypeDefinition)
				ReportError("Call to a member of a generic type definition: {0}.", GetFullName(method));
			if (method.IsGenericMethodDefinition)
				ReportError("Call to generic method definition: {0}.", GetFullName(method));

			if (opcode.Kind == CallKind.Constructor && !(method is ConstructorInfo))
				ReportError("New object by calling a non-constructor: {0}.", GetFullName(method));

			var parameters = method.GetParameters();
			if (stack.Count < parameters.Length)
				ReportError("Not enough stack values for call to {0}.", GetFullName(method));

			for (int i = parameters.Length - 1; i >= 0; --i)
				PopAssignableTo(parameters[i].ParameterType);

			if (sink != null) sink.Call(opcode, method);
		}

		public override void LoadString(string str)
		{
			stack.Add(new StackEntry(DataType.ObjectReference, typeof(string)));
			if (sink != null) sink.LoadString(str);
		}

		public override void FieldReference(FieldReferenceOpcode opcode, FieldInfo field)
		{
			if (opcode.IsStatic && !field.IsStatic)
				ReportError("Static field reference to non-static field {0}.", GetFullName(field));
			if (field.DeclaringType.IsGenericTypeDefinition)
				ReportError("Field reference to a field from a generic type definition: {0}.", GetFullName(field));

			if (opcode.ReferenceKind == LocationReferenceKind.Store)
				PopAssignableTo(field.FieldType);

			if (!field.IsStatic) PopAssignableTo(field.DeclaringType);

			if (opcode.ReferenceKind == LocationReferenceKind.Load) Push(field.FieldType);
			else if (opcode.ReferenceKind == LocationReferenceKind.LoadAddress) Push(field.FieldType.MakeByRefType());

			if (sink != null) sink.FieldReference(opcode, field);
		}

		public override void LoadToken(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field: 
				case MemberTypes.Method:
				case MemberTypes.TypeInfo:
				case MemberTypes.NestedType:
					break;

				default:
					ReportError("Cannot load a token for {0} {1}.", member.MemberType, GetFullName(member));
					break;
			}

			if (sink != null) sink.LoadToken(member);
		}

		public override void Switch(int[] jumpTable)
		{
			ReportError("Unverifiable switch with integral labels.");

			if (sink != null) sink.Switch(jumpTable);
		}

		private void PopAssignableTo(Type targetType)
		{
			Contract.Requires(targetType != null);

			if (stack.Count == 0)
			{
				ReportError("Expected {0} but evaluation stack was empty.", targetType.FullName);
			}
			else
			{
				Type poppedType = stack[stack.Count - 1].CtsType;
				if (targetType.IsAssignableFrom(poppedType))
					ReportError("Type mismatch, expected {0} but got {1}", targetType.FullName, poppedType.FullName);
			}

			stack.RemoveAt(stack.Count - 1);
		}

		private void Push(Type type)
		{
			Contract.Requires(type != null);

			stack.Add(new StackEntry(DataTypeEnum.FromCtsType(type), type));
		}

		private static string GetFullName(MemberInfo info)
		{
			Type type = info as Type;
			if (type != null) return type.FullName;
			if (info.DeclaringType == null) return info.Name;
			return info.DeclaringType.FullName + '.' + info.Name;
		}

		private static void ReportError(string message)
		{
			throw new InvalidOperationException(message);
		}

		private static void ReportError(string format, params object[] args)
		{
			ReportError(string.Format(CultureInfo.InvariantCulture, format, args));
		}
		#endregion
	}
}
