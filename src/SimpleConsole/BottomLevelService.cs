internal class BottomLevelService : ISimpleService
{
	public void WriteEntryAndExitToConsole()
	{
		Console.WriteLine($"{nameof(BottomLevelService)}.{nameof(WriteEntryAndExitToConsole)} is starting.");
		Console.WriteLine($"{nameof(BottomLevelService)}.{nameof(WriteEntryAndExitToConsole)} is exiting.");
	}
}