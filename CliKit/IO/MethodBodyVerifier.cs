using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OperandType = System.Reflection.Emit.OperandType;

namespace CliKit.IO
{
	/// <summary>
	/// Performs CIL verification of an method bodies.
	/// </summary>
	public sealed partial class MethodBodyVerifier : MethodBodyWriter
	{
		#region Structures
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

		private sealed class LabelInfo
		{
			public string Name;
			public StackEntry[] StackState;
			public bool Marked;
		}
		#endregion

		#region Fields
		private readonly MethodBase method;
		private readonly MethodBodyWriter sink;
		private readonly List<LocalInfo> locals = new List<LocalInfo>();
		private readonly List<LabelInfo> labels = new List<LabelInfo>();
		private readonly List<StackEntry> stack = new List<StackEntry>();
		#endregion

		#region Constructors
		public MethodBodyVerifier(MethodBase method, MethodBodyWriter sink)
		{
			Contract.Requires(method != null);
			Contract.Requires(sink != null);

			this.method = method;
			this.sink = sink;
		}
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

		public override Label CreateLabel(string name)
		{
			int labelIndex = labels.Count;
			labels.Add(new LabelInfo { Name = name });

			if (sink != null)
			{
				var sinkLabel = sink.CreateLabel(name);
				Contract.Assert(GetLabelIndex(sinkLabel) == labelIndex);
			}

			return CreateLabel(labelIndex);
		}

		public override void MarkLabel(Label label)
		{
			var labelInfo = GetLabelInfo(label);
			if (labelInfo.Marked)
			{
				string message = string.Format(CultureInfo.InvariantCulture,
					"{0} label marked twice.", labelInfo.Name ?? "Unnamed");
				throw new InvalidOperationException(message);
			}

			labelInfo.Marked = true;

			SetLabelStackState(labelInfo);
		}

		public override void Branch(BranchOpcode opcode, Label target)
		{
			switch (opcode.BranchKind)
			{
				case BranchKind.Unconditional: break;

				// TODO: Check operand types
				case BranchKind.Boolean: PopStack(opcode); break;
				case BranchKind.Comparison:
					RequireStackSize(opcode, 2);
					PopStack(opcode);
					PopStack(opcode);
					break;

				default: throw new NotImplementedException();
			}

			SetLabelStackState(GetLabelInfo(target));

			if (sink != null) sink.Branch(opcode, target);
		}

		public override void Call(CallOpcode opcode, MethodBase method)
		{
			if (method.DeclaringType.IsGenericTypeDefinition)
				throw Error("Call to a member of a generic type definition: {0}.", GetFullName(method));
			if (method.IsGenericMethodDefinition)
				throw Error("Call to generic method definition: {0}.", GetFullName(method));
			if (opcode.Kind == CallKind.Constructor && !(method is ConstructorInfo))
				throw Error("New object by calling a non-constructor: {0}.", GetFullName(method));

			var parameters = method.GetParameters();
			
			int requiredStackSize = parameters.Length;
			if (!method.IsStatic) ++requiredStackSize;

			if (stack.Count < requiredStackSize)
				throw Error("Calling {0} requires {1} stack operands, but the stack has size {2}.", GetFullName(method), requiredStackSize, stack.Count);

			for (int i = parameters.Length - 1; i >= 0; --i)
				PopAssignableTo(opcode, parameters[i].ParameterType);

			if (!method.IsStatic) PopAssignableTo(opcode, method.DeclaringType);

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
				throw Error("Static field reference to non-static field {0}.", GetFullName(field));
			if (field.DeclaringType.IsGenericTypeDefinition)
				throw Error("Field reference to a field from a generic type definition: {0}.", GetFullName(field));

			if (opcode.ReferenceKind == LocationReferenceKind.Store)
				PopAssignableTo(opcode, field.FieldType);

			if (!field.IsStatic) PopAssignableTo(opcode, field.DeclaringType);

			if (opcode.ReferenceKind == LocationReferenceKind.Load)
				Push(field.FieldType);
			else if (opcode.ReferenceKind == LocationReferenceKind.LoadAddress)
			{
				var managedPointerType = field.FieldType.MakeByRefType();
				var dataType = field.IsInitOnly ? DataType.ReadonlyManagedPointer : DataType.MutableManagedPointer;
				stack.Add(new StackEntry(dataType, managedPointerType));
			}

			if (sink != null) sink.FieldReference(opcode, field);
		}

		public override void Instruction(Opcode opcode, NumericalOperand operand)
		{
			var param = new VisitorParam(this, operand);
			opcode.Accept(Visitor.Instance, param);

			if (sink != null) sink.Instruction(opcode, operand);
		}

		public override void Instruction(Opcode opcode, Type type)
		{
			if (opcode.PopCount > 0 || opcode.PushCount > 0)
				throw new NotImplementedException();

			if (sink != null) sink.Instruction(opcode, type);
		}

		public override void LoadToken(MemberInfo member)
		{
			switch (member.MemberType)
			{
				case MemberTypes.Field: Push(typeof(RuntimeFieldHandle)); break;
				case MemberTypes.Method: Push(typeof(RuntimeMethodHandle)); break;

				case MemberTypes.TypeInfo:
				case MemberTypes.NestedType:
					Push(typeof(RuntimeTypeHandle));
					break;

				default:
					throw Error("Cannot load a token for {0} {1}.", member.MemberType, GetFullName(member));
			}

			if (sink != null) sink.LoadToken(member);
		}

		public override void Switch(Label[] jumpTable)
		{
			var top = PopStack(Opcode.Switch);
			if (!top.DataType.IsInteger())
				throw Error("Switch requires an integral stack, but the stack top is of type {0}.", top.DataType);

			foreach (var label in jumpTable)
				SetLabelStackState(GetLabelInfo(label));

			if (sink != null) sink.Switch(jumpTable);
		}

		public override void Switch(int[] jumpTable)
		{
			throw RequiresSymbolicOverload(Opcode.Switch);
		}

		private LabelInfo GetLabelInfo(Label label)
		{
			return labels[GetLabelIndex(label)];
		}

		private void SetLabelStackState(LabelInfo labelInfo)
		{
			if (labelInfo.StackState == null)
			{
				labelInfo.StackState = stack.ToArray();
				return;
			}

			// TODO: merge stack states
			throw new NotImplementedException();
		}

		private void RequireStackSize(Opcode opcode, int size)
		{
			if (stack.Count < size)
				throw Error("{0} expects {1} stack operands, but the stack has size {2}.", opcode.Name, size, stack.Count);
		}

		private StackEntry PopStack(Opcode opcode)
		{
			if (stack.Count == 0)
				throw Error("{0} expects a stack operand, but the stack is empty.", opcode.Name);

			var stackEntry = stack[stack.Count - 1];
			stack.RemoveAt(stack.Count - 1);
			return stackEntry;
		}

		private void PopAssignableTo(Opcode opcode, Type targetType)
		{
			Contract.Requires(targetType != null);

			if (stack.Count == 0)
				throw Error("{0} expects a stack operand of type {1}, but the stack is empty.", opcode.Name, targetType.FullName);

			Type poppedType = stack[stack.Count - 1].CtsType;
			stack.RemoveAt(stack.Count - 1);
			if (targetType.IsAssignableFrom(poppedType))
				throw Error("{0} expects a stack operand of type {1} but the stack top is of type {2}.", opcode.Name, targetType.FullName, poppedType.FullName);
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

		private static Exception Error(string message)
		{
			return new InvalidOperationException(message);
		}

		private static Exception Error(string format, params object[] args)
		{
			return Error(string.Format(CultureInfo.InvariantCulture, format, args));
		}

		private static Exception RequiresSymbolicOverload(Opcode opcode)
		{
			string message = string.Format(
				"Cannot verify opcode {0} with a numeric operand, "
				+ "use an overload which accepts a symbolic operand.", opcode.Name);
			return new NotSupportedException(message);
		}
		#endregion
	}
}
