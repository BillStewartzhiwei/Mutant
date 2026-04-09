using System;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslMetadataEntry
	{
		public string key;
		public string value;

		public MutantLslMetadataEntry()
		{
		}

		public MutantLslMetadataEntry(string metadataKey, string metadataValue)
		{
			key = metadataKey;
			value = metadataValue;
		}
	}
}
