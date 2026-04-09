namespace Mutant.LSL
{
	public static class MutantLslInstaller
	{
		public static MutantLslModule InstallDefault(bool setAsCurrent = true)
		{
			return MutantLslModule.CreateDefault(setAsCurrent);
		}
	}
}
