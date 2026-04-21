using NUnit.Framework;
using Mutant.Core.Modules;
using UnityEngine.Assertions;

namespace Mutant.Core.Tests.Editor
{
	public class ModuleManagerEditorTests
	{
		[SetUp]
		public void SetUp()
		{
			ModuleManager.Instance.DisposeAll();
			ModuleManager.Instance.Configure(false);
		}

		[TearDown]
		public void TearDown()
		{
			ModuleManager.Instance.DisposeAll();
		}

		[Test]
		public void Register_SameTypeOnlyOnce()
		{
			bool first = ModuleManager.Instance.Register(new TestModule());
			bool second = ModuleManager.Instance.Register(new TestModule());

			Assert.IsTrue(first);
			Assert.IsFalse(second);
			Assert.AreEqual(1, ModuleManager.Instance.GetAllModules().Count);
		}

		[Test]
		public void InitAll_ThenLateRegister_AutoInits()
		{
			ModuleManager.Instance.InitAll();

			TestModule module = new TestModule();
			ModuleManager.Instance.Register(module);

			Assert.IsTrue(module.IsInitialized);
		}

		private sealed class TestModule : ModuleBase
		{
			protected override void OnInit() { }
		}
	}
}
