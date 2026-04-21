using Mutant.VR.Bootstrap;
using Mutant.VR.Contracts;
using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Pointers
{
    [DisallowMultipleComponent]
    public sealed class MutantVrPointerInteractor : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;
        [SerializeField] private MutantVrControllerHand _hand = MutantVrControllerHand.Right;
        [SerializeField] private LayerMask _pointerMask = ~0;
        [SerializeField] private float _maxDistance = 10.0f;
        [SerializeField] private QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Ignore;
        [SerializeField] private bool _drawDebugPointer = true;

        private IMutantVrPointerService _pointerService;
        private MutantVrPointerCastResult _lastResult;

        public MutantVrPointerCastResult LastResult => _lastResult;

        private void Awake()
        {
            if (_installer == null)
            {
                _installer = GetComponentInParent<MutantVrInstaller>(true);
            }
        }

        private void Update()
        {
            TickPointerCast();
        }

        public bool TickPointerCast()
        {
            MutantVrPointerCastResult nextResult = new MutantVrPointerCastResult
            {
                FrameIndex = Time.frameCount,
                Hand = _hand,
                MaxDistance = ResolveMaxDistance()
            };

            if (!TryResolvePointerService())
            {
                _lastResult = nextResult;
                return false;
            }

            if (!_pointerService.TryBuildPointer(_hand, out UnityEngine.Ray pointerRay))
            {
                _lastResult = nextResult;
                return false;
            }

            nextResult.HasPointer = true;
            nextResult.PointerRay = pointerRay;

            bool hasHit = Physics.Raycast(
                pointerRay,
                out RaycastHit hit,
                nextResult.MaxDistance,
                _pointerMask,
                _queryTriggerInteraction);

            if (hasHit)
            {
                nextResult.HasHit = true;
                nextResult.HitDistance = hit.distance;
                nextResult.HitPoint = hit.point;
                nextResult.HitNormal = hit.normal;
                nextResult.HitCollider = hit.collider;
                nextResult.HitTransform = hit.transform;
            }

            if (_drawDebugPointer)
            {
                Debug.DrawRay(
                    pointerRay.origin,
                    pointerRay.direction * (hasHit ? nextResult.HitDistance : nextResult.MaxDistance),
                    hasHit ? Color.green : Color.yellow);
            }

            _lastResult = nextResult;
            return hasHit;
        }

        public bool TryGetCurrentPointer(out UnityEngine.Ray pointerRay)
        {
            if (!TryResolvePointerService())
            {
                pointerRay = default;
                return false;
            }

            return _pointerService.TryBuildPointer(_hand, out pointerRay);
        }

        private bool TryResolvePointerService()
        {
            if (_pointerService != null)
            {
                return true;
            }

            if (_installer == null || _installer.RuntimeContext == null)
            {
                return false;
            }

            return _installer.RuntimeContext.TryGetService(out _pointerService);
        }

        private float ResolveMaxDistance()
        {
            if (_maxDistance > 0.0f)
            {
                return _maxDistance;
            }

            if (_installer != null
                && _installer.RuntimeContext != null
                && _installer.RuntimeContext.Settings != null)
            {
                return Mathf.Max(0.1f, _installer.RuntimeContext.Settings.DefaultPointerLength);
            }

            return 10.0f;
        }
    }
}
