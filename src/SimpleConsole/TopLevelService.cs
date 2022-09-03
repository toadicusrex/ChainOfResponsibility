public class TopLevelService : ISimpleService
{
	private readonly ISimpleService _next;

	public TopLevelService(ISimpleService next)
	{
		_next = next;
	}
	public void WriteEntryAndExitToConsole()
	{
		Console.WriteLine($"{nameof(TopLevelService)}.{nameof(WriteEntryAndExitToConsole)} is starting.");
		_next.WriteEntryAndExitToConsole();
		Console.WriteLine($"{nameof(TopLevelService)}.{nameof(WriteEntryAndExitToConsole)} is exiting.");
	}
}