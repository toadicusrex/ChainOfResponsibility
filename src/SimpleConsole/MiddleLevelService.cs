internal class MiddleLevelService : ISimpleService
{
	private readonly ISimpleService _next;

	public MiddleLevelService(ISimpleService next)
	{
		_next = next;
	}
	public void WriteEntryAndExitToConsole()
	{
		Console.WriteLine($"{nameof(MiddleLevelService)}.{nameof(WriteEntryAndExitToConsole)} is starting.");
		_next.WriteEntryAndExitToConsole();
		Console.WriteLine($"{nameof(MiddleLevelService)}.{nameof(WriteEntryAndExitToConsole)} is exiting.");
	}
}