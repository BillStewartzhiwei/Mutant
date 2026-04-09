namespace Mutant.LSL
{
	public interface IMutantLslResolver
	{
		MutantLslResolvedStreamInfo[] Resolve(MutantLslResolverConfig config);
	}
}
