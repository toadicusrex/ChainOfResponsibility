// See https://aka.ms/new-console-template for more information

using Phyros.ChainOfResponsibility.SimpleInjectorExtension;
using SimpleInjector;

Console.WriteLine("Hello, World!");

var container = new Container();

ChainOfResponsibilityBuilder.Create<ISimpleService>()
	.AddChainLink<TopLevelService>()
	.AddChainLink<MiddleLevelService>()
	.AddChainLink<BottomLevelService>()
	.RegisterWithSimpleInjector(container);

container.Verify();

var simpleService = container.GetInstance<ISimpleService>();

simpleService.WriteEntryAndExitToConsole();
Console.ReadKey();