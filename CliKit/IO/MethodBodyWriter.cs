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
		#region Properties
		/// <summary>
		/// Gets the <see cref="IMetadataTokenProvider"/> to be used
		/// to convert symboling operand values into metadata tokens.
		/// Can return <c>null</c>.
		/// </summary>
		public virtual IMetadataTokenProvider MetadataTokenProvider
		{
			get { return null; }
		}
		#endregion

		#region Methods
		#region Overridables
		public abstract int DeclareLocal(Type type, bool pinned, string name);

		public abstract Label CreateLabel(string name);
		public abstract void MarkLabel(Label label);
		public abstract void Switch(Label[] jumpTable);
		protected abstract void Branch(BranchOpcode opcode, Label target);

		public virtual void LoadString(string str)
		{
			Contract.Requires(str != null);
			Instruction(Opcode.Ldstr, GetMetadataTokenProviderOrThrow().GetStringToken(str));
		}

		public virtual void Call(CallOpcode opcode, MemberInfo member)
		{
			Contract.Requires(opcode.Kind != CallKind.Indirect);
			Contract.Requires(member != null);
			Instruction(opcode, GetMetadataTokenProviderOrThrow().GetMemberToken(member));
		}

		public virtual void FieldReference(FieldReferenceOpcode opcode, FieldInfo field)
		{
			Contract.Requires(opcode != null);
			Contract.Requires(field != null);
			Instruction(opcode, GetMetadataTokenProviderOrThrow().GetMemberToken(field));
		}

		public virtual void LoadToken(MemberInfo member)
		{
			Contract.Requires(member != null);
			Instruction(Opcode.Ldtoken, GetMetadataTokenProviderOrThrow().GetMemberToken(member));
		}

		public virtual void Instruction(Opcode opcode, Type type)
		{
			Contract.Requires(opcode != null);
			Contract.Assert(opcode.OperandType == Emit.OperandType.InlineType || opcode.OperandType == Emit.OperandType.InlineTok);
			Contract.Requires(type != null);
			Instruction(opcode, GetMetadataTokenProviderOrThrow().GetMemberToken(type));
		}
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

		private IMetadataTokenProvider GetMetadataTokenProviderOrThrow()
		{
			var provider = MetadataTokenProvider;
			if (provider == null) throw new NotSupportedException("No metadata token provider available.");
			return provider;
		}
		#endregion
		#endregion
	}
}
