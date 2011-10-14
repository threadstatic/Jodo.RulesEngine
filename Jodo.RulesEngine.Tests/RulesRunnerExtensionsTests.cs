using System;
using Jodo.Rules;
using NUnit.Framework;

namespace Jodo.Tests
{
    public class RulesRunnerExtensionsTests
    {
        private RulesEngine rulesEngine;
        private IRulesInitializer RulesInitializer { get { return rulesEngine; } }
        private IRulesRunner RulesRunner { get { return rulesEngine; } }

        [SetUp]
        public void Setup()
        {
            rulesEngine = new RulesEngine();
        }

        [TearDown]
        public void TearDown()
        {
            rulesEngine.Dispose();
        }

        [Test]
        public void TestRules_OneRuleRegisteredAndThatRuleFails_ExceptionThrown()
        {
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => new MeetsTheMinimumRequiredAccountBalance(100));
            Assert.Catch(typeof(InvalidOperationException), () => RulesRunner.TestRules<IAccountBalanceRules, decimal>(typeof(Account), 99));
        }

        [Test]
        public void TestRules_TwoRulesRegisteredAndOneRuleFails_ExceptionThrown()
        {
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => new MeetsTheMinimumRequiredAccountBalance(100));
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => new RuleThatWillAlwaysPass());
            Assert.Catch(typeof(InvalidOperationException), () => RulesRunner.TestRules<IAccountBalanceRules, decimal>(typeof(Account), 99));
        }

        [Test]
        public void TestRules_TwoRulesRegisteredThatBothPass_DoesNotThrowException()
        {
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => new MeetsTheMinimumRequiredAccountBalance(100));
            RulesInitializer.RegisterRule<IAccountBalanceRules, decimal>(typeof(Account), () => new RuleThatWillAlwaysPass());
            Assert.DoesNotThrow(() => RulesRunner.TestRules<IAccountBalanceRules, decimal>(typeof(Account), 100));
        }

        [Test]
        public void TestRulesWithDecisionData_OneRuleRegisteredAndThatRuleFails_ExceptionThrown()
        {
            RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new RuleWithDecisionDataThatWillAlwaysFail());
            Assert.Catch(typeof(InvalidOperationException), () => RulesRunner.TestRules<IRuleContextWithDecimalDecisionData, decimal, decimal>(typeof(Account), 99, 1));
        }

        [Test]
        public void TestRulesWithDecisionData_TwoRulesRegisteredAndOneRuleFails_ExceptionThrown()
        {
            RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new RuleWithDecisionDataThatWillAlwaysPass());
            RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new RuleWithDecisionDataThatWillAlwaysFail());
            Assert.Catch(typeof(InvalidOperationException), () => RulesRunner.TestRules<IRuleContextWithDecimalDecisionData, decimal, decimal>(typeof(Account), 99, 1));
        }

        [Test]
        public void TestRulesWithDecisionData_OneRuleRegisteredThatPasses_DoesNotThrowException()
        {
            RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new RuleWithDecisionDataThatWillAlwaysPass());
            Assert.DoesNotThrow(() => RulesRunner.TestRules<IRuleContextWithDecimalDecisionData, decimal, decimal>(typeof(Account), 100, 0));
        }

        [Test]
        public void TestRulesWithDecisionData_TwoRulesRegisteredThatBothPass_DoesNotThrowException()
        {
            RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new RuleWithDecisionDataThatWillAlwaysPass());
            RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new AnotherRuleWithDecisionDataThatWillAlwaysPass());
            Assert.DoesNotThrow(() => RulesRunner.TestRules<IRuleContextWithDecimalDecisionData, decimal, decimal>(typeof(Account), 100, 0));
        }

        [Test]
        public void TestRules_RegisteredRuleHasDecisionDataAndTheDecisionDataOverloadIsNotUsed_DoesNotThrowException()
        {
            RulesInitializer.RegisterRule<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), () => new RuleThatWillAlwaysPassAndDeclaresDecisionDataButDoesNotUseTheDecisionData());
            Assert.DoesNotThrow(() => RulesRunner.TestRules<IRuleContextWithDecimalDecisionData, decimal>(typeof(Account), 100));
        }

        [Test]
        public void TestRules_RegisteredRuleHasDecisionDataAndTheDecisionDataOverloadIsNotUsed_ThrowsException()
        {
            RulesInitializer.RegisterRule<IRuleContextWithNullableDecisionData, decimal>(typeof(Account), () => new RuleDeclaresAndUsesDecisionDataThatWillThrowAnExceptionIfNull());
            Assert.Catch(typeof(NullReferenceException), () => RulesRunner.TestRules<IRuleContextWithNullableDecisionData, decimal>(typeof(Account), 100));
        }
    }
}
