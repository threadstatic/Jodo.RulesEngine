using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Jodo.Rules;

namespace Jodo
{
    [Export(typeof(IRulesRunner))]
    [Export(typeof(IRulesInitializer))]
	public sealed class RulesEngine : IRulesInitializer, IRulesProvider, IRulesRunner, IDisposable
	{
		private static readonly Dictionary<RuleKey, IEnumerable<object>> RulesStore = new Dictionary<RuleKey, IEnumerable<object>>();
        private static IRulesRunner rulesRunner = new RulesEngine();
        internal static IRulesRunner RulesRunner { get { return rulesRunner; } }

		#region IRulesInitializer Implementation

		IRulesInitializer IRulesInitializer.RegisterRule<TRuleContext, TCandidate>(Type rulesForType, Func<IRule<TCandidate>> rule)
		{
			return ((IRulesInitializer)this).RegisterRules<TRuleContext, TCandidate>(rulesForType, new List<Func<IRule<TCandidate>>> {rule});
		}
		
		IRulesInitializer IRulesInitializer.RegisterRules<TRuleContext, TCandidate>(Type rulesForType, IEnumerable<Func<IRule<TCandidate>>> rules)
		{
			ValidateSpecificationContext<TRuleContext>();
            ValidateDecisionDataType<TRuleContext, TCandidate>(rules);

			RuleKey key = new RuleKey(new TypePair(rulesForType, typeof(TRuleContext)));

			// Overwrite a existing rule of the same type
            if (RulesStore.ContainsKey(key))
            {
                var existingRulesForKey = RulesStore[key].ToList();
                existingRulesForKey.AddRange(rules);
                RulesStore[key] = existingRulesForKey;
            }
            else 
            { 
                RulesStore[key] = rules; 
            }
			
			return this;
		}

		#endregion

		#region IRulesProvider Implementation

        /// <summary>
        /// Returns all rules for the requested type.
        /// </summary>
		/// <typeparam name="TRuleContext">The context in which to retrieve rules for.</typeparam>
		/// <typeparam name="TCandidate">The rule candidate type.</typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
        /// <returns>A IEnumerable of rule factories.</returns>
		IEnumerable<Func<IRule<TCandidate>>> IRulesProvider.GetRulesFor<TRuleContext, TCandidate>(Type typeToGetRulesFor)
		{
			return new RulesFetcher<TRuleContext, TCandidate, object>().GetRulesFor(typeToGetRulesFor);
		}
		
		#endregion

        #region IRulesRunner Implementation

        void IRulesRunner.TestRules<TRuleContext, TCandidate>(Type typeToGetRulesFor, TCandidate candidate)
        {
            RulesRunner.TestRules<TRuleContext, TCandidate>(this, typeToGetRulesFor, candidate);
        }

        void IRulesRunner.TestRules<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData) 
        {
            RulesRunner.TestRules<TRuleContext, TCandidate, TDecisionData>(this, typeToGetRulesFor, candidate, decisionData);
        }

        #endregion

        #region RulesRunner Registration

        public static void RegisterRulesRunner(IRulesRunner runner)
        {
            rulesRunner = runner;
        }

        #endregion

        #region Classes

        private class RuleKey
		{
			private TypePair TypePair { get; set; }
		
			public RuleKey(TypePair typePair)
			{
				TypePair = typePair;
			}

			public override bool Equals(object obj)
			{
				return TypePair.Equals(((RuleKey)obj).TypePair);
			}

			public override int GetHashCode()
			{
				return TypePair.GetHashCode();
			}
		}

        private class RulesFetcher<TRuleContext, TCandidate, TDecisionData> where TRuleContext : IRule<TCandidate>
		{
			public List<Func<IRule<TCandidate>>> GetRulesFor(Type type)
			{
				List<Func<IRule<TCandidate>>> rules = new List<Func<IRule<TCandidate>>>();

				GetRulesForAllInterfaces(rules, type);
				GetRulesForAllClasses(rules, type);

				return rules;
			}

			private void GetRulesForAllInterfaces(List<Func<IRule<TCandidate>>> rules, Type type)
			{
				foreach (Type t in type.GetInterfaces())
				{
					RuleKey key = new RuleKey(new TypePair(t, typeof(TRuleContext)));

					if (RulesStore.ContainsKey(key))
					{
						foreach (Func<IRule<TCandidate>> specification in RulesStore[key].Cast<Func<IRule<TCandidate>>>())
							rules.Add(specification);
					}
				}
			}

			private void GetRulesForAllClasses(List<Func<IRule<TCandidate>>> rules, Type type)
			{
				RuleKey key = new RuleKey(new TypePair(type, typeof(TRuleContext)));

				if (RulesStore.ContainsKey(key))
				{
					foreach (Func<IRule<TCandidate>> specification in RulesStore[key].Cast<Func<IRule<TCandidate>>>())
					{
						if (specification != null)
							rules.Add(specification);
					}
				}

				if (type.BaseType != null)
					GetRulesForAllClasses(rules, type.BaseType);
			}
		}

		#endregion

		#region Rules Initialization Validation

		/// <summary>
		/// Validates that the declared TRuleContext type is
		/// a interface.		
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		private void ValidateSpecificationContext<TRuleContext>()
		{
			Type specificationContextType = typeof(TRuleContext);

			if (!specificationContextType.IsInterface)
				throw new RulesInitializationException(String.Format("The specification context of type {0}, must be a interface.", typeof(TRuleContext)));
		}

		/// <summary>
		/// If a usage of the <see cref="Jodo.Rules.IRule{TCandidate}"/> interface
        /// is discovered, the TDecisionData type supplied on the TRuleContext
        /// will be validated against the TDecisionData type supplied 
        /// in the <see cref="Jodo.Rules.IRule{TCandidate}"/> interface.
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <param name="rules"></param>
		/// <exception cref="DecisionDataTypeMismatchException">
        /// Throw if the  the TDecisionData type supplied on the TRuleContext
        /// does not match the TDecisionData type supplied 
        /// in the <see cref="Jodo.Rules.IRule{TCandidate}"/> interface.
		/// </exception>
        private void ValidateDecisionDataType<TRuleContext, TCandidate>(IEnumerable<Func<IRule<TCandidate>>> rules)
		{
			if (rules == null)
				return;

            Type decisionDataType = null;

			foreach (Type contextsInterface in typeof(TRuleContext).GetInterfaces())
			{
				Type[] genArgs = contextsInterface.GetGenericArguments();
				Type[] contextsInterfaces = contextsInterface.GetInterfaces();

                // we are looking for a interface of type ISpecification<TCandidate, TDecisionData>
				// we are trying to ensure we have the correct interface so that the decisionDataType we find is of the correct Type
				// if we have it, the interfaces genArgs.Length will equal 2
				// it will also inherit the ISpecification<TCandidate> interface so its GetInterfaces method should return a length greater then 0.
				// here we are checking for these conditions

				if (genArgs.Length == 2 && contextsInterfaces.Length > 0 && (contextsInterface.GetInterfaces()[0] == typeof(IRule<>).MakeGenericType(new[] {typeof(TCandidate)})))
				{
                    decisionDataType = genArgs[1];
					break;
				}
			}

            if (decisionDataType != null)
			{
				foreach (Func<IRule<TCandidate>> spec in rules)
				{
					if (spec == null)
						continue;

					foreach (Type specsInterface in spec().GetType().GetInterfaces())
					{
						Type[] genArgs = specsInterface.GetGenericArguments();

                        // we are looking for a interface of type ISpecification<TCandidate, TDecisionData>
						// here we are trying to ensure we have the correct interface
						if (genArgs.Length == 2)
						{
							Type[] specificationWithTypeGenArgs = specsInterface.GetInterfaces();

							if ((specificationWithTypeGenArgs[0] == typeof(IRule<>).MakeGenericType(new[] {typeof(TCandidate)})) && genArgs[1] != decisionDataType)
                                throw new DecisionDataTypeMismatchException(String.Format("The decision data type parameter declared on specifcation {0}, does not match the decision data type parameter declared on the specification context interface {1}.", spec, typeof(TRuleContext)));
						}
					}
				}
			}
		}

		#endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            RulesStore.Clear();
        }

        #endregion
    }
}