using System;
using System.Linq;
using FakeItEasy;
using Jodo.Rules;
using Jodo.Tests;
using NUnit.Framework;

namespace Jodo
{
	public class RulesEngineTests
	{
        private RulesEngine rulesEngine;
        private IRulesInitializer RulesInitializer { get { return rulesEngine; } }
        private IRulesProvider RulesProvider { get { return rulesEngine; } }

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
        public void GetRuleFor_ReturnsOneRuleThatIsTheRegisteredRule()
		{
            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));
            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList().Single().Invoke().GetType());
		}

        [Test]
        public void RegisterRule_WhenRuleHasAlreadyBeenRegisted_WillRegisterTheRuleAgain()
        {
            var rule = new MeetsTheMinimumRequiredAccountBalance(100);
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);
            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));

            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[0].Invoke().GetType());
            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[1].Invoke().GetType());
        }

        [Test]
        public void GetRulesFor_ReturnsAllRegisteredRules()
        {
            var rule = new RuleThatWillAlwaysPass();
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => rule);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));

            Assert.AreEqual(typeof(MeetsTheMinimumRequiredAccountBalance), rules.ToList()[0].Invoke().GetType());
            Assert.AreEqual(typeof(RuleThatWillAlwaysPass), rules.ToList()[1].Invoke().GetType());
        }

        [Test]
        public void RegisterRule_WithRuleContextClassInsteadOfAInterface_ThrowsException()
        {
            Action action = () => RulesInitializer.RegisterRule<RuleContextThatIsAClassInsteadOfAInterface, decimal>(typeof(Account), A.Fake<IRule<decimal>>);
            Assert.Throws<RulesInitializationException>(new TestDelegate(action));
        }

        [Test]
        public void RegisterRule_WithARuleContextDecisionDataTypeAndRuleDecisionDataTypeMismatch_ThrowsException()
        {
            Action action = () => RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), A.Fake<IRule<decimal, string>>);
            Assert.Throws<DecisionDataTypeMismatchException>(new TestDelegate(action));
        }

        [Test]
        public void GetRulesFor_ASubType_OfATypeThatHasRulesRegistered_SubTypeAndBaseTypeRulesAreReturned()
        {
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(AccountSubType), A.Fake<IRule<decimal>>);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(AccountSubType));
            Assert.AreEqual(2, rules.ToList().Count);
        }

        [Test]
        public void GetRulesFor_ForASubTypeOfATypeThatHasRules_OnlyBaseTypeRulesAreReturned()
        {
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(AccountSubType), A.Fake<IRule<decimal>>);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(Account));
            Assert.AreEqual(1, rules.ToList().Count);
        }

        [Test]
        public void GetRulesFor_AInterfaceThatHasOneRuleRegistered_RuleIsReturned()
        {
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(IAccount), A.Fake<IRule<decimal>>);

            var rules = RulesProvider.GetRulesFor<IAccountBalanceRules, decimal>(typeof(IAccount));
            Assert.AreEqual(1, rules.ToList().Count);
        }
	}
}
