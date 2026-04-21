using NUnit.Framework;
using Mutant.Core.Events;
using UnityEngine.Assertions;

namespace Mutant.Core.Tests.Editor
{
	public class EventBusEditorTests
	{
		[SetUp]
		public void SetUp()
		{
			EventBus.ClearAll();
			StickyEventBus.ClearAll();
		}

		[Test]
		public void SubscribeOnce_OnlyInvokesOnce()
		{
			int count = 0;

			EventBus.SubscribeOnce<TestEvent>(_ => count++);
			EventBus.Publish(new TestEvent());
			EventBus.Publish(new TestEvent());

			Assert.AreEqual(1, count);
		}

		[Test]
		public void StickyEvent_ReplaysLatestOnSubscribe()
		{
			StickyEventBus.PublishSticky(new StickyValue { Value = 10 });

			int received = 0;
			StickyEventBus.Subscribe<StickyValue>(x => received = x.Value, true);

			Assert.AreEqual(10, received);
		}

		private struct TestEvent { }

		private struct StickyValue
		{
			public int Value;
		}
	}
}
