
namespace Jodo.Rules.Operators
{
    public class OrRule<TCandidate, TDecisionData> : Rule<TCandidate, TDecisionData>
    {
        private readonly IRule<TCandidate, TDecisionData> rule1;
        private readonly IRule<TCandidate, TDecisionData> rule2;

        public OrRule(IRule<TCandidate, TDecisionData> rule1, IRule<TCandidate, TDecisionData> rule2)
        {
            this.rule1 = rule1;
            this.rule2 = rule2;
        }

        public override RuleResult IsSatisfiedBy(TCandidate candidate)
        {
            rule1.DecisionData = DecisionData;
            rule2.DecisionData = DecisionData;

            RuleResult rule1Result = rule1.IsSatisfiedBy(candidate);

            if (rule1Result || rule2.IsSatisfiedBy(candidate))
                return new RuleResult(true);

            return new RuleResult(false, rule1Result.Messages);
        }
    }

    public class OrRule<T> : Rule<T>
	{
		private readonly IRule<T> rule1;
		private readonly IRule<T> rule2;

        public OrRule(IRule<T> rule1, IRule<T> rule2)
		{
			this.rule1 = rule1;
			this.rule2 = rule2;
		}

		public override RuleResult IsSatisfiedBy(T candidate)
		{
			RuleResult rule1Result = rule1.IsSatisfiedBy(candidate);

			if (rule1Result || rule2.IsSatisfiedBy(candidate))
				return new RuleResult(true);

			return new RuleResult(false, rule1Result.Messages);
		}
	}
}