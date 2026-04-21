using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using Mutant.Core.Modules;
using UnityEngine.Assertions;
using Assert = UnityEngine.Assertions.Assert;

namespace Mutant.Core.Tests.Runtime
{
	public class ModuleManagerRuntimeTests
	{
		[UnitySetUp]
		public IEnumerator SetUp()
		{
			ModuleManager.Instance.DisposeAll();
			yield return null;
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			ModuleManager.Instance.DisposeAll();
			yield return null;
		}

		[UnityTest]
		public IEnumerator RegisterThenInit_WorksInRuntime()
		{
			TestModule module = new TestModule();
			ModuleManager.Instance.Register(module);
			ModuleManager.Instance.InitAll();

			yield return null;

			Assert.IsTrue(module.IsInitialized);
		}

		private sealed class TestModule : ModuleBase
		{
			protected override void OnInit() { }
		}
	}
}
