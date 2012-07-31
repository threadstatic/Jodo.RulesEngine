using System;

namespace Jodo
{
	/// <summary>
	/// Structure that binds two <see cref="Type"/> objects
	/// into one conceptual pair. Provides a GetHashCode implementation
	/// utilizing the combined objects.
	/// </summary>
	public struct RuleKey : IEquatable<RuleKey>
	{
		public readonly Type ruleForType;
		private readonly Type ruleContext;
        private readonly Type ruleCandidate;
		private readonly int hashcode;

        public RuleKey(Type ruleForType, Type ruleContext, Type ruleCandidate)
		{
			this.ruleForType = ruleForType;
			this.ruleContext = ruleContext;
            this.ruleCandidate = ruleCandidate;
            hashcode = (ruleForType.GetHashCode() * 397) ^ ruleContext.GetHashCode() ^ ruleCandidate.GetHashCode();
		}

		public bool Equals(RuleKey other)
		{
            return Equals(other.ruleForType, ruleForType) && Equals(other.ruleContext, ruleContext) && Equals(other.ruleCandidate, ruleCandidate);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (obj.GetType() != typeof(RuleKey))
				return false;

			return Equals((RuleKey)obj);
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