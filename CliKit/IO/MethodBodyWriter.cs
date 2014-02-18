using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Emit = System.Reflection.Emit;

namespace CliKit.IO
{
	/// <summary>
	/// Base class for objects that write Common Intermediate Language method bodies.
	/// </summary>
	public abstract class MethodBodyWriter : RawInstructionSink
	{
		#region Methods
		#region Overridables
		public abstract int DeclareLocal(Type type, bool pinned, string name);

		public abstract Label CreateLabel(string name);
		public abstract void MarkLabel(Label label);

		public abstract void Branch(BranchOpcode opcode, Label target);
		public abstract void Call(CallOpcode opcode, MethodBase method);
		public abstract void FieldReference(FieldReferenceOpcode opcode, FieldInfo field);
		public abstract void Instruction(Opcode opcode, Type type);
		public abstract void LoadString(string str);
		public abstract void LoadToken(MemberInfo member);
		public abstract void Switch(Label[] jumpTable);
		#endregion

		#region Helpers
		public int DeclareLocal(Type type, bool pinned)
		{
			return DeclareLocal(type, pinned, name: null);
		}

		public int DeclareLocal(Type type, string name)
		{
			return DeclareLocal(type, false, name: name);
		}

		public int DeclareLocal(Type type)
		{
			return DeclareLocal(type, pinned: false, name: null);
		}

		public Label CreateLabel()
		{
			return CreateLabel(name: null);
		}

		public void Call(CallKind kind, MethodInfo method)
		{
			Contract.Requires(method != null);
			Call(Opcode.GetCall(kind), method);
		}

		public void Call(MethodInfo method)
		{
			Contract.Requires(method != null);
			Call(method.IsStatic ? Opcode.Call : Opcode.Callvirt, method);
		}

		public void FieldReference(LocationReferenceKind referenceKind, FieldInfo field)
		{
			Contract.Requires(field != null);
			FieldReference(Opcode.FieldReference(referenceKind, field.IsStatic), field);
		}

		public void LoadField(FieldInfo field)
		{
			Contract.Requires(field != null);
			FieldReference(field.IsStatic ? Opcode.Ldsfld : Opcode.Ldfld, field);
		}

		public void LoadFieldAddress(FieldInfo field)
		{
			Contract.Requires(field != null);
			FieldReference(field.IsStatic ? Opcode.Ldsflda : Opcode.Ldflda, field);
		}

		public void StoreField(FieldInfo field)
		{
			Contract.Requires(field != null);
			FieldReference(field.IsStatic ? Opcode.Stsfld : Opcode.Stfld, field);
		}

		public void Branch(Comparison comparison, Label target)
		{
			Contract.Requires(target != null);
			Branch(Opcode.Branch(comparison, longForm: true), target);
		}

		public void Branch(bool condition, Label target)
		{
			Contract.Requires(target != null);
			Branch(Opcode.Branch(condition, longForm: true), target);
		}

		public void Branch(Label target)
		{
			Contract.Requires(target != null);
			Branch(Opcode.Br, target);
		}
		#endregion
		#endregion
	}
}
