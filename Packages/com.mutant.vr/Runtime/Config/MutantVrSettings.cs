using UnityEngine;

namespace Mutant.VR.Config
{
    [CreateAssetMenu(menuName = "Mutant/VR/MutantVrSettings", fileName = "MutantVrSettings")]
    public sealed class MutantVrSettings : ScriptableObject
    {
        [Header("Lifecycle")]
        [SerializeField] private bool _autoInstallOnAwake = true;
        [SerializeField] private bool _autoTickInUpdate = true;
        [SerializeField] private bool _autoShutdownOnDestroy = true;

        [Header("Pointer")]
        [SerializeField] private float _defaultPointerLength = 10.0f;
        [SerializeField] private LayerMask _defaultPointerMask = ~0;
        [SerializeField] private QueryTriggerInteraction _defaultPointerQueryTrigger = QueryTriggerInteraction.Ignore;

        [Header("Diagnostics")]
        [SerializeField] private bool _enableVerboseLogging = true;
        [SerializeField] private bool _drawDebugPointers = true;

        public bool AutoInstallOnAwake => _autoInstallOnAwake;
        public bool AutoTickInUpdate => _autoTickInUpdate;
        public bool AutoShutdownOnDestroy => _autoShutdownOnDestroy;

        public float DefaultPointerLength => _defaultPointerLength;
        public LayerMask DefaultPointerMask => _defaultPointerMask;
        public QueryTriggerInteraction DefaultPointerQueryTrigger => _defaultPointerQueryTrigger;

        public bool EnableVerboseLogging => _enableVerboseLogging;
        public bool DrawDebugPointers => _drawDebugPointers;
    }
}