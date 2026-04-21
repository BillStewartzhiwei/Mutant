using UnityEngine;

namespace Mutant.VR.Pointers
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public sealed class MutantVrPointerRenderer : MonoBehaviour
    {
        [SerializeField] private MutantVrPointerInteractor _pointerInteractor;
        [SerializeField] private float _idleLength = 8.0f;
        [SerializeField] private float _lineWidth = 0.01f;
        [SerializeField] private bool _useHitPointWhenAvailable = true;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            if (_pointerInteractor == null)
            {
                _pointerInteractor = GetComponent<MutantVrPointerInteractor>();
            }

            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;
        }

        private void LateUpdate()
        {
            if (_lineRenderer == null || _pointerInteractor == null)
            {
                return;
            }

            if (!_pointerInteractor.TryGetCurrentPointer(out UnityEngine.Ray pointerRay))
            {
                return;
            }

            MutantVrPointerCastResult result = _pointerInteractor.LastResult;

            Vector3 startPoint = pointerRay.origin;
            Vector3 endPoint = startPoint + pointerRay.direction * _idleLength;

            if (_useHitPointWhenAvailable && result.HasHit)
            {
                endPoint = result.HitPoint;
            }

            _lineRenderer.SetPosition(0, startPoint);
            _lineRenderer.SetPosition(1, endPoint);
        }
    }
}