using System.Runtime.CompilerServices;
using SimpleInjector;

namespace Phyros.ChainOfResponsibility.SimpleInjectorExtension
{
	public static class ChainOfResponsibilityBuilder
	{
		public static IChainOfResponsibility<TLinkInterface> Create<TLinkInterface>()
		{
			return new ChainOfResponsibility<TLinkInterface>();
		}
	}

	public interface IChainOfResponsibility<TLinkInterface>
	{
		/// <summary>
		/// Adds the chain link.  Note that if the type provided does not have an object of type T provided to it in its constructor, the chain will effectively be terminated with that link.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="Exception">It is an error to attempt to register a chain of responsibility with a duplicate type.  Please remove all but one duplicate.</exception>
		IChainOfResponsibility<TLinkInterface> AddChainLink<T>() where T : TLinkInterface;

		/// <summary>
		/// Registers the chain of responsibility with SimpleInjector. If we need to add this to another IOC, this could easily be separated off into an extension library.
		/// NOTE: This implementation is somewhat complex, tread lightly.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <exception cref="Exception">You must have completed the chain of responsibility (i.e. '{nameof(_chainOfResponsibility.AddFinalChainLink)}') in order to register the chain.  Therefore, for your last link, call '{nameof(_chainOfResponsibility.AddFinalChainLink)}' instead.</exception>
		void RegisterWithSimpleInjector(Container container);
	}

	public class ChainOfResponsibility<TLinkInterface> : IChainOfResponsibility<TLinkInterface>
	{
		public readonly List<Type> TypeList = new List<Type>();
		public bool IsComplete { get; private set; }

		internal ChainOfResponsibility()
		{

		}

		/// <summary>
		/// Adds the chain link.  Note that if the type provided does not have an object of type T provided to it in its constructor, the chain will effectively be terminated with that link.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="Exception">It is an error to attempt to register a chain of responsibility with a duplicate type.  Please remove all but one duplicate.</exception>
		public IChainOfResponsibility<TLinkInterface> AddChainLink<T>() where T : TLinkInterface
		{
			if (TypeList.Contains(typeof(T)))
			{
				throw new InvalidChainConfigurationException(ExceptionKinds.DuplicateTypeRegistrationInChain,
				$"It is an error to attempt to register a chain of responsibility with a duplicate type.  Please remove all but one duplicate.");
			}

			if (IsComplete)
			{
				throw new InvalidChainConfigurationException(ExceptionKinds.PrematureTermination,
				$"ChainOfResponsibilityBuilder cannot add another link of type '{typeof(T).Name}' as the previous link did not have a constructor parameter of type '{typeof(TLinkInterface).Name}', effectively ending the chain.  Add the 'next' parameter to your class or remove the link.");
			}

			var constructors = typeof(T).GetConstructors();
			if (!constructors.Any(x => x.GetParameters().Any(p => p.ParameterType.IsAssignableFrom(typeof(T)))))
			{
				IsComplete = true;
			}

			TypeList.Add(typeof(T));
			return this;
		}

		/// <summary>
		/// Registers the chain of responsibility with SimpleInjector. If we need to add this to another IOC, this could easily be separated off into an extension library.
		/// NOTE: This implementation is somewhat complex, tread lightly.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <exception cref="InvalidChainConfigurationException">You must have completed the chain of responsibility (i.e. '{nameof(_chainOfResponsibility.AddFinalChainLink)}') in order to register the chain.  Therefore, for your last link, call '{nameof(_chainOfResponsibility.AddFinalChainLink)}' instead.</exception>
		public void RegisterWithSimpleInjector(Container container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			if (!TypeList.Any())
			{
				throw new InvalidChainConfigurationException(ExceptionKinds.EmptyChainUponRegistration,
					"Attempted to register the chain with SimpleInjector without any chain link registrations.");
			}
			// verify that the chain of responsibility is complete (i.e. the last one doesn't have a "next" parameter in its constructor pointing to the next link in the chain.
			if (!IsComplete)
			{
				throw new InvalidChainConfigurationException(ExceptionKinds.UnterminatedChain,
				$"You must have completed the chain of responsibility (i.e. the last link cannot have a 'next' parameter of type '{typeof(TLinkInterface).Name}') prior to registering with the IOC.");
			}

			var reversedTypes = TypeList.ToList();
			reversedTypes.Reverse(); // reversing the list; we'll build it from the Final link to the First link.
															 //DEFINITION: ChainOfResponsibilityBuilder target type - this is the type of object we're chaining; a ChainOfResponsibilityBuilder<IThing> target type is the IThing.
															 //DEFINITION: Final link - this is the last link in the chain of responsibility and cannot have a parameter of the ChainOfResponsibilityBuilder target type in its constructor (this avoids infinite creation loops).
															 //DEFINITION: Default link - this is the first link in the chain; this is the object that will be returned to any object requesting  the ChainOfResponsibilityBuilder target type, and if it is not also the Final link it will have a reference to the next link in the chain.

			// we're now going to loop over all of the types in the chain of responsibility and register them.
			foreach (var currentLinkType in reversedTypes)
			{
				if (reversedTypes.Last() != currentLinkType)
				{
					// For everything but the Default link, we only want to resolve it's type during construction of the upstream link.  We'll first identify the upstream link:
					var currentIndex = reversedTypes.IndexOf(currentLinkType);
					var upstreamLinkType = reversedTypes[currentIndex + 1];
					// now we'll register it, so when SimpleInjector creates an object of the upstreamLinkType, it will use the currentLinkType to fulfill it's constructor parameter.
					container.RegisterConditional(typeof(TLinkInterface), currentLinkType,
					c => c.HasConsumer && c.Consumer?.ImplementationType == upstreamLinkType);
				}
				else
				{
					// this is the Default link, so it should be returned to all classes that need an object of type TLink (except those in the chain, obviously).
					container.RegisterConditional(typeof(TLinkInterface), currentLinkType,
					c => !c.Handled);
				}
			}
		}
	}

}