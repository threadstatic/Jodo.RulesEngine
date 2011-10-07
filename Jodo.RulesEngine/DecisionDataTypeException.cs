using System;
using System.Runtime.InteropServices;
using Jodo.Rules;

namespace Jodo
{
	/// <summary>
	/// The exception that is thrown when the declared decision data generic type
	/// on a rule context interface does not match the
	/// decision data generic type declared on the
	/// <see cref="IRule{TCandidate}"/>
	/// being registered.	
	/// </summary>
	[Serializable]
	[ComVisible(true)]
	public class DecisionDataTypeException : RulesInitializationException
	{
		public DecisionDataTypeException(string message) : base(message)
		{
		}
	}
}