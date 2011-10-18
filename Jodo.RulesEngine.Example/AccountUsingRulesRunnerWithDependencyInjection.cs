using System;
using Jodo.Rules;

namespace Jodo
{
    public class AccountUsingRulesRunnerWithDependencyInjection : Account
    {
        private readonly IRulesRunner rulesRunner;
       

        public AccountUsingRulesRunnerWithDependencyInjection(int id) : base(id)
        {
        }

        public AccountUsingRulesRunnerWithDependencyInjection(int id, IRulesRunner rulesRunner) :this(id)
        {
            this.rulesRunner = rulesRunner;
        }

        protected override void TestWithDrawlRules()
        {
            rulesRunner.TestRules<IAccountBalanceWithdrawlRules, decimal, Account>(GetType(), Balance, this);
            rulesRunner.TestRules<IAccountStatusWithdrawRules, Account>(GetType(), this);
        }
    }
}