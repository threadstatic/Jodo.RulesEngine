
namespace Jodo.Rules.Operators
{
    public class NotRule<TCandidate, TDecisionData> : Rule<TCandidate, TDecisionData>
    {
        private readonly IRule<TCandidate, TDecisionData> ruleWrapped;

        public override string Name
        {
            get { return string.Format("{0}", ruleWrapped.Name); }
        }

        public NotRule(IRule<TCandidate, TDecisionData> rule)
        {
            ruleWrapped = rule;
        }

        public override RuleResult IsSatisfiedBy(TCandidate candidate)
        {
            ruleWrapped.DecisionData = DecisionData;
            RuleResult ruleResult = ruleWrapped.IsSatisfiedBy(candidate);

            if (!ruleResult)
                return new RuleResult(true);

            return new RuleResult(false, ruleResult.Messages);
        }
    }

    public class NotRule<TCandidate> : Rule<TCandidate>
	{
		private readonly IRule<TCandidate> ruleWrapped;

        public override string Name
        {
            get { return string.Format("{0}", ruleWrapped.Name); }
        }

        public NotRule(IRule<TCandidate> rule)
		{
			ruleWrapped = rule;
		}

		public override RuleResult IsSatisfiedBy(TCandidate candidate)
		{
			RuleResult ruleResult = ruleWrapped.IsSatisfiedBy(candidate);

			if (!ruleResult)
				return new RuleResult(true);

			return new RuleResult(false, ruleResult.Messages);
		}
	}
}