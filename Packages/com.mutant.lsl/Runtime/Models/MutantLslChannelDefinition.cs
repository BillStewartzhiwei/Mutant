using System;

namespace Mutant.LSL
{
	[Serializable]
	public sealed class MutantLslChannelDefinition
	{
		public string label;
		public string unit;
		public string semantic;

		public MutantLslChannelDefinition()
		{
		}

		public MutantLslChannelDefinition(string channelLabel, string channelUnit = "", string channelSemantic = "")
		{
			label = channelLabel;
			unit = channelUnit;
			semantic = channelSemantic;
		}
	}
}
