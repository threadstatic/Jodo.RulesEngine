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
            IRulesInitializer rulesInitializer = Container.GetExportedValue<IRulesInitializer>();

            rulesInitializer
                .RegisterRule<IAccountBalanceWithdrawlRules, decimal>(typeof(Account), () => new MinimumAccountBalanceToAllowWithdrawl(100).Or(new RuleThatWillAlwaysPass()))
                .RegisterRule<IAccountBalanceWithdrawlRules, decimal>(typeof(Account), () => new RuleThatWillAlwaysPass())
                .RegisterRule<IAccountStatusWithdrawRules, Account>(typeof(Account), () => new AccountStatusRequirementToAllowWithDrawl())
                ;
        }

        private static void Run()
        {
            // Create and Save a Account with an Id of 1
            Container.GetExportedValue<IAccountRepository>().Save(new Account(1, Container.GetExportedValue<IRulesRunner>()));

            // Execute a command
            Container.GetExportedValue<AccountWithdrawlHandler>().Handle(new AccountWithdrawl(1, 25));
        }
    }
}
