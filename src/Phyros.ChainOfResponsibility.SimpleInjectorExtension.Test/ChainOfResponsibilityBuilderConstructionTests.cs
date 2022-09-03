using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Phyros.ChainOfResponsibility.SimpleInjectorExtension.Test
{
	public class ChainOfResponsibilityBuilderConstructionTests
	{
		[Fact]
		public void Create_should_build_an_empty_chain_of_responsibility()
		{
			// arrange

			// act
			var chain = ChainOfResponsibilityBuilder.Create<ISomeType>() as ChainOfResponsibility<ISomeType>;

			// assert
			chain.Should().NotBeNull();
			chain.TypeList.Should().BeEmpty();
		}

		public interface ISomeType
		{
		}
	}
}
