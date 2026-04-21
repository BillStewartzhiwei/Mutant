using System.Collections;
using Mutant.Core.Bootstrap;
using Mutant.Core.Diagnostics;
using Mutant.Core.Modules;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Mutant.Core.Tests.PlayMode
{
    public sealed class CoreBootstrapPlayModeTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            ModuleManager.Instance.DisposeAll();
            CoreRecorder.Clear();
            CoreRecorder.Enabled = true;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (CoreBootstrap bootstrap in Object.FindObjectsOfType<CoreBootstrap>())
                Object.Destroy(bootstrap.gameObject);

            yield return null;

            ModuleManager.Instance.DisposeAll();
            CoreRecorder.Enabled = false;
            CoreRecorder.Clear();
        }

        [UnityTest]
        public IEnumerator Destroying_Duplicate_CoreBootstrap_Does_Not_Dispose_Modules()
        {
            TrackingModule module = new();
            ModuleManager.Instance.Register(module);

            GameObject ownerGo = new("OwnerBootstrap");
            ownerGo.AddComponent<CoreBootstrap>();
            yield return null;

            GameObject duplicateGo = new("DuplicateBootstrap");
            duplicateGo.AddComponent<CoreBootstrap>();
            yield return null;

            Assert.That(module.DisposeCount, Is.EqualTo(0),
                "Destroying duplicate bootstrap should not dispose modules.");

            Object.Destroy(ownerGo);
            yield return null;

            Assert.That(module.DisposeCount, Is.EqualTo(1),
                "Owner bootstrap destruction should dispose modules exactly once.");
        }

        [UnityTest]
        public IEnumerator CoreRecorder_Should_Record_Bootstrap_And_Module_Lifecycle()
        {
            TrackingModule module = new();
            ModuleManager.Instance.Register(module);

            GameObject ownerGo = new("RecorderOwnerBootstrap");
            ownerGo.AddComponent<CoreBootstrap>();
            yield return null;

            Object.Destroy(ownerGo);
            yield return null;

            Assert.That(CoreRecorder.GetEntries().Count, Is.GreaterThan(0));

            bool hasOwnerInit = false;
            bool hasInitAll = false;
            bool hasDisposeAll = false;

            foreach (CoreRecordEntry entry in CoreRecorder.GetEntries())
            {
                hasOwnerInit |= entry.Message.Contains("Owner bootstrap initialized.");
                hasInitAll |= entry.Message.Contains("InitAll");
                hasDisposeAll |= entry.Message.Contains("DisposeAll");
            }

            Assert.That(hasOwnerInit, Is.True, "Recorder should include bootstrap init.");
            Assert.That(hasInitAll, Is.True, "Recorder should include module init cycle.");
            Assert.That(hasDisposeAll, Is.True, "Recorder should include module dispose cycle.");
        }

        private sealed class TrackingModule : IModule
        {
            public int Priority => 0;
            public int DisposeCount { get; private set; }

            public void Init() { }
            public void Update() { }
            public void LateUpdate() { }
            public void FixedUpdate() { }

            public void Dispose()
            {
                DisposeCount++;
            }
        }
    }
}
