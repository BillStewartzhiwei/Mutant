using Mutant.VR.Config;
using Mutant.VR.Contracts;
using Mutant.VR.Controls;
using Mutant.VR.Core;
using Mutant.VR.Rig;
using UnityEngine;

namespace Mutant.VR.Integrations.VRIF
{
    [DisallowMultipleComponent]
    public sealed class MutantVrVrifPlatformAdapter : MonoBehaviour, IMutantVrPlatformAdapter
    {
        [Header("Optional Rig Root")]
        [SerializeField] private MutantVrRigRoot _rigRoot;

        [Header("Optional Direct References")]
        [SerializeField] private Transform _headAnchor;
        [SerializeField] private Transform _leftHandAnchor;
        [SerializeField] private Transform _rightHandAnchor;
        [SerializeField] private Transform _leftPointerOrigin;
        [SerializeField] private Transform _rightPointerOrigin;

        [Header("Fallback")]
        [SerializeField] private bool _allowCameraMainFallback = true;

        private MutantVrContext _context;
        private VrifRigService _rigService;
        private VrifControlService _controlService;
        private VrifPointerService _pointerService;

        public string AdapterKey => "VRIF";
        public bool CanInstall => true;
        public bool IsInstalled { get; private set; }

        public void Install(MutantVrContext context)
        {
            if (context == null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            if (IsInstalled)
            {
                return;
            }

            _context = context;

            MutantVrRigReferences resolvedRig = BuildResolvedRigReferences(context.RigReferences);
            _context.ReplaceRigReferences(resolvedRig);

            _rigService = new VrifRigService(resolvedRig, _allowCameraMainFallback);
            _controlService = new VrifControlService();
            _pointerService = new VrifPointerService(_rigService, ResolvePointerLength(context.Settings));

            _context.RegisterService<IMutantVrRigService>(_rigService);
            _context.RegisterService<IMutantVrControlService>(_controlService);
            _context.RegisterService<IMutantVrPointerService>(_pointerService);

            IsInstalled = true;
            _context.LogVerbose("VRIF adapter installed.");
        }

        public void Tick(float deltaTime)
        {
            if (!IsInstalled)
            {
                return;
            }

            _rigService.RefreshRig();
            _controlService.UpdateControls(deltaTime);

            if (_context != null && _context.Settings != null && _context.Settings.DrawDebugPointers)
            {
                DrawDebugPointer(MutantVrControllerHand.Left, Color.green);
                DrawDebugPointer(MutantVrControllerHand.Right, Color.cyan);
            }
        }

        public void Shutdown()
        {
            IsInstalled = false;
            _rigService = null;
            _controlService = null;
            _pointerService = null;
            _context = null;
        }

        private MutantVrRigReferences BuildResolvedRigReferences(MutantVrRigReferences fallback)
        {
            MutantVrRigReferences fromRigRoot = _rigRoot != null ? _rigRoot.BuildRigReferences() : null;

            return new MutantVrRigReferences
            {
                HeadAnchorTransform = FirstNonNull(_headAnchor, fromRigRoot?.HeadAnchorTransform, fallback?.HeadAnchorTransform, Camera.main != null ? Camera.main.transform : null),
                LeftHandAnchorTransform = FirstNonNull(_leftHandAnchor, fromRigRoot?.LeftHandAnchorTransform, fallback?.LeftHandAnchorTransform),
                RightHandAnchorTransform = FirstNonNull(_rightHandAnchor, fromRigRoot?.RightHandAnchorTransform, fallback?.RightHandAnchorTransform),
                LeftPointerOriginTransform = FirstNonNull(_leftPointerOrigin, fromRigRoot?.LeftPointerOriginTransform, fallback?.LeftPointerOriginTransform, _leftHandAnchor, fromRigRoot?.LeftHandAnchorTransform, fallback?.LeftHandAnchorTransform),
                RightPointerOriginTransform = FirstNonNull(_rightPointerOrigin, fromRigRoot?.RightPointerOriginTransform, fallback?.RightPointerOriginTransform, _rightHandAnchor, fromRigRoot?.RightHandAnchorTransform, fallback?.RightHandAnchorTransform)
            };
        }

        private float ResolvePointerLength(MutantVrSettings settings)
        {
            if (settings != null)
            {
                return Mathf.Max(0.1f, settings.DefaultPointerLength);
            }

            return 10.0f;
        }

        private void DrawDebugPointer(MutantVrControllerHand hand, Color color)
        {
            if (_pointerService != null && _pointerService.TryBuildPointer(hand, out UnityEngine.Ray pointerRay))
            {
                Debug.DrawRay(pointerRay.origin, pointerRay.direction * ResolvePointerLength(_context?.Settings), color);
            }
        }

        private static Transform FirstNonNull(params Transform[] transforms)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != null)
                {
                    return transforms[i];
                }
            }

            return null;
        }

        private void Reset()
        {
            if (_rigRoot == null)
            {
                _rigRoot = GetComponentInChildren<MutantVrRigRoot>(true);
            }

            if (_headAnchor == null && Camera.main != null)
            {
                _headAnchor = Camera.main.transform;
            }
        }

        private sealed class VrifRigService : IMutantVrRigService
        {
            private readonly MutantVrRigReferences _rigReferences;
            private readonly bool _allowCameraMainFallback;

            public VrifRigService(MutantVrRigReferences rigReferences, bool allowCameraMainFallback)
            {
                _rigReferences = rigReferences ?? new MutantVrRigReferences();
                _allowCameraMainFallback = allowCameraMainFallback;
            }

            public void RefreshRig()
            {
                if (_rigReferences.HeadAnchorTransform == null && _allowCameraMainFallback && Camera.main != null)
                {
                    _rigReferences.HeadAnchorTransform = Camera.main.transform;
                }
            }

            public bool TryGetHeadTransform(out Transform headTransform)
            {
                headTransform = _rigReferences.HeadAnchorTransform;
                if (headTransform == null && _allowCameraMainFallback && Camera.main != null)
                {
                    headTransform = Camera.main.transform;
                }

                return headTransform != null;
            }

            public bool TryGetHandTransform(MutantVrControllerHand hand, out Transform handTransform)
            {
                switch (hand)
                {
                    case MutantVrControllerHand.Left:
                        handTransform = _rigReferences.LeftHandAnchorTransform;
                        return handTransform != null;

                    case MutantVrControllerHand.Right:
                        handTransform = _rigReferences.RightHandAnchorTransform;
                        return handTransform != null;

                    default:
                        handTransform = null;
                        return false;
                }
            }

            public bool TryGetPointerOriginTransform(MutantVrControllerHand hand, out Transform pointerOriginTransform)
            {
                switch (hand)
                {
                    case MutantVrControllerHand.Left:
                        pointerOriginTransform = _rigReferences.LeftPointerOriginTransform != null
                            ? _rigReferences.LeftPointerOriginTransform
                            : _rigReferences.LeftHandAnchorTransform;
                        return pointerOriginTransform != null;

                    case MutantVrControllerHand.Right:
                        pointerOriginTransform = _rigReferences.RightPointerOriginTransform != null
                            ? _rigReferences.RightPointerOriginTransform
                            : _rigReferences.RightHandAnchorTransform;
                        return pointerOriginTransform != null;

                    default:
                        pointerOriginTransform = null;
                        return false;
                }
            }
        }

        private sealed class VrifControlService : IMutantVrControlService
        {
            public MutantVrControlSnapshot CurrentSnapshot { get; private set; }

            public void UpdateControls(float deltaTime)
            {
                MutantVrControlSnapshot nextSnapshot = CurrentSnapshot;
                nextSnapshot.FrameIndex = Time.frameCount;
                nextSnapshot.RealtimeSinceStartup = Time.realtimeSinceStartup;

#if ENABLE_LEGACY_INPUT_MANAGER
                nextSnapshot.LeftTriggerPressed = UnityEngine.Input.GetKey(KeyCode.Q);
                nextSnapshot.RightTriggerPressed = UnityEngine.Input.GetMouseButton(0);

                nextSnapshot.LeftGripPressed = UnityEngine.Input.GetKey(KeyCode.E);
                nextSnapshot.RightGripPressed = UnityEngine.Input.GetMouseButton(1);

                nextSnapshot.LeftPrimaryPressed = UnityEngine.Input.GetKey(KeyCode.Alpha1);
                nextSnapshot.RightPrimaryPressed = UnityEngine.Input.GetKey(KeyCode.Alpha2);

                float leftHorizontal = (UnityEngine.Input.GetKey(KeyCode.D) ? 1f : 0f) - (UnityEngine.Input.GetKey(KeyCode.A) ? 1f : 0f);
                float leftVertical = (UnityEngine.Input.GetKey(KeyCode.W) ? 1f : 0f) - (UnityEngine.Input.GetKey(KeyCode.S) ? 1f : 0f);

                float rightHorizontal = (UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1f : 0f) - (UnityEngine.Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
                float rightVertical = (UnityEngine.Input.GetKey(KeyCode.UpArrow) ? 1f : 0f) - (UnityEngine.Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);

                nextSnapshot.LeftStickAxis = new Vector2(leftHorizontal, leftVertical);
                nextSnapshot.RightStickAxis = new Vector2(rightHorizontal, rightVertical);
#else
                nextSnapshot.LeftTriggerPressed = false;
                nextSnapshot.RightTriggerPressed = false;
                nextSnapshot.LeftGripPressed = false;
                nextSnapshot.RightGripPressed = false;
                nextSnapshot.LeftPrimaryPressed = false;
                nextSnapshot.RightPrimaryPressed = false;
                nextSnapshot.LeftStickAxis = Vector2.zero;
                nextSnapshot.RightStickAxis = Vector2.zero;
#endif

                CurrentSnapshot = nextSnapshot;
            }
        }

        private sealed class VrifPointerService : IMutantVrPointerService
        {
            private readonly IMutantVrRigService _rigService;
            private readonly float _defaultPointerLength;

            public VrifPointerService(IMutantVrRigService rigService, float defaultPointerLength)
            {
                _rigService = rigService;
                _defaultPointerLength = Mathf.Max(0.1f, defaultPointerLength);
            }

            public bool TryBuildPointer(MutantVrControllerHand hand, out UnityEngine.Ray pointerRay)
            {
                if (_rigService != null && _rigService.TryGetPointerOriginTransform(hand, out Transform pointerOriginTransform))
                {
                    pointerRay = new UnityEngine.Ray(pointerOriginTransform.position, pointerOriginTransform.forward);
                    return true;
                }

                pointerRay = new UnityEngine.Ray(Vector3.zero, Vector3.forward * _defaultPointerLength);
                return false;
            }
        }
    }
}