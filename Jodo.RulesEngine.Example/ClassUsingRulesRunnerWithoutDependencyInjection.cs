using Jodo.Rules;

namespace Jodo
{
    public class ClassUsingRulesRunnerWithoutDependencyInjection
    {
        public void DoSomething()
        {
            //RulesRunner.TestRules<IAccountBalanceWithdrawlRules, decimal, Account>(GetType(), 10, this);
        }
    }
}
