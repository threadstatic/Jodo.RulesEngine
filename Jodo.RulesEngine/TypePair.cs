using System;

namespace Jodo
{
	/// <summary>
	/// Structure that binds two <see cref="Type"/> objects
	/// into one conceptual pair. Provides a GetHashCode implementation
	/// utilizing the combined objects.
	/// </summary>
    /// <remarks>Implementation taken from Jimmy Bogard's AutoMapper</remarks>
	public struct TypePair : IEquatable<TypePair>
	{
		private readonly Type type1;
		private readonly Type type2;
		private readonly int hashcode;

		public TypePair(Type type1, Type type2)
		{
			this.type1 = type1;
			this.type2 = type2;
            hashcode = (type1.GetHashCode() * 397) ^ type2.GetHashCode();
		}

		public bool Equals(TypePair other)
		{
			return Equals(other.type1, type1) && Equals(other.type2, type2);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (obj.GetType() != typeof(TypePair))
				return false;

			return Equals((TypePair)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
                return hashcode;
			}
		}
	}
}