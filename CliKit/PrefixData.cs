using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliKit
{
	/// <summary>
	/// Aggregates the information provided by all possible opcode prefixes.
	/// </summary>
	public struct PrefixData
	{
		#region Fields
		public static readonly PrefixData Empty = default(PrefixData);

		private readonly byte prefixes;
		private readonly byte alignment;
		private readonly byte faultChecks;
		private readonly MetadataToken constraintTypeToken;
		#endregion

		#region Constructors
		private PrefixData(byte mask, byte alignment, byte faultChecks, MetadataToken constraintTypeToken)
		{
			this.prefixes = mask;
			this.alignment = alignment;
			this.faultChecks = faultChecks;
			this.constraintTypeToken = constraintTypeToken;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating if no prefix is being applied.
		/// </summary>
		public bool IsEmpty
		{
			get { return prefixes == 0; }
		}

		/// <summary>
		/// Gets the mask of prefixes being applied.
		/// </summary>
		public PrefixMask Prefixes
		{
			get { return (PrefixMask)prefixes; }
		}

		/// <summary>
		/// Gets the alignment specified by the unaligned prefix.
		/// </summary>
		public byte Alignment
		{
			get
			{
				Contract.Requires(Has(PrefixMask.Unaligned));
				return alignment;
			}
		}

		/// <summary>
		/// Gets the metadata token for the constraint type specifid by the constrained prefix.
		/// </summary>
		public MetadataToken ConstraintTypeToken
		{
			get
			{
				Contract.Requires(Has(PrefixMask.Constrained));
				return constraintTypeToken;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Tests if this object specifies the given prefix(es).
		/// </summary>
		/// <param name="prefixes">The mask of prefixes to be tested.</param>
		/// <returns>A value indicating if this object specifies all those prefixes.</returns>
		[Pure]
		public bool Has(PrefixMask prefixes)
		{
			return (Prefixes & prefixes) == prefixes;
		}

		/// <summary>
		/// Gets a copy of this <see cref="PrefixData"/>, modified to includes the tail prefix.
		/// </summary>
		/// <returns>The updated prefix data.</returns>
		public PrefixData WithTail()
		{
			return With(Prefixes | PrefixMask.Tail);
		}

		public PrefixData WithVolatile()
		{
			return With(Prefixes | PrefixMask.Volatile);
		}

		public PrefixData WithReadonly()
		{
			return With(Prefixes | PrefixMask.Readonly);
		}

		public PrefixData WithUnaligned(byte alignment)
		{
			return new PrefixData((byte)(Prefixes | PrefixMask.Unaligned), alignment, faultChecks, constraintTypeToken);
		}

		public PrefixData WithConstrained(MetadataToken typeToken)
		{
			return new PrefixData((byte)(Prefixes | PrefixMask.Constrained), alignment, faultChecks, typeToken);
		}

		public PrefixData Without(PrefixMask prefixes)
		{
			return With(Prefixes & ~prefixes);
		}

		public override string ToString()
		{
			return Prefixes.ToString();
		}

		private PrefixData With(PrefixMask prefixes)
		{
			return new PrefixData((byte)prefixes, alignment, faultChecks, constraintTypeToken);
		}
		#endregion
	}
}
