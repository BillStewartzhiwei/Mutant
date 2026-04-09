using System;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslResolverConfig
	{
		public MutantLslResolveMode resolveMode = MutantLslResolveMode.ByProperty;
		public string propertyName = "name";
		public string propertyValue = "";
		public string predicate = "";
		public int minimumResultCount = 1;
		public double timeoutSeconds = MutantLslConstants.DefaultResolveTimeoutSeconds;
	}
}
