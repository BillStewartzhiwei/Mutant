using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Pointers
{
    [System.Serializable]
    public struct MutantVrPointerCastResult
    {
        public int FrameIndex;
        public MutantVrControllerHand Hand;

        public bool HasPointer;
        public UnityEngine.Ray PointerRay;
        public float MaxDistance;

        public bool HasHit;
        public float HitDistance;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public Collider HitCollider;
        public Transform HitTransform;
    }
}
