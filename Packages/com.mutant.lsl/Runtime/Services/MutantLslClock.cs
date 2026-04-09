using LSL;

namespace Mutant.LSL
{
	public sealed class MutantLslClock : IMutantLslClock
	{
		public double Now()
		{
			return global::LSL.LSL.local_clock();
		}
	}
}
