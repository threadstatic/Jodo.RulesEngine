using System;
using System.ComponentModel.Composition.Hosting;
using Jodo.Rules;

namespace Jodo
{
    class Program
    {
        private static CompositionContainer container;

        public static CompositionContainer Container
        {
            get { return container ?? (container = container = new CompositionContainer(new AssemblyCatalog(typeof(RulesEngine).Assembly))); }
        }

        static void Main(string[] args)
        {
            RegisterRules();
            Run();
            Console.ReadKey();
        }

        private static void RegisterRules()
        {
            IRulesInitializer rulesInitializer = new RulesEngine();
            RulesEngine.RegisterRulesRunner(new MockRulesRunner());

            rulesInitializer
                .RegisterRule<IAccountBalanceWithdrawlRules, decimal>(typeof(Account), () => new MinimumAccountBalanceToAllowWithdrawl(100).Or(new RuleThatWillAlwaysFail()))
                .RegisterRule<IAccountBalanceWithdrawlRules, decimal>(typeof(Account), () => new RuleThatWillAlwaysPass())
                .RegisterRule<IAccountStatusWithdrawRules, Account>(typeof(Account), () => new AccountStatusRequirementToAllowWithDrawl())
                ;
        }

        private static void Run()
        {
            // Example using RulesRunner without DependencyInjection. The default RulesRunner class will be used, or a explicitly regesitered RulesRunner, using the static RegisterRulesRunner method on the RulesEngine
            // Create and Save a Account with an Id of 1
            new AccountRepository().Save(new Account(1));

            // Example using RulesRunner provided via DependencyInjection.
            //Create and Save a Account with an Id of 2
            new AccountRepository().Save(new AccountUsingRulesRunnerWithDependencyInjection(2, Container.GetExportedValue<IRulesRunner>()));

            // Execute a commands for both accounts
            new AccountWithdrawlHandler(new AccountRepository()).Handle(new AccountWithdrawl(1, 25));
            new AccountWithdrawlHandler(new AccountRepository()).Handle(new AccountWithdrawl(2, 25));
        }

        private class MockRulesRunner : IRulesRunner
        {
            public void TestRules<TRuleContext, TCandidate>(Type typeToGetRulesFor, TCandidate candidate) where TRuleContext : IRule<TCandidate>
            {
                
            }

            public void TestRules<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData) where TRuleContext : IRule<TCandidate, TDecisionData>
            {
                
            }
        }

    }

}
