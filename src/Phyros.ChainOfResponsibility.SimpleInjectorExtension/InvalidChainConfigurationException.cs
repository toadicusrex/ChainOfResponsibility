using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phyros.ChainOfResponsibility.SimpleInjectorExtension
{
	public class InvalidChainConfigurationException : Exception
	{
		public ExceptionKinds Kind { get; }

		public InvalidChainConfigurationException(ExceptionKinds kind, string message) : base(message)
		{
			Kind = kind;
		}
	}

	public enum ExceptionKinds
	{
		DuplicateTypeRegistrationInChain,
		PrematureTermination,
		UnterminatedChain,
		EmptyChainUponRegistration
	}
}
