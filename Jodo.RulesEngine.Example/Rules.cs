using System;
using Jodo.Rules;

namespace Jodo
{
    // rule context interface with decision data type declared(the Account)
    // you will use the context interface and the type of object you registered the rule with, in order to retrieve the rules
    public interface IAccountBalanceWithdrawlRules : IRule<decimal, Account> { }
  
    // Rule using runtime decision data (in addition to the Candidate, the decimal, the Account is injected at runtime and used to support decision making).
    public class MinimumAccountBalanceToAllowWithdrawl : Rule<decimal, Account>
    {
        private readonly decimal minimumAccountBalanceToAllowWithdrawl;

        public MinimumAccountBalanceToAllowWithdrawl(decimal minimumAccountBalanceToAllowWithdrawl)
        {
            this.minimumAccountBalanceToAllowWithdrawl = minimumAccountBalanceToAllowWithdrawl;
            Description = String.Format("Checks that a account balance meets the requirements to allow withdraw.");
        }

        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            decimal requiredAccountBalance = minimumAccountBalanceToAllowWithdrawl;
            
            if (DecisionData.GetAccountStatus() == Account.AccountStatus.OnHold)
                requiredAccountBalance += 100;

            return new RuleResult(candidate >= requiredAccountBalance, String.Format("The account balance must be at least ${0} for a {1} account, and it is ${2}", requiredAccountBalance, DecisionData.GetAccountStatus(), candidate));
        }
    }

    public class RuleThatWillAlwaysPass : Rule<decimal>
    {
        public override RuleResult IsSatisfiedBy(decimal candidate)
        {
            return new RuleResult(true);
        }
    }

    // rule context interface without decision data
    public interface IAccountStatusWithdrawRules : IRule<Account> { }

    // this rule does not use runtime decision data, and only operates on the Candidate, the Account.
    public class AccountStatusRequirementToAllowWithDrawl : Rule<Account>
    {
        public AccountStatusRequirementToAllowWithDrawl()
        {
            Description = String.Format("Checks that a account status meets the requirements to allow withdraw.");
        }

        public override RuleResult IsSatisfiedBy(Account candidate)
        {
             if (candidate.GetAccountStatus() == Account.AccountStatus.OnHold)
                 return new RuleResult(false, String.Format("You cannot withdrawl funds when the account status is {0}.", Account.AccountStatus.OnHold));

             return new RuleResult(true, String.Format("Account status of {0} does meet the requirements to allow withdrawing funds.", candidate.GetAccountStatus()));
        }
    }
}