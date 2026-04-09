using UnityEngine;
using Mutant.Core.Modules;
using Mutant.Core.Events;

public class CoreTestEntry : MonoBehaviour
{
	private void Start()
	{
		ModuleManager.Instance.Register(new DummyModule());

		EventBus.Subscribe<TestEvent>(OnTestEvent);
		EventBus.Publish(new TestEvent { Message = "Hello Mutant Core" });
	}

	private void OnDestroy()
	{
		EventBus.Unsubscribe<TestEvent>(OnTestEvent);
	}

	private void OnTestEvent(TestEvent evt)
	{
		Debug.Log("[CoreTestEntry] Event Received: " + evt.Message);
	}

	private sealed class DummyModule : IModule
	{
		public string Name
		{
			get;
		}
		public int Priority => 0;
		public bool IsInitialized
		{
			get;
		}

		public void Init()
		{
			Debug.Log("[DummyModule] Init");
		}

		public void Update() { }

		public void LateUpdate() { }

		public void FixedUpdate() { }

		public void Dispose()
		{
			Debug.Log("[DummyModule] Dispose");
		}
	}

	private struct TestEvent
	{
		public string Message;
	}
}
