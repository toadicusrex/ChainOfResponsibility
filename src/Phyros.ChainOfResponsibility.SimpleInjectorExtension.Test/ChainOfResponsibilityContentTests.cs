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
	public class ChainOfResponsibilityContentTests
	{
		[Fact]
		public void AddChainLink_should_build_a_chain_with_one_link()
		{
			// arrange

			// act
			var chain = ChainOfResponsibilityBuilder.Create<ISomeType>()
				.AddChainLink<NonTerminatingConcreteType>() as ChainOfResponsibility<ISomeType>;

			// assert
			chain.Should().NotBeNull();
			chain.TypeList.Should().HaveCount(1);
			chain.TypeList[0].Should().Be(typeof(NonTerminatingConcreteType));
		}

		[Fact]
		public void AddChainLink_should_build_a_chain_with_two_links_in_the_right_order()
		{
			// arrange

			// act
			var chain = ChainOfResponsibilityBuilder.Create<ISomeType>()
				.AddChainLink<NonTerminatingConcreteType>()
				.AddChainLink<ChainTerminatingConcreteType>() as ChainOfResponsibility<ISomeType>;

			// assert
			chain.Should().NotBeNull();
			chain.TypeList.Should().HaveCount(2);
			chain.TypeList[0].Should().Be(typeof(NonTerminatingConcreteType));
			chain.TypeList[1].Should().Be(typeof(ChainTerminatingConcreteType));
		}

		[Fact]
		public void AddChainLink_should_fail_if_a_terminating_link_is_added_before_another_link()
		{
			// act and assert
			var exception = Assert.Throws<InvalidChainConfigurationException>(() =>  ChainOfResponsibilityBuilder.Create<ISomeType>()
				.AddChainLink<ChainTerminatingConcreteType>()
				.AddChainLink<NonTerminatingConcreteType>() as ChainOfResponsibility<ISomeType>);

			exception.Kind.Should().Be(ExceptionKinds.PrematureTermination);
		}

		[Fact]
		public void AddChainLink_should_fail_if_duplicate_registrations_are_attempted()
		{
			// act and assert
			var exception = Assert.Throws<InvalidChainConfigurationException>(() =>  ChainOfResponsibilityBuilder.Create<ISomeType>()
				.AddChainLink<NonTerminatingConcreteType>()
				.AddChainLink<NonTerminatingConcreteType>() as ChainOfResponsibility<ISomeType>);

			exception.Kind.Should().Be(ExceptionKinds.DuplicateTypeRegistrationInChain);
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

		public class ChainTerminatingConcreteType : ISomeType
		{

		}

		public class AnotherNonTerminatingConcreteType : ISomeType
		{

		}
	}

	
}
