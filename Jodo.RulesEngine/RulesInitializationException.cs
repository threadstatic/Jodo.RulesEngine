using System;
using System.Runtime.InteropServices;

namespace Jodo
{
	/// <summary>
	/// The exception that is thrown when rules are being registered
	/// that have invalid configuraion.
	/// </summary>
	[Serializable]
	[ComVisible(true)]
	public class RulesInitializationException : ApplicationException
	{
		public RulesInitializationException(string message) : base(message)
		{
		}
	}
}