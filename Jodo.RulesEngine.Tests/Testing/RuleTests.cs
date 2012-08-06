using System;
using NUnit.Framework;

namespace Jodo.Testing
{
    public class RuleTests
    {
        private RulesEngine rulesEngine;
        private IRulesInitializer RulesInitializer { get { return rulesEngine; } }

        [SetUp]
        public void Setup()
        {
            rulesEngine = new RulesEngine();
            var rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);
        }

        [TearDown]
        public void TearDown()
        {
            rulesEngine.Dispose();
        }

        [Test]
        public void WhenCallingIsRegisteredForARuleHasBeenRegistered()
        {
            var rule = new RuleThatWillAlwaysPass();
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);

            Func<bool> should_return_true_for_the_registered_rule = Rule<RuleThatWillAlwaysPass>.IsRegisteredFor<Account>;
            Assert.IsTrue(should_return_true_for_the_registered_rule());
        }

        [Test]
        public void WhenCallingIsRegisteredForARuleThatHasNotBeenRegistered()
        {
            Assert.IsFalse(Rule<RuleThatWillAlwaysPass>.IsRegisteredFor<Account>());
        }
    }
}
