using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using SimpleInjector;
using Xunit;

namespace Phyros.ChainOfResponsibility.SimpleInjectorExtension.Test
{
	public class ChainOfResponsibilityRegistrationTests
	{
		[Fact]
		public void RegisterWithSimpleInjector_should_throw_argument_null_exception_if_a_null_container_is_provided()
		{
			// act and assert
			Assert.Throws<ArgumentNullException>(() =>  ChainOfResponsibilityBuilder.Create<ISomeType>().RegisterWithSimpleInjector(null));
		}

		[Fact]
		public void RegisterWithSimpleInjector_should_throw_argument_null_exception_if_chain_is_empty()
		{
			// ordinarily, I'd use a mock here, but this feels harmless enough as this project is very small, and there really isn't an easy
			// way to mock a SimpleInjector container (and that would be weird anyway, right?)
			var container = new Container();

			// act and assert
			var exception = Assert.Throws<InvalidChainConfigurationException>(() =>  ChainOfResponsibilityBuilder.Create<ISomeType>().RegisterWithSimpleInjector(container));

			exception.Kind.Should().Be(ExceptionKinds.EmptyChainUponRegistration);
		}

		[Fact]
		public void RegisterWithSimpleInjector_should_throw_argument_null_exception_if_chain_is_not_complete()
		{
			// ordinarily, I'd use a mock here, but this feels harmless enough as this project is very small, and there really isn't an easy
			// way to mock a SimpleInjector container (and that would be weird anyway, right?)
			var container = new Container();

			// act and assert
			var exception = Assert.Throws<InvalidChainConfigurationException>(() =>  ChainOfResponsibilityBuilder.Create<ISomeType>()
				.AddChainLink<NonTerminatingConcreteType>()
				.RegisterWithSimpleInjector(container));

			exception.Kind.Should().Be(ExceptionKinds.UnterminatedChain);
		}

		public interface ISomeType
		{
		}

		public class NonTerminatingConcreteType : ISomeType
		{
			public NonTerminatingConcreteType(ISomeType next)
			{

			}
		}
	}

	
}
