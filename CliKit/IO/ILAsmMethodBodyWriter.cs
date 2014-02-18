using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

		#region Fields
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly List<string> localNames = new List<string>();
		#endregion

		#region Constructors
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
			stringBuilder.AppendLine(".locals " + type.FullName + ' ' + name);
			localNames.Add(name);
			return localNames.Count - 1;
		}

		public override void Instruction(Opcode opcode, NumericalOperand operand)
		{
			var variableReferenceOpcode = opcode as VariableReferenceOpcode;
			if (variableReferenceOpcode != null
				&& variableReferenceOpcode.VariableKind == VariableKind.Local)
			{
				int localIndex = variableReferenceOpcode.ConstantIndex ?? operand.IntValue;
				stringBuilder.AppendLine(opcode.Name + ' ' + localNames[localIndex]);
			}
			else
			{
				stringBuilder.AppendLine(opcode.Name);
			}
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
