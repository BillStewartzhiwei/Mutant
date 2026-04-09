namespace Mutant.LSL
{
	public enum MutantLslConnectionState
	{
		None = 0,
		Resolving = 1,
		Connected = 2,
		Disconnected = 3,
		Reconnecting = 4,
		Faulted = 5,
		Disposed = 6
	}
}
