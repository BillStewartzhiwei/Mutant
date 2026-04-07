using NUnit.Framework;
using Mutant.Log.Config;
using Mutant.Log.Modules;

namespace Mutant.Log.Tests.Editor
{
	public class MutantLogEditorSmokeTests
	{
		[Test]
		public void RuntimeFallbackSettings_CanBeCreated()
		{
			MutantLogRuntimeSettings settingsAsset = MutantLogRuntimeSettings.CreateRuntimeFallback();

			Assert.IsNotNull(settingsAsset);
		}

		[Test]
		public void Configure_WithNull_DoesNotThrow()
		{
			Assert.DoesNotThrow(() => MutantLogModule.Configure(null));
		}
	}
}
