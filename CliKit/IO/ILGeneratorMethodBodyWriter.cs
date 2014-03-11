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
		#region Fields
		private readonly ILGenerator generator;
		private readonly List<EmitLabel> emitLabels = new List<EmitLabel>();
		private bool supportsDebugInfo = true;
		#endregion

		#region Constructors
		public ILGeneratorMethodBodyWriter(ILGenerator generator)
		{
			Contract.Requires(generator != null);
			this.generator = generator;
		}
		#endregion

		#region Methods
		public override int DeclareLocal(Type type, bool pinned, string name)
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
			emitLabels.Add(generator.DefineLabel());
			return MakeLabel(emitLabels.Count - 1);
		}

		public override void MarkLabel(Label label)
		{
			generator.MarkLabel(GetEmitLabel(label));
		}

		public override void LoadString(string str)
		{
			generator.Emit(OpCodes.Ldstr, str);
		}

		public override void Call(CallOpcode opcode, MethodBase method, Type[] parameterTypes, Type returnType)
		{
			// We don't need the parameter/return types
			Call(opcode, method);
		}

		public override void Call(CallOpcode opcode, MethodBase method)
		{
			OpCode emitOpCode;
			opcode.GetReflectionEmitOpCode(out emitOpCode);

			switch (opcode.Kind)
			{
				case CallKind.EarlyBound: generator.Emit(OpCodes.Call, (MethodInfo)method); break;
				case CallKind.Virtual: generator.Emit(OpCodes.Callvirt, (MethodInfo)method); break;
				case CallKind.Jump: generator.Emit(OpCodes.Call, (MethodInfo)method); break;
				case CallKind.Constructor: generator.Emit(OpCodes.Newobj, (ConstructorInfo)method); break;
				default: throw new ArgumentException("opcode");
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
				case OperandType.InlineI8: generator.Emit(emitOpCode, operand.Int64); break;
				case OperandType.InlineR: generator.Emit(emitOpCode, operand.Float64); break;
				case OperandType.InlineVar: generator.Emit(emitOpCode, (ushort)operand.Int); break;
				case OperandType.ShortInlineR: generator.Emit(emitOpCode, operand.Float32); break;
				case OperandType.ShortInlineVar: generator.Emit(emitOpCode, (byte)operand.Int); break;
				case OperandType.ShortInlineI: generator.Emit(emitOpCode, (sbyte)operand.Int); break;
				case OperandType.InlineI: generator.Emit(emitOpCode, operand.Int); break;
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
				emitLabels[i] = GetEmitLabel(jumpTable[i]);

			generator.Emit(OpCodes.Switch, emitLabels);
		}

		public override void Branch(BranchOpcode opcode, Label target)
		{
			OpCode emitOpCode;
			opcode.GetReflectionEmitOpCode(out emitOpCode);

			generator.Emit(emitOpCode, GetEmitLabel(target));
		}

		private EmitLabel GetEmitLabel(Label target)
		{
			return emitLabels[GetLabelIndex(target)];
		}
		#endregion
	}
}
