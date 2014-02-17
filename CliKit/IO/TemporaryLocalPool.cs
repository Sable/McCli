using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit.IO
{
	/// <summary>
	/// Provides a pool of local variables meant to be used for a short span and later reused.
	/// </summary>
	public sealed class TemporaryLocalPool
	{
		#region Scope Struct
		public struct AllocationScope : IDisposable
		{
			private readonly TemporaryLocalPool pool;
			private readonly Type type;
			private readonly int index;

			internal AllocationScope(TemporaryLocalPool pool, int index, Type type)
			{
				this.pool = pool;
				this.type = type;
				this.index = index;
			}

			public int Index
			{
				get { return index; }
			}

			public VariableLocation Location
			{
				get { return VariableLocation.Local(index); }
			}

			public Type Type
			{
				get { return type; }
			}

			public void Dispose()
			{
				pool.Release(index, type);
			}
		}
		#endregion

		#region Fields
		private readonly MethodBodyWriter methodBodyWriter;
		private readonly string namePrefix;
		private int createdCount;
		private int allocatedCount;
		private readonly Dictionary<Type, List<int>> localsByType = new Dictionary<Type, List<int>>();
		#endregion

		#region Constructors
		public TemporaryLocalPool(MethodBodyWriter methodBodyWriter, string namePrefix)
		{
			Contract.Requires(methodBodyWriter != null);

			this.methodBodyWriter = methodBodyWriter;
			this.namePrefix = namePrefix;
		}
		#endregion

		#region Properties
		public string NamePrefix
		{
			get { return namePrefix; }
		}

		public int CreatedCount
		{
			get { return createdCount; }
		}

		public int AllocatedCount
		{
			get { return allocatedCount; }
		}
		#endregion

		#region Methods
		public AllocationScope Alloc(Type type)
		{
			Contract.Requires(type != null);

			List<int> localIndices;
			if (!localsByType.TryGetValue(type, out localIndices))
			{
				localIndices = new List<int>();
				localsByType.Add(type, localIndices);
			}

			int localIndex;
			if (localIndices.Count == 0)
			{
				string name = GetNextName();
				createdCount++;
				localIndex = methodBodyWriter.DeclareLocal(type, name);
			}
			else
			{
				localIndex = localIndices[localIndices.Count - 1];
				localIndices.RemoveAt(localIndices.Count - 1);
			}

			allocatedCount++;
			return new AllocationScope(this, localIndex, type);
		}

		private void Release(int index, Type type)
		{
			allocatedCount--;
			if (allocatedCount < 0) throw new InvalidOperationException("Temporary released twice!");

			localsByType[type].Add(index);
		}

		private string GetNextName()
		{
			return namePrefix == null ? null : namePrefix + createdCount;
		}
		#endregion
	}
}
