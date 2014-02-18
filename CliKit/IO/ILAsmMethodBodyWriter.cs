using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
		private sealed class TextLabel : Label
		{
			public string Name;
			public bool Marked;

			public override string DebugName
			{
				get { return Name; }
			}

			public override bool IsMarked
			{
				get { return Marked; }
			}
		}

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
		private readonly string generatedLocalNamePrefix;
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
		public string GetString()
		{
			return stringBuilder.ToString();
		}

		public override Label CreateLabel(string name)
		{
			return new TextLabel { Name = name };
		}

		public override void MarkLabel(Label label)
		{
			((TextLabel)label).Marked = true;
			stringBuilder.AppendLine(label.DebugName + ":");
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
					int variableIndex = operand.IntValue;
					bool isLocal = ((VariableReferenceOpcode)opcode).VariableKind == VariableKind.Local;
					stringBuilder.Append(isLocal ? locals[variableIndex].Name : argumentNames[variableIndex]);
					break;
				}

				case OperandType.InlineI:
				case OperandType.ShortInlineI:
				case OperandType.InlineBrTarget:
				case OperandType.ShortInlineBrTarget:
					stringBuilder.Append(operand.IntValue);
					break;

				case OperandType.InlineI8:
					stringBuilder.Append(operand.Int64Constant);
					break;

				case OperandType.InlineR:
					stringBuilder.Append(operand.Float64Constant);
					break;

				case OperandType.ShortInlineR:
					stringBuilder.Append(operand.Float32Constant);
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

		public override void Call(CallOpcode opcode, MethodBase method)
		{
			stringBuilder.AppendLine(opcode.Name + ' ' + method.DeclaringType.FullName + '.' + method.Name);
		}

		public override void Switch(int[] jumpTable)
		{
			stringBuilder.AppendLine("switch");
		}

		public override void Switch(Label[] jumpTable)
		{
			stringBuilder.AppendLine("switch");
		}

		public override void Branch(BranchOpcode opcode, Label target)
		{
			stringBuilder.AppendLine(opcode.Name + ' ' + target.DebugName);
		}
		#endregion
	}
}
