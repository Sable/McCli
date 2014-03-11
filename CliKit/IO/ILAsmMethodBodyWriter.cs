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
	/// Writes a method body as a string of ILAsm code.
	/// </summary>
	public sealed class ILAsmMethodBodyWriter : MethodBodyWriter
	{
		private struct LocalInfo
		{
			public readonly string Name;
			public readonly Type Type;
			public readonly bool Pinned;

			public LocalInfo(string name, Type type, bool pinned)
			{
				this.Name = name;
				this.Type = type;
				this.Pinned = pinned;
			}
		}

		#region Fields
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly string[] argumentNames;
		private readonly List<LocalInfo> locals = new List<LocalInfo>();
		private readonly List<string> labelNames = new List<string>();
		private readonly string generatedLocalNamePrefix;
		private readonly string generatedLabelNamePrefix;
		private int generatedLocalNameCount;
		#endregion

		#region Constructors
		public ILAsmMethodBodyWriter(string[] argumentNames, string generatedLocalNamePrefix)
		{
			Contract.Requires(argumentNames != null);
			Contract.Requires(generatedLocalNamePrefix != null);

			this.argumentNames = (string[])argumentNames.Clone();
			this.generatedLocalNamePrefix = generatedLocalNamePrefix;
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
		public override Label CreateLabel(string name)
		{
			if (name == null) name = generatedLabelNamePrefix + labelNames.Count;
			int labelIndex = labelNames.Count;
			labelNames.Add(name);
			return MakeLabel(labelIndex);
		}

		public string GetString()
		{
			return stringBuilder.ToString();
		}

		public override void MarkLabel(Label label)
		{
			stringBuilder.AppendLine(GetLabelName(label) + ":");
		}

		private string GetLabelName(Label label)
		{
			return labelNames[GetLabelIndex(label)];
		}

		public override int DeclareLocal(Type type, bool pinned, string name)
		{
			if (name == null)
			{
				name = generatedLocalNamePrefix + generatedLocalNameCount;
				generatedLocalNameCount++;
			}

			stringBuilder.AppendLine(".locals " + type.FullName + ' ' + name);
			locals.Add(new LocalInfo(name, type, pinned));
			return locals.Count - 1;
		}

		public override void Instruction(Opcode opcode, NumericalOperand operand)
		{
			stringBuilder.Append(opcode.Name);
			if (opcode.OperandType == OperandType.InlineNone)
			{
				stringBuilder.AppendLine();
				return;
			}

			stringBuilder.Append(' ');
			switch (opcode.OperandType)
			{
				case OperandType.InlineVar:
				case OperandType.ShortInlineVar:
				{
					// TODO: Use index if variable has no name
					int variableIndex = operand.Int;
					bool isLocal = ((VariableReferenceOpcode)opcode).VariableKind == VariableKind.Local;
					stringBuilder.Append(isLocal ? locals[variableIndex].Name : argumentNames[variableIndex]);
					break;
				}

				case OperandType.InlineI:
				case OperandType.ShortInlineI:
				case OperandType.InlineBrTarget:
				case OperandType.ShortInlineBrTarget:
					stringBuilder.Append(operand.Int);
					break;

				case OperandType.InlineI8:
					stringBuilder.Append(operand.Int64);
					break;

				case OperandType.InlineR:
					stringBuilder.Append(operand.Float64);
					break;

				case OperandType.ShortInlineR:
					stringBuilder.Append(operand.Float32);
					break;

				default:
					throw new NotSupportedException();
			}

			stringBuilder.AppendLine();
		}

		public override void LoadString(string str)
		{
			stringBuilder.AppendLine("ldstr \"" + str + '"');
		}

		public override void LoadToken(MemberInfo member)
		{
			stringBuilder.AppendLine("ldtoken " + member.Name);
		}

		public override void Instruction(Opcode opcode, Type type)
		{
			stringBuilder.AppendLine(opcode.Name + ' ' + type.FullName);
		}

		public override void FieldReference(FieldReferenceOpcode opcode, FieldInfo field)
		{
			stringBuilder.AppendLine(opcode.Name + ' ' + field.DeclaringType.FullName + '.' + field.Name);
		}

		public override void Call(CallOpcode opcode, MethodBase method, Type[] parameterTypes, Type returnType)
		{
			stringBuilder.AppendLine(opcode.Name + ' ' + method.DeclaringType.FullName + '.' + method.Name);
		}

		public override void Call(CallOpcode opcode, MethodBase method)
		{
			CallWithReflectedTypes(opcode, method);
		}

		public override void Switch(int[] jumpTable)
		{
			stringBuilder.Append("switch (");
			for (int i = 0; i < jumpTable.Length; ++i)
			{
				if (i > 0) stringBuilder.Append(", ");
				stringBuilder.Append(jumpTable[i].ToString(CultureInfo.InvariantCulture));
			}
			stringBuilder.AppendLine(")");
		}

		public override void Switch(Label[] jumpTable)
		{
			stringBuilder.Append("switch (");
			for (int i = 0; i < jumpTable.Length; ++i)
			{
				if (i > 0) stringBuilder.Append(", ");
				stringBuilder.Append(GetLabelName(jumpTable[i]));
			}
			stringBuilder.AppendLine(")");
		}

		public override void Branch(BranchOpcode opcode, Label target)
		{
			stringBuilder.AppendLine(opcode.Name + ' ' + GetLabelName(target));
		}
		#endregion
	}
}
