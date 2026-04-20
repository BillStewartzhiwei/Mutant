using System.Collections;
using Mutant.Core.Bootstrap;
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
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (CoreBootstrap bootstrap in Object.FindObjectsOfType<CoreBootstrap>())
                Object.Destroy(bootstrap.gameObject);

            yield return null;

            ModuleManager.Instance.DisposeAll();
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
