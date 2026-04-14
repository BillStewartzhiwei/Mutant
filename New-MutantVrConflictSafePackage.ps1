param(
    [string]$RootPath = (Get-Location).Path
)

$ErrorActionPreference = "Stop"

function Ensure-Directory {
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Write-Utf8BomFile {
    param(
        [string]$Path,
        [string]$Content
    )
    $parent = Split-Path -Path $Path -Parent
    Ensure-Directory -Path $parent
    $utf8Bom = [System.Text.UTF8Encoding]::new($true)
    [System.IO.File]::WriteAllText($Path, $Content, $utf8Bom)
}

$packageRoot = Join-Path $RootPath "Packages/com.mutant.vr"

# Remove old package to avoid stale conflict-prone files
if (Test-Path -LiteralPath $packageRoot) { Remove-Item -Recurse -Force $packageRoot }

Ensure-Directory -Path (Join-Path $packageRoot "Documentation~")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Bootstrap")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Config")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Contracts")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Controls")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Core")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/CoreBridge")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Diagnostics")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Integrations/VRIF")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Pointers")
Ensure-Directory -Path (Join-Path $packageRoot "Runtime/Rig")
Ensure-Directory -Path (Join-Path $packageRoot "Samples~/CoreIntegrationExample")

Write-Utf8BomFile -Path (Join-Path $packageRoot "package.json") -Content @'
{
  "name": "com.mutant.vr",
  "displayName": "Mutant VR",
  "version": "0.2.0",
  "unity": "2022.3",
  "description": "Conflict-safe first runnable skeleton for Mutant VR with a Core integration example.",
  "keywords": [
    "mutant",
    "vr",
    "xr"
  ],
  "author": {
    "name": "Bill Stewart"
  },
  "dependencies": {}
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "README.md") -Content @'
# com.mutant.vr

一版可运行的 Mutant VR 骨架包，重点处理两件事：

1. 避开容易和 Unity / XR / 引擎 API 冲突的命名
2. 给出一个可以接入 Mutant Core 生命周期的最小示例

## 这版做了什么

- 不再使用高冲突命名段作为核心运行时代码入口：
  - `Input` -> `Controls`
  - `Ray` -> `Pointers`
- 统一用 `UnityEngine.Ray` 显式类型名
- 提供：
  - `MutantVrModule`
  - `MutantVrInstaller`
  - `MutantVrContext`
  - `MutantVrVrifPlatformAdapter`
  - `MutantVrCoreInstallExample`

## 当前运行方式

- 场景里放一个空物体
- 挂：
  - `MutantVrInstaller`
  - `MutantVrVrifPlatformAdapter`
- 有 `MutantVrRigRoot` 就拖进去
- 没有 Rig 时，Head 会优先回退到 `Camera.main`

## Core 接入示例

`Runtime/CoreBridge/MutantVrCoreInstallExample.cs`

这个脚本不依赖你当前 Core 的具体 API，只负责给你一个最小接线点：
- Core 启动时调用 `InstallFromCore()`
- Core Tick 时调用 `TickFromCore(deltaTime)`
- Core 结束时调用 `ShutdownFromCore()`

你可以把它接到你现有的启动编排里。
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "CHANGELOG.md") -Content @'
# Changelog

## 0.2.0
- Added conflict-safe runtime skeleton
- Renamed runtime domains to avoid common engine naming conflicts
- Added Core integration example
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/com.mutant.vr.asmdef") -Content @'
{
  "name": "com.mutant.vr",
  "rootNamespace": "Mutant.VR",
  "references": [],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": true,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Core/MutantVrLifecycleState.cs") -Content @'
namespace Mutant.VR.Core
{
    public enum MutantVrLifecycleState
    {
        None = 0,
        Configured = 1,
        Installing = 2,
        Ready = 3,
        Running = 4,
        Failed = 5,
        Stopped = 6
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Core/MutantVrServiceRegistry.cs") -Content @'
using System;
using System.Collections.Generic;

namespace Mutant.VR.Core
{
    public sealed class MutantVrServiceRegistry
    {
        private readonly Dictionary<Type, object> _serviceMap = new Dictionary<Type, object>();

        public void Register<TService>(TService service)
            where TService : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            _serviceMap[typeof(TService)] = service;
        }

        public bool TryGet<TService>(out TService service)
            where TService : class
        {
            if (_serviceMap.TryGetValue(typeof(TService), out object boxed))
            {
                service = boxed as TService;
                return service != null;
            }

            service = null;
            return false;
        }

        public bool Contains<TService>()
            where TService : class
        {
            return _serviceMap.ContainsKey(typeof(TService));
        }

        public void Clear()
        {
            _serviceMap.Clear();
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Config/MutantVrSettings.cs") -Content @'
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
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Controls/MutantVrControllerHand.cs") -Content @'
namespace Mutant.VR.Controls
{
    public enum MutantVrControllerHand
    {
        None = 0,
        Left = 1,
        Right = 2
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Controls/MutantVrControlSnapshot.cs") -Content @'
using System;
using UnityEngine;

namespace Mutant.VR.Controls
{
    [Serializable]
    public struct MutantVrControlSnapshot
    {
        public int FrameIndex;
        public float RealtimeSinceStartup;

        public bool LeftTriggerPressed;
        public bool RightTriggerPressed;

        public bool LeftGripPressed;
        public bool RightGripPressed;

        public bool LeftPrimaryPressed;
        public bool RightPrimaryPressed;

        public Vector2 LeftStickAxis;
        public Vector2 RightStickAxis;
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Rig/MutantVrRigReferences.cs") -Content @'
using System;
using UnityEngine;

namespace Mutant.VR.Rig
{
    [Serializable]
    public sealed class MutantVrRigReferences
    {
        public Transform HeadAnchorTransform;
        public Transform LeftHandAnchorTransform;
        public Transform RightHandAnchorTransform;
        public Transform LeftPointerOriginTransform;
        public Transform RightPointerOriginTransform;

        public MutantVrRigReferences CreateCopy()
        {
            return new MutantVrRigReferences
            {
                HeadAnchorTransform = HeadAnchorTransform,
                LeftHandAnchorTransform = LeftHandAnchorTransform,
                RightHandAnchorTransform = RightHandAnchorTransform,
                LeftPointerOriginTransform = LeftPointerOriginTransform,
                RightPointerOriginTransform = RightPointerOriginTransform
            };
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Rig/MutantVrRigRoot.cs") -Content @'
using UnityEngine;

namespace Mutant.VR.Rig
{
    [DisallowMultipleComponent]
    public sealed class MutantVrRigRoot : MonoBehaviour
    {
        [SerializeField] private Transform _headAnchor;
        [SerializeField] private Transform _leftHandAnchor;
        [SerializeField] private Transform _rightHandAnchor;
        [SerializeField] private Transform _leftPointerOrigin;
        [SerializeField] private Transform _rightPointerOrigin;

        public MutantVrRigReferences BuildRigReferences()
        {
            return new MutantVrRigReferences
            {
                HeadAnchorTransform = _headAnchor,
                LeftHandAnchorTransform = _leftHandAnchor,
                RightHandAnchorTransform = _rightHandAnchor,
                LeftPointerOriginTransform = _leftPointerOrigin != null ? _leftPointerOrigin : _leftHandAnchor,
                RightPointerOriginTransform = _rightPointerOrigin != null ? _rightPointerOrigin : _rightHandAnchor
            };
        }

        private void Reset()
        {
            if (_headAnchor == null && Camera.main != null)
            {
                _headAnchor = Camera.main.transform;
            }
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Contracts/IMutantVrPlatformAdapter.cs") -Content @'
using Mutant.VR.Core;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrPlatformAdapter
    {
        string AdapterKey { get; }
        bool CanInstall { get; }
        bool IsInstalled { get; }

        void Install(MutantVrContext context);
        void Tick(float deltaTime);
        void Shutdown();
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Contracts/IMutantVrRigService.cs") -Content @'
using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrRigService
    {
        void RefreshRig();

        bool TryGetHeadTransform(out Transform headTransform);
        bool TryGetHandTransform(MutantVrControllerHand hand, out Transform handTransform);
        bool TryGetPointerOriginTransform(MutantVrControllerHand hand, out Transform pointerOriginTransform);
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Contracts/IMutantVrControlService.cs") -Content @'
using Mutant.VR.Controls;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrControlService
    {
        MutantVrControlSnapshot CurrentSnapshot { get; }

        void UpdateControls(float deltaTime);
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Contracts/IMutantVrPointerService.cs") -Content @'
using Mutant.VR.Controls;

namespace Mutant.VR.Contracts
{
    public interface IMutantVrPointerService
    {
        bool TryBuildPointer(MutantVrControllerHand hand, out UnityEngine.Ray pointerRay);
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Core/MutantVrContext.cs") -Content @'
using Mutant.VR.Config;
using Mutant.VR.Contracts;
using Mutant.VR.Rig;
using UnityEngine;

namespace Mutant.VR.Core
{
    public sealed class MutantVrContext
    {
        public MutantVrSettings Settings { get; private set; }
        public MutantVrRigReferences RigReferences { get; private set; } = new MutantVrRigReferences();
        public IMutantVrPlatformAdapter PlatformAdapter { get; private set; }
        public MutantVrServiceRegistry ServiceRegistry { get; } = new MutantVrServiceRegistry();
        public MutantVrLifecycleState State { get; private set; } = MutantVrLifecycleState.None;

        public void Configure(
            MutantVrSettings settings,
            MutantVrRigReferences rigReferences,
            IMutantVrPlatformAdapter platformAdapter)
        {
            Settings = settings;
            RigReferences = rigReferences != null ? rigReferences.CreateCopy() : new MutantVrRigReferences();
            PlatformAdapter = platformAdapter;
            State = MutantVrLifecycleState.Configured;
        }

        public void ReplaceRigReferences(MutantVrRigReferences rigReferences)
        {
            RigReferences = rigReferences != null ? rigReferences.CreateCopy() : new MutantVrRigReferences();
        }

        public void SetState(MutantVrLifecycleState state)
        {
            State = state;
        }

        public void RegisterService<TService>(TService service)
            where TService : class
        {
            ServiceRegistry.Register(service);
        }

        public bool TryGetService<TService>(out TService service)
            where TService : class
        {
            return ServiceRegistry.TryGet(out service);
        }

        public void LogVerbose(string message)
        {
            if (Settings != null && Settings.EnableVerboseLogging)
            {
                Debug.Log($"[MutantVr] {message}");
            }
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning($"[MutantVr] {message}");
        }

        public void LogError(string message)
        {
            Debug.LogError($"[MutantVr] {message}");
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Bootstrap/MutantVrModule.cs") -Content @'
using System;
using Mutant.VR.Contracts;
using Mutant.VR.Core;

namespace Mutant.VR.Bootstrap
{
    public sealed class MutantVrModule
    {
        public MutantVrContext Context { get; }

        public MutantVrModule(MutantVrContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool Install()
        {
            IMutantVrPlatformAdapter platformAdapter = Context.PlatformAdapter;
            if (platformAdapter == null)
            {
                Context.SetState(MutantVrLifecycleState.Failed);
                Context.LogError("Install failed: PlatformAdapter is null.");
                return false;
            }

            if (!platformAdapter.CanInstall)
            {
                Context.SetState(MutantVrLifecycleState.Failed);
                Context.LogError($"Install failed: Adapter '{platformAdapter.AdapterKey}' is not ready.");
                return false;
            }

            if (Context.State == MutantVrLifecycleState.Ready || Context.State == MutantVrLifecycleState.Running)
            {
                return true;
            }

            try
            {
                Context.SetState(MutantVrLifecycleState.Installing);
                platformAdapter.Install(Context);

                if (!platformAdapter.IsInstalled)
                {
                    Context.SetState(MutantVrLifecycleState.Failed);
                    Context.LogError($"Install failed: Adapter '{platformAdapter.AdapterKey}' did not enter installed state.");
                    return false;
                }

                Context.SetState(MutantVrLifecycleState.Ready);
                Context.LogVerbose($"Installed with adapter '{platformAdapter.AdapterKey}'.");
                return true;
            }
            catch (Exception exception)
            {
                Context.SetState(MutantVrLifecycleState.Failed);
                Context.LogError($"Install exception: {exception}");
                return false;
            }
        }

        public void Tick(float deltaTime)
        {
            if (Context.State == MutantVrLifecycleState.Failed || Context.State == MutantVrLifecycleState.Stopped)
            {
                return;
            }

            if (Context.State == MutantVrLifecycleState.Configured && !Install())
            {
                return;
            }

            IMutantVrPlatformAdapter platformAdapter = Context.PlatformAdapter;
            if (platformAdapter == null || !platformAdapter.IsInstalled)
            {
                return;
            }

            platformAdapter.Tick(deltaTime);
            Context.SetState(MutantVrLifecycleState.Running);
        }

        public void Shutdown()
        {
            IMutantVrPlatformAdapter platformAdapter = Context.PlatformAdapter;
            if (platformAdapter != null && platformAdapter.IsInstalled)
            {
                platformAdapter.Shutdown();
            }

            Context.ServiceRegistry.Clear();
            Context.SetState(MutantVrLifecycleState.Stopped);
            Context.LogVerbose("Shutdown completed.");
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Bootstrap/MutantVrInstaller.cs") -Content @'
using Mutant.VR.Config;
using Mutant.VR.Contracts;
using Mutant.VR.Core;
using Mutant.VR.Rig;
using UnityEngine;

namespace Mutant.VR.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class MutantVrInstaller : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private MutantVrSettings _settings;

        [Header("Scene References")]
        [SerializeField] private MutantVrRigRoot _rigRoot;
        [SerializeField] private MonoBehaviour _platformAdapterBehaviour;

        private MutantVrContext _runtimeContext;
        private MutantVrModule _runtimeModule;

        public MutantVrContext RuntimeContext => _runtimeContext;
        public MutantVrModule RuntimeModule => _runtimeModule;

        private void Awake()
        {
            BuildRuntime();

            bool shouldAutoInstall = _settings == null || _settings.AutoInstallOnAwake;
            if (shouldAutoInstall)
            {
                InstallRuntime();
            }
        }

        private void Update()
        {
            bool shouldAutoTick = _settings == null || _settings.AutoTickInUpdate;
            if (!shouldAutoTick)
            {
                return;
            }

            _runtimeModule?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            bool shouldAutoShutdown = _settings == null || _settings.AutoShutdownOnDestroy;
            if (shouldAutoShutdown)
            {
                _runtimeModule?.Shutdown();
            }
        }

        [ContextMenu("Mutant/VR/Install Runtime")]
        public void InstallRuntime()
        {
            if (_runtimeModule == null)
            {
                BuildRuntime();
            }

            _runtimeModule?.Install();
        }

        [ContextMenu("Mutant/VR/Tick Runtime Once")]
        public void TickRuntimeOnce()
        {
            _runtimeModule?.Tick(Time.deltaTime);
        }

        [ContextMenu("Mutant/VR/Shutdown Runtime")]
        public void ShutdownRuntime()
        {
            _runtimeModule?.Shutdown();
        }

        private void BuildRuntime()
        {
            IMutantVrPlatformAdapter adapter = ResolvePlatformAdapter();
            MutantVrRigReferences rigReferences = _rigRoot != null
                ? _rigRoot.BuildRigReferences()
                : new MutantVrRigReferences();

            _runtimeContext = new MutantVrContext();
            _runtimeContext.Configure(_settings, rigReferences, adapter);

            _runtimeModule = new MutantVrModule(_runtimeContext);
        }

        private IMutantVrPlatformAdapter ResolvePlatformAdapter()
        {
            if (_platformAdapterBehaviour is IMutantVrPlatformAdapter serializedAdapter)
            {
                return serializedAdapter;
            }

            MonoBehaviour[] localBehaviours = GetComponents<MonoBehaviour>();
            for (int i = 0; i < localBehaviours.Length; i++)
            {
                if (localBehaviours[i] is IMutantVrPlatformAdapter foundAdapter)
                {
                    _platformAdapterBehaviour = localBehaviours[i];
                    return foundAdapter;
                }
            }

            MonoBehaviour[] childBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = 0; i < childBehaviours.Length; i++)
            {
                if (childBehaviours[i] is IMutantVrPlatformAdapter foundAdapter)
                {
                    _platformAdapterBehaviour = childBehaviours[i];
                    return foundAdapter;
                }
            }

            Debug.LogWarning("[MutantVrInstaller] No IMutantVrPlatformAdapter found.");
            return null;
        }

        private void Reset()
        {
            if (_rigRoot == null)
            {
                _rigRoot = GetComponentInChildren<MutantVrRigRoot>(true);
            }
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Pointers/MutantVrPointerCastResult.cs") -Content @'
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
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Pointers/MutantVrPointerInteractor.cs") -Content @'
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
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Pointers/MutantVrPointerRenderer.cs") -Content @'
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
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Diagnostics/MutantVrRuntimeStatusView.cs") -Content @'
using Mutant.VR.Bootstrap;
using Mutant.VR.Contracts;
using Mutant.VR.Core;
using UnityEngine;

namespace Mutant.VR.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class MutantVrRuntimeStatusView : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;
        [SerializeField] private bool _showOnGui = true;

        private void Awake()
        {
            if (_installer == null)
            {
                _installer = GetComponentInParent<MutantVrInstaller>(true);
            }
        }

        private void OnGUI()
        {
            if (!_showOnGui || !Application.isPlaying)
            {
                return;
            }

            MutantVrContext context = _installer != null ? _installer.RuntimeContext : null;

            GUILayout.BeginArea(new Rect(16f, 16f, 340f, 150f), GUI.skin.box);
            GUILayout.Label("Mutant VR Runtime");

            if (context == null)
            {
                GUILayout.Label("Context: null");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label($"State: {context.State}");

            IMutantVrPlatformAdapter adapter = context.PlatformAdapter;
            GUILayout.Label($"Adapter: {(adapter != null ? adapter.AdapterKey : "null")}");
            GUILayout.Label($"Adapter Installed: {(adapter != null && adapter.IsInstalled)}");
            GUILayout.Label($"Rig Service: {context.ServiceRegistry.Contains<IMutantVrRigService>()}");
            GUILayout.Label($"Control Service: {context.ServiceRegistry.Contains<IMutantVrControlService>()}");
            GUILayout.Label($"Pointer Service: {context.ServiceRegistry.Contains<IMutantVrPointerService>()}");

            GUILayout.EndArea();
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Diagnostics/MutantVrRigStatusView.cs") -Content @'
using Mutant.VR.Bootstrap;
using Mutant.VR.Contracts;
using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class MutantVrRigStatusView : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;
        [SerializeField] private float _axisLength = 0.15f;
        [SerializeField] private float _forwardLength = 0.35f;

        private IMutantVrRigService _rigService;

        private void Awake()
        {
            if (_installer == null)
            {
                _installer = GetComponentInParent<MutantVrInstaller>(true);
            }
        }

        private void Update()
        {
            if (!TryResolveRigService())
            {
                return;
            }

            if (_rigService.TryGetHeadTransform(out Transform headTransform))
            {
                DrawBasis(headTransform, Color.white);
            }

            if (_rigService.TryGetHandTransform(MutantVrControllerHand.Left, out Transform leftHandTransform))
            {
                DrawBasis(leftHandTransform, Color.green);
            }

            if (_rigService.TryGetHandTransform(MutantVrControllerHand.Right, out Transform rightHandTransform))
            {
                DrawBasis(rightHandTransform, Color.cyan);
            }
        }

        private bool TryResolveRigService()
        {
            if (_rigService != null)
            {
                return true;
            }

            if (_installer == null || _installer.RuntimeContext == null)
            {
                return false;
            }

            return _installer.RuntimeContext.TryGetService(out _rigService);
        }

        private void DrawBasis(Transform targetTransform, Color forwardColor)
        {
            Vector3 position = targetTransform.position;
            Debug.DrawRay(position, targetTransform.forward * _forwardLength, forwardColor);
            Debug.DrawRay(position, targetTransform.up * _axisLength, Color.yellow);
            Debug.DrawRay(position, targetTransform.right * _axisLength, Color.red);
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Diagnostics/MutantVrControlStatusView.cs") -Content @'
using Mutant.VR.Bootstrap;
using Mutant.VR.Contracts;
using Mutant.VR.Controls;
using UnityEngine;

namespace Mutant.VR.Diagnostics
{
    [DisallowMultipleComponent]
    public sealed class MutantVrControlStatusView : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;
        [SerializeField] private bool _showOnGui = true;

        private IMutantVrControlService _controlService;

        private void Awake()
        {
            if (_installer == null)
            {
                _installer = GetComponentInParent<MutantVrInstaller>(true);
            }
        }

        private void OnGUI()
        {
            if (!_showOnGui || !Application.isPlaying || !TryResolveControlService())
            {
                return;
            }

            MutantVrControlSnapshot snapshot = _controlService.CurrentSnapshot;

            GUILayout.BeginArea(new Rect(16f, 176f, 320f, 180f), GUI.skin.box);
            GUILayout.Label("Mutant VR Controls");
            GUILayout.Label($"Frame: {snapshot.FrameIndex}");
            GUILayout.Label($"Realtime: {snapshot.RealtimeSinceStartup:F3}");
            GUILayout.Label($"Left Trigger: {snapshot.LeftTriggerPressed}");
            GUILayout.Label($"Right Trigger: {snapshot.RightTriggerPressed}");
            GUILayout.Label($"Left Grip: {snapshot.LeftGripPressed}");
            GUILayout.Label($"Right Grip: {snapshot.RightGripPressed}");
            GUILayout.Label($"Left Primary: {snapshot.LeftPrimaryPressed}");
            GUILayout.Label($"Right Primary: {snapshot.RightPrimaryPressed}");
            GUILayout.Label($"Left Stick: {snapshot.LeftStickAxis}");
            GUILayout.Label($"Right Stick: {snapshot.RightStickAxis}");
            GUILayout.EndArea();
        }

        private bool TryResolveControlService()
        {
            if (_controlService != null)
            {
                return true;
            }

            if (_installer == null || _installer.RuntimeContext == null)
            {
                return false;
            }

            return _installer.RuntimeContext.TryGetService(out _controlService);
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/CoreBridge/MutantVrCoreInstallExample.cs") -Content @'
using Mutant.VR.Bootstrap;
using UnityEngine;

namespace Mutant.VR.CoreBridge
{
    [DisallowMultipleComponent]
    public sealed class MutantVrCoreInstallExample : MonoBehaviour
    {
        [SerializeField] private MutantVrInstaller _installer;

        public void InstallFromCore()
        {
            _installer?.InstallRuntime();
        }

        public void TickFromCore(float deltaTime)
        {
            if (_installer != null && _installer.RuntimeModule != null)
            {
                _installer.RuntimeModule.Tick(deltaTime);
            }
        }

        public void ShutdownFromCore()
        {
            _installer?.ShutdownRuntime();
        }

        private void Reset()
        {
            if (_installer == null)
            {
                _installer = GetComponent<MutantVrInstaller>();
            }
        }
    }
}
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Runtime/Integrations/VRIF/MutantVrVrifPlatformAdapter.cs") -Content @'
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
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Documentation~/mutant-vr-naming-notes.md") -Content @'
# Mutant VR Naming Notes

为了避免和引擎命名冲突，这一版做了下面的替换：

- `Input` -> `Controls`
- `Ray` -> `Pointers`

并且所有射线类型都显式使用 `UnityEngine.Ray`。
'@

Write-Utf8BomFile -Path (Join-Path $packageRoot "Samples~/CoreIntegrationExample/README.md") -Content @'
# CoreIntegrationExample

这个目录给的是最小接线说明：

- `MutantVrInstaller`
- `MutantVrVrifPlatformAdapter`
- `MutantVrCoreInstallExample`

你现有的 Core 生命周期可以这样接：

1. Core Init -> `InstallFromCore()`
2. Core Loop/Update -> `TickFromCore(deltaTime)`
3. Core Dispose -> `ShutdownFromCore()`
'@

Write-Host "Mutant VR package written to:" -ForegroundColor Green
Write-Host $packageRoot -ForegroundColor Cyan
Write-Host ""
Get-ChildItem -Path $packageRoot -Recurse -File | Select-Object FullName