using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EmitLabel = System.Reflection.Emit.Label;

namespace CliKit.IO
{
	/// <summary>
	/// Adapts the <see cref="ILGenerator"/> class as a <see cref="MethodBodyWriter"/>.
	/// </summary>
	public sealed class ILGeneratorMethodBodyWriter : MethodBodyWriter
	{
		#region LabelAdapter Class
		private sealed class LabelAdapter : CliKit.IO.Label
		{
			public readonly EmitLabel EmitLabel;
			public readonly string Name;
			public bool Marked;

			public LabelAdapter(EmitLabel label, string name)
			{
				this.EmitLabel = label;
				this.Name = name;
			}

			public override string DebugName
			{
				get { return Name; }
			}

			public override bool IsMarked
			{
				get { return Marked; }
			}
		}
		#endregion

		#region Fields
		private readonly ILGenerator generator;
		private bool supportsDebugInfo = true;
		#endregion

		#region Constructors
		public ILGeneratorMethodBodyWriter(ILGenerator generator)
		{
			Contract.Requires(generator != null);
			this.generator = generator;
		}
		#endregion

		#region Properties
		public override IMetadataTokenProvider MetadataTokenProvider
		{
			get { return null; } // TODO: Implement in terms of DynamicILInfo?
		}
		#endregion

		#region Methods
		public override int CreateLocal(Type type, bool pinned, string name)
		{
			var localBuilder = generator.DeclareLocal(type, pinned);

			if (supportsDebugInfo)
			{
				try { localBuilder.SetLocalSymInfo(name); }
				catch (InvalidOperationException) { supportsDebugInfo = false; }
				catch (NotSupportedException) { supportsDebugInfo = false; }
			}

			return localBuilder.LocalIndex;
		}

		public override Label CreateLabel(string name)
		{
			return new LabelAdapter(generator.DefineLabel(), name);
		}

		public override void MarkLabel(Label label)
		{
			var adapter = (LabelAdapter)label;
			generator.MarkLabel(adapter.EmitLabel);
			adapter.Marked = true;
		}

		public override void LoadString(string str)
		{
			generator.Emit(OpCodes.Ldstr, str);
		}

		public override void Call(CallKind kind, MemberInfo member)
		{
			switch (kind)
			{
				case CallKind.EarlyBound: generator.Emit(OpCodes.Call, (MethodInfo)member); break;
				case CallKind.Virtual: generator.Emit(OpCodes.Callvirt, (MethodInfo)member); break;
				case CallKind.Jump: generator.Emit(OpCodes.Call, (MethodInfo)member); break;
				case CallKind.Constructor: generator.Emit(OpCodes.Newobj, (ConstructorInfo)member); break;
				default: base.Call(kind, member); break; // Let the base class handle failures
			}
		}

		public override void FieldReference(FieldReferenceOpcode opcode, FieldInfo field)
		{
			OpCode emitOpCode;
			opcode.GetReflectionEmitOpCode(out emitOpCode);
			generator.Emit(emitOpCode, field);
		}

		public override void Instruction(Opcode opcode, Type type)
		{
			OpCode emitOpCode;
			opcode.GetReflectionEmitOpCode(out emitOpCode);
			generator.Emit(emitOpCode, type);
		}

		public override void LoadToken(MemberInfo member)
		{
			if (member is MethodInfo) generator.Emit(OpCodes.Ldtoken, (MethodInfo)member);
			else if (member is FieldInfo) generator.Emit(OpCodes.Ldtoken, (FieldInfo)member);
			else if (member is Type) generator.Emit(OpCodes.Ldtoken, (Type)member);
			else throw new ArgumentException("member");
		}

		public override void Instruction(Opcode opcode, NumericalOperand operand)
		{
			OpCode emitOpCode;
			opcode.GetReflectionEmitOpCode(out emitOpCode);

			switch (emitOpCode.OperandType)
			{
				case OperandType.InlineNone: generator.Emit(emitOpCode); break;
				case OperandType.InlineI8: generator.Emit(emitOpCode, operand.Int64Constant); break;
				case OperandType.InlineR: generator.Emit(emitOpCode, operand.Float64Constant); break;
				case OperandType.InlineVar: generator.Emit(emitOpCode, (ushort)operand.IntValue); break;
				case OperandType.ShortInlineR: generator.Emit(emitOpCode, operand.Float32Constant); break;
				case OperandType.ShortInlineVar: generator.Emit(emitOpCode, (byte)operand.IntValue); break;
				case OperandType.ShortInlineI: generator.Emit(emitOpCode, (sbyte)operand.IntValue); break;
				case OperandType.InlineI: generator.Emit(emitOpCode, operand.IntValue); break;
				default: throw new NotSupportedException(); // Phi, tokens, branches and switch
			}
		}

		public override void Switch(int[] jumpTable)
		{
			throw new NotSupportedException("ILGenerator does not support raw switch instructions, use labels.");
		}

		public override void Switch(Label[] jumpTable)
		{
			var emitLabels = new EmitLabel[jumpTable.Length];
			for (int i = 0; i < jumpTable.Length; ++i)
				emitLabels[i] = ((LabelAdapter)jumpTable[i]).EmitLabel;

			generator.Emit(OpCodes.Switch, emitLabels);
		}

		public override void Dispose() { }

		protected override void Branch(BranchOpcode opcode, Label target)
		{
			OpCode emitOpCode;
			opcode.GetReflectionEmitOpCode(out emitOpCode);

			generator.Emit(emitOpCode, ((LabelAdapter)target).EmitLabel);
		}
		#endregion
	}
}
