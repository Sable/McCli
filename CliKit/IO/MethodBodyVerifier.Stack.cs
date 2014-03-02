using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	partial class MethodBodyVerifier
	{
		private struct StackEntry
		{
			public readonly DataType DataType;
			public readonly Type CtsType; // Only null after ldnull

			public StackEntry(DataType dataType, Type ctsType)
			{
				Contract.Requires(ctsType != null || dataType == DataType.ObjectReference);
				this.DataType = dataType;
				this.CtsType = ctsType;
			}
		}

		private struct Stack
		{
			#region Fields
			private readonly List<StackEntry> entries;
			private readonly int maxSize;
			#endregion

			#region Constructors
			public Stack(int maxSize)
			{
				this.entries = new List<StackEntry>();
				this.maxSize = maxSize;
			}
			#endregion

			#region Properties
			public int Size
			{
				get { return entries.Count; }
			}
			#endregion

			#region Methods
			public StackEntry[] TakeSnapshot()
			{
				return entries.ToArray();
			}

			public void RequireSize(Opcode opcode, int size)
			{
				if (entries.Count < size)
					throw Error("{0} expects {1} stack operands, but the stack has size {2}.", opcode.Name, size, entries.Count);
			}

			public StackEntry Pop(Opcode opcode)
			{
				if (entries.Count == 0)
					throw Error("{0} expects a stack operand, but the stack is empty.", opcode.Name);

				var entry = entries[entries.Count - 1];
				entries.RemoveAt(entries.Count - 1);
				return entry;
			}

			public void PopAssignableTo(Opcode opcode, Type targetType)
			{
				Contract.Requires(opcode != null);
				Contract.Requires(targetType != null);

				if (entries.Count == 0)
					throw Error("{0} expects a stack operand of type {1}, but the stack is empty.", opcode.Name, targetType.FullName);

				var poppedType = entries[entries.Count - 1].CtsType;
				entries.RemoveAt(entries.Count - 1);

				if (poppedType == null)
				{
					if (targetType.IsValueType)
						throw Error("{0} expects a stack operand of type {1}, but the top of the stack is a null value.", opcode.Name, targetType.FullName);

				}
				else if (!targetType.IsAssignableFrom(poppedType))
					throw Error("{0} expects a stack operand of type {1} but the stack top has type {2}.", opcode.Name, targetType.FullName, poppedType.FullName);
			}

			public void Push(StackEntry entry)
			{
				if (entries.Count == maxSize)
					throw Error("Exceeded the maximum stack size of {0}.", maxSize);
				entries.Add(entry);
			}

			public void Push(DataType dataType)
			{
				Contract.Requires(dataType.IsNumeric());
				dataType = dataType.ToStackType();
				var ctsType = dataType.TryGetCtsType();
				if (dataType == DataType.NativeFloat) ctsType = typeof(double);
				Contract.Assert(ctsType != null);
				Push(new StackEntry(dataType, ctsType));
			}

			public void Push(Type ctsType)
			{
				Contract.Requires(ctsType != null);
				Push(new StackEntry(DataTypeEnum.FromCtsType(ctsType), ctsType));
			}

			public void PushManagedPointer(Type type, bool mutable)
			{
				Contract.Requires(type != null && type.IsByRef);
				Push(new StackEntry(mutable ? DataType.MutableManagedPointer : DataType.ReadonlyManagedPointer, type));
			}

			public void PushNull()
			{
				Push(new StackEntry(DataType.ObjectReference, null));
			}
			#endregion
		}
	}
}
