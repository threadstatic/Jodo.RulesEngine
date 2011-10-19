using System;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Jodo.Rules;

namespace Jodo
{
    class Program
    {
        private static CompositionContainer container;

        public static CompositionContainer Container
        {
            get
            {
                return container ?? (container = container = new CompositionContainer
                (
                    new AggregateCatalog
                    (
                        new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                        new AssemblyCatalog(typeof(RulesEngine).Assembly)
                    )
                ));
            }
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
            //RulesEngine.RegisterRulesRunner(new MockRulesRunner());

            rulesInitializer
                .RegisterRule<IAccountBalanceWithdrawlRules, decimal>(typeof(Account), () => new MinimumAccountBalanceToAllowWithdrawl(100).Or(new RuleThatWillAlwaysFail()))
                .RegisterRule<IAccountBalanceWithdrawlRules, decimal>(typeof(Account), () => new RuleThatWillAlwaysPass())
                .RegisterRule<IAccountStatusWithdrawRules, Account>(typeof(Account), () => new AccountStatusRequirementToAllowWithDrawl())
                ;
        }

        private static void Run()
        {
            // Example using RulesRunner with the default registered RulesRunner, or a explicitly regesitered RulesRunner, using the static RegisterRulesRunner method on the RulesEngine. For mocking you would use the RegisterRulesRunner passing in a mock.
            
            // Create and Save a Account with an Id of 1
            new AccountRepository().Save(new Account(1));
            // try to withdrawl
            new AccountWithdrawlHandler(new AccountRepository()).Handle(new AccountWithdrawl(1, 25));


            // Example using RulesRunner provided via DependencyInjection.

            //Create and Save a Account with an Id of 2,
            Container.GetExportedValue<IAccountRepository>().Save(new AccountUsingInjectedRulesRunner(2, Container.GetExportedValue<IRulesRunner>()));
            // try to withdrawl
            Container.GetExportedValue<AccountWithdrawlHandler>().Handle(new AccountWithdrawl(2, 25));
        }

        private class MockRulesRunner : IRulesRunner
        {
            public void TestRules<TRuleContext, TCandidate>(Type typeToGetRulesFor, TCandidate candidate) where TRuleContext : IRule<TCandidate> { }
            public void TestRules<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData) where TRuleContext : IRule<TCandidate, TDecisionData> { }
        }
    }
}
