using System;
using UnityEngine;

namespace Mutant.LSL
{
	public sealed class MutantLslPoseSender : IDisposable
	{
		private readonly MutantLslFloatSender _innerSender;

		public MutantLslPoseSender(MutantLslFloatSender innerSender)
		{
			_innerSender = innerSender ?? throw new MutantLslException("Pose sender inner sender is null.");
		}

		public void SendPose(Vector3 position, Quaternion rotation, double? timestampSeconds = null)
		{
			float[] sample = new float[7]
			{
				position.x,
				position.y,
				position.z,
				rotation.x,
				rotation.y,
				rotation.z,
				rotation.w
			};

			_innerSender.SendFloat(sample, timestampSeconds);
		}

		public void SendPose(Transform poseTransform, double? timestampSeconds = null)
		{
			if (poseTransform == null)
			{
				throw new MutantLslException("Pose transform is null.");
			}

			SendPose(poseTransform.position, poseTransform.rotation, timestampSeconds);
		}

		public void Dispose()
		{
			_innerSender?.Dispose();
		}
	}
}
