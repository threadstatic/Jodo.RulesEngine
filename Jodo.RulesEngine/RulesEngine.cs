using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Jodo.Rules;

namespace Jodo
{
    [Export(typeof(IRulesRunner))]
	public sealed class RulesEngine : IRulesInitializer, IRulesProvider, IRulesRunner
	{
		private static readonly Dictionary<RuleKey, IEnumerable<object>> RulesStore = new Dictionary<RuleKey, IEnumerable<object>>();

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
		///
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <returns></returns>
		IEnumerable<Func<IRule<TCandidate>>> IRulesProvider.GetRulesFor<TRuleContext, TCandidate>(Type typeToGetRulesFor)
		{
			return new RulesFetcher<TRuleContext, TCandidate, object>().GetRulesFor(typeToGetRulesFor);
		}

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using an AND <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <returns></returns>
		IRule<TCandidate> IRulesProvider.GetRuleFor<TRuleContext, TCandidate>(Type typeToGetRulesFor)
		{
			return new RulesFetcher<TRuleContext, TCandidate, object>().GetRuleFor(typeToGetRulesFor, RuleCompositionStrategy.And, null);
		}

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using a <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <param name="compositionStrategy"></param>
		/// <returns></returns>
		IRule<TCandidate> IRulesProvider.GetRuleFor<TRuleContext, TCandidate>(Type typeToGetRulesFor, RuleCompositionStrategy compositionStrategy)
		{
			return new RulesFetcher<TRuleContext, TCandidate, object>().GetRuleFor(typeToGetRulesFor, compositionStrategy, null);
		}

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using an AND <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
        /// <typeparam name="TDecisionData"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
        /// <param name="decisionData"></param>
		/// <returns></returns>
        IRule<TCandidate> IRulesProvider.GetRuleFor<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TDecisionData decisionData)
		{
            return new RulesFetcher<TRuleContext, TCandidate, TDecisionData>().GetRuleFor(typeToGetRulesFor, RuleCompositionStrategy.And, decisionData);
		}

		/// <summary>
		/// Merges all registered specifications for the supplied object
		/// into one composite specification using a <see cref="RuleCompositionStrategy"/>.	
		/// </summary>
		/// <typeparam name="TRuleContext"></typeparam>
		/// <typeparam name="TCandidate"></typeparam>
        /// <typeparam name="TDecisionData"></typeparam>
		/// <param name="typeToGetRulesFor">The type to retrieve the registered rules for.</param>
		/// <param name="compositionStrategy"></param>
		/// <returns></returns>
        IRule<TCandidate> IRulesProvider.GetRuleFor<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, RuleCompositionStrategy compositionStrategy, TDecisionData decisionData)
		{
            return new RulesFetcher<TRuleContext, TCandidate, TDecisionData>().GetRuleFor(typeToGetRulesFor, compositionStrategy, decisionData);
		}

		#endregion

        #region IRulesRunner Implementation

        void IRulesRunner.TestRules<TRuleContext, TCandidate>(Type typeToGetRulesFor, TCandidate candidate)
        {
            this.TestRules<TRuleContext, TCandidate>(this, typeToGetRulesFor, candidate);
        }

        void IRulesRunner.TestRules<TRuleContext, TCandidate, TDecisionData>(Type typeToGetRulesFor, TCandidate candidate, TDecisionData decisionData) 
        {
            this.TestRules<TRuleContext, TCandidate, TDecisionData>(this, typeToGetRulesFor, candidate, decisionData);
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
            public IRule<TCandidate> GetRuleFor(Type type, RuleCompositionStrategy compositionStrategy, TDecisionData decisionData)
			{
				List<Func<IRule<TCandidate>>> ruleDelegates = GetRulesFor(type);
				if (ruleDelegates.Count < 1)
					return new NullRule<TCandidate>();

				List<IRule<TCandidate>> rules = new List<IRule<TCandidate>>();

				IRule<TCandidate> compositeRule = ruleDelegates[0].Invoke();
                SetDecisionData(compositeRule, decisionData);

				rules.Add(compositeRule);

				for (int i = 1; i < ruleDelegates.Count; i++)
				{
					// we must invoke the delegate in order to evaluate the runtime type it will return
					// we are looking to see if we already have that Specification Type.
					IRule<TCandidate> spec = ruleDelegates[i].Invoke();

					if (rules.Contains(spec, new SpecificationComparer<TCandidate>()))
						continue;

					rules.Add(spec);
                    SetDecisionData(spec, decisionData);

					switch (compositionStrategy)
					{
						case RuleCompositionStrategy.And:
							compositeRule = compositeRule.And(spec);
							break;
						case RuleCompositionStrategy.Or:
							compositeRule = compositeRule.Or(spec);
							break;
						default:
							compositeRule = compositeRule.And(spec);
							break;
					}
				}

				return compositeRule;
			}

			public List<Func<IRule<TCandidate>>> GetRulesFor(Type type)
			{
				List<Func<IRule<TCandidate>>> rules = new List<Func<IRule<TCandidate>>>();

				GetRulesForAllInterfaces(rules, type);
				GetRulesForAllClasses(rules, type);

				return rules;
			}

            private void SetDecisionData(IRule<TCandidate> spec, TDecisionData decisionData)
			{
                IRule<TCandidate, TDecisionData> decisionDataRule = spec as IRule<TCandidate, TDecisionData>;

                if (decisionDataRule != null)
                    decisionDataRule.DecisionData = decisionData;
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

			private class SpecificationComparer<TCand> : IEqualityComparer<IRule<TCand>>
			{
				public bool Equals(IRule<TCand> x, IRule<TCand> y)
				{
					return x.GetType() == y.GetType();
				}

				public int GetHashCode(IRule<TCand> obj)
				{
					return obj.GetType().GetHashCode();
				}
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
		/// <exception cref="DecisionDataTypeException">
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
                                throw new DecisionDataTypeException(String.Format("The decision data type parameter declared on specifcation {0}, does not match the decision data type parameter declared on the specification context interface {1}.", spec, typeof(TRuleContext)));
						}
					}
				}
			}
		}

		#endregion
    }
}