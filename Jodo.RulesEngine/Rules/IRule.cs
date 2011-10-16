
namespace Jodo.Rules
{
	public interface IRule<in TCandidate> 
	{
        string Name { get; }
		string Description { get; }
		string FailedMessage { get; }
        RuleResult IsSatisfiedBy(TCandidate candidate);
	}

	public interface IRule<in TCandidate, TDecisionData> : IRule<TCandidate>
	{
		TDecisionData DecisionData { get; set; }
	}
}