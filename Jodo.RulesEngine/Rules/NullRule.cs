
namespace Jodo.Rules
{
    public sealed class NullRule<TCandidate> : Rule<TCandidate>
    {
        public NullRule()
        {
            Description = "This rule always returns 'true', and is returned if no rules have been Registered";
        }

        public override RuleResult IsSatisfiedBy(TCandidate candidate)
        {
            return new RuleResult(true, Description);
        }
    }
}
