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

function Write-TextFile {
    param(
        [string]$Path,
        [string]$Content
    )

    $parent = Split-Path -Path $Path -Parent
    Ensure-Directory -Path $parent

    $utf8Bom = [System.Text.UTF8Encoding]::new($true)
    [System.IO.File]::WriteAllText($Path, $Content, $utf8Bom)
}

function Normalize-NamespaceSegment {
    param([string]$Value)

    $v = $Value.Replace("~", "")
    $v = [regex]::Replace($v, "[^A-Za-z0-9_]", "")
    if ([string]::IsNullOrWhiteSpace($v)) {
        return $null
    }

    return $v
}

function Get-CsNamespace {
    param([string]$RelativePath)

    $dir = Split-Path -Path $RelativePath -Parent
    if ([string]::IsNullOrWhiteSpace($dir)) {
        return "Mutant.VR"
    }

    $parts = $dir -split "[\\/]" |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object { Normalize-NamespaceSegment $_ } |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

    return ((@("Mutant", "VR") + $parts) -join ".")
}

function Get-ClassName {
    param([string]$RelativePath)
    return [System.IO.Path]::GetFileNameWithoutExtension($RelativePath)
}

function Get-SpecialKind {
    param(
        [string]$RelativePath,
        [string]$ClassName
    )

    $specialKinds = @{
        "MutantVrModule"                = "plain"
        "MutantVrInstaller"             = "plain"
        "MutantVrRuntimeEntry"          = "plain"
        "MutantVrRuntimeBinder"         = "plain"
        "MutantVrServiceRegistry"       = "plain"
        "MutantVrServiceLocator"        = "static"
        "MutantVrContext"               = "plain"
        "MutantVrState"                 = "plain"
        "MutantVrConstants"             = "static"
        "MutantVrFeatureFlags"          = "flags"
        "MutantVrControllerSide"        = "enum"
        "MutantVrInteractionMask"       = "flags"
        "MutantVrHoverState"            = "enum"
        "MutantVrSelectState"           = "enum"
        "MutantVrUiFocusState"          = "enum"
        "MutantVrTrackingState"         = "enum"
        "MutantVrButtonState"           = "enum"
        "MutantVrAxisState"             = "plain"
        "MutantVrMenuItems"             = "editor-static"
        "MutantVrCreateAssetMenu"       = "editor-static"
        "MutantVrProjectValidator"      = "editor-static"
        "MutantVrDependencyValidator"   = "editor-static"
        "MutantVrSceneValidator"        = "editor-static"
        "MutantVrRigAutoBinder"         = "editor-static"
        "MutantVrVrifAutoSetup"         = "editor-static"
        "MutantVrDefaultAssetsGenerator"= "editor-static"
        "MutantVrDebugSettings"         = "scriptable"
        "MutantVrSettings"              = "scriptable"
        "MutantVrRigProfile"            = "scriptable"
        "MutantVrInputProfile"          = "scriptable"
        "MutantVrRayProfile"            = "scriptable"
        "MutantVrGrabProfile"           = "scriptable"
        "MutantVrHapticProfile"         = "scriptable"
        "MutantVrLocomotionProfile"     = "scriptable"
        "MutantVrUiProfile"             = "scriptable"
        "MutantVrHapticClip"            = "scriptable"
    }

    if ($specialKinds.ContainsKey($ClassName)) {
        return $specialKinds[$ClassName]
    }

    if ($RelativePath -like "Tests/*") {
        return "test"
    }

    if ($RelativePath -like "Editor/Windows/*") {
        return "editor-window"
    }

    if ($RelativePath -like "Editor/Inspectors/*") {
        return "editor"
    }

    if ($RelativePath -like "Editor/*") {
        return "editor-static"
    }

    if ($ClassName.StartsWith("I")) {
        return "interface"
    }

    if ($ClassName.EndsWith("Event")) {
        return "event"
    }

    if ($ClassName.EndsWith("Utility")) {
        return "static"
    }

    if ($ClassName.EndsWith("Constants")) {
        return "static"
    }

    if ($ClassName.EndsWith("Profile")) {
        return "scriptable"
    }

    if (
        $ClassName.EndsWith("Root")      -or
        $ClassName.EndsWith("Anchor")    -or
        $ClassName.EndsWith("Renderer")  -or
        $ClassName.EndsWith("Interactor")-or
        $ClassName.EndsWith("Grabbable") -or
        $ClassName.EndsWith("Pointer")   -or
        $ClassName.EndsWith("Proxy")     -or
        $ClassName.EndsWith("Overlay")   -or
        $ClassName.EndsWith("DebugView") -or
        $ClassName.EndsWith("Tracker")   -or
        $ClassName.EndsWith("Updater")   -or
        $ClassName.EndsWith("Player")
    ) {
        return "monobehaviour"
    }

    if (
        $ClassName.EndsWith("Provider")  -or
        $ClassName.EndsWith("Binder")
    ) {
        return "monobehaviour"
    }

    return "plain"
}

function Get-CsContent {
    param([string]$RelativePath)

    $namespace = Get-CsNamespace -RelativePath $RelativePath
    $className = Get-ClassName -RelativePath $RelativePath
    $kind = Get-SpecialKind -RelativePath $RelativePath -ClassName $className

    switch ($kind) {
        "interface" {
            return @"
namespace $namespace
{
    public interface $className
    {
    }
}
"@
        }

        "enum" {
            return @"
namespace $namespace
{
    public enum $className
    {
        None = 0
    }
}
"@
        }

        "flags" {
            return @"
namespace $namespace
{
    [System.Flags]
    public enum $className
    {
        None = 0
    }
}
"@
        }

        "event" {
            return @"
namespace $namespace
{
    public readonly struct $className
    {
    }
}
"@
        }

        "scriptable" {
            return @"
using UnityEngine;

namespace $namespace
{
    [CreateAssetMenu(menuName = "Mutant/VR/$className", fileName = "$className")]
    public sealed class $className : ScriptableObject
    {
    }
}
"@
        }

        "monobehaviour" {
            return @"
using UnityEngine;

namespace $namespace
{
    public sealed class $className : MonoBehaviour
    {
    }
}
"@
        }

        "editor-window" {
            return @"
using UnityEditor;
using UnityEngine;

namespace $namespace
{
    public sealed class $className : EditorWindow
    {
        [MenuItem("Mutant/VR/$className")]
        public static void Open()
        {
            GetWindow<$className>("$className");
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("$className placeholder window.", MessageType.Info);
        }
    }
}
"@
        }

        "editor" {
            return @"
using UnityEditor;

namespace $namespace
{
    public sealed class $className : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
"@
        }

        "editor-static" {
            return @"
using UnityEditor;
using UnityEngine;

namespace $namespace
{
    public static class $className
    {
    }
}
"@
        }

        "test" {
            return @"
using NUnit.Framework;

namespace $namespace
{
    public sealed class $className
    {
        [Test]
        public void Placeholder()
        {
            Assert.Pass();
        }
    }
}
"@
        }

        "static" {
            return @"
namespace $namespace
{
    public static class $className
    {
    }
}
"@
        }

        default {
            return @"
namespace $namespace
{
    [System.Serializable]
    public sealed class $className
    {
    }
}
"@
        }
    }
}

$packageRoot = Join-Path $RootPath "Packages/com.mutant.vr"

$directories = @(
    "Packages/com.mutant.vr",
    "Packages/com.mutant.vr/Documentation~",
    "Packages/com.mutant.vr/Runtime",
    "Packages/com.mutant.vr/Runtime/Bootstrap",
    "Packages/com.mutant.vr/Runtime/Contracts",
    "Packages/com.mutant.vr/Runtime/Config",
    "Packages/com.mutant.vr/Runtime/Core",
    "Packages/com.mutant.vr/Runtime/Rig",
    "Packages/com.mutant.vr/Runtime/Input",
    "Packages/com.mutant.vr/Runtime/Tracking",
    "Packages/com.mutant.vr/Runtime/Ray",
    "Packages/com.mutant.vr/Runtime/Grab",
    "Packages/com.mutant.vr/Runtime/Locomotion",
    "Packages/com.mutant.vr/Runtime/Haptics",
    "Packages/com.mutant.vr/Runtime/UI",
    "Packages/com.mutant.vr/Runtime/Interaction",
    "Packages/com.mutant.vr/Runtime/Events",
    "Packages/com.mutant.vr/Runtime/Integrations",
    "Packages/com.mutant.vr/Runtime/Integrations/VRIF",
    "Packages/com.mutant.vr/Runtime/Integrations/OpenXR",
    "Packages/com.mutant.vr/Runtime/Integrations/SteamVR",
    "Packages/com.mutant.vr/Runtime/Utilities",
    "Packages/com.mutant.vr/Runtime/Diagnostics",
    "Packages/com.mutant.vr/Editor",
    "Packages/com.mutant.vr/Editor/Inspectors",
    "Packages/com.mutant.vr/Editor/Windows",
    "Packages/com.mutant.vr/Editor/Validation",
    "Packages/com.mutant.vr/Editor/MenuItems",
    "Packages/com.mutant.vr/Editor/AutoSetup",
    "Packages/com.mutant.vr/Tests",
    "Packages/com.mutant.vr/Tests/Runtime",
    "Packages/com.mutant.vr/Tests/Editor",
    "Packages/com.mutant.vr/Samples~",
    "Packages/com.mutant.vr/Samples~/BasicRigSample",
    "Packages/com.mutant.vr/Samples~/RayInteractionSample",
    "Packages/com.mutant.vr/Samples~/GrabInteractionSample",
    "Packages/com.mutant.vr/Samples~/VrUiSample",
    "Packages/com.mutant.vr/Samples~/VrifIntegrationSample"
)

foreach ($dir in $directories) {
    Ensure-Directory -Path (Join-Path $RootPath $dir)
}

$staticFiles = [ordered]@{
    "Packages/com.mutant.vr/package.json" = @'
{
  "name": "com.mutant.vr",
  "displayName": "Mutant VR",
  "version": "0.1.0",
  "unity": "2022.3",
  "description": "First-pass skeleton package for Mutant VR.",
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

    "Packages/com.mutant.vr/README.md" = @'
# com.mutant.vr

Mutant VR 第一版骨架包。

## 内容
- Runtime 基础模块目录
- Editor 工具目录
- Tests 目录
- Samples~ 占位目录

## 说明
- 当前为可编译空骨架版本
- `.unity` Sample Scene 请在 Unity Editor 中创建
'@

    "Packages/com.mutant.vr/CHANGELOG.md" = @'
# Changelog

## 0.1.0
- Initial skeleton structure for Mutant VR.
'@

    "Packages/com.mutant.vr/LICENSE.md" = @'
Placeholder license file.
'@

    "Packages/com.mutant.vr/Third Party Notices.md" = @'
Placeholder third party notices.
'@

    "Packages/com.mutant.vr/Documentation~/com.mutant.vr-overview.md" = @'
# Mutant VR Overview

第一版包结构总览文档。
'@

    "Packages/com.mutant.vr/Documentation~/com.mutant.vr-quickstart.md" = @'
# Mutant VR Quick Start

后续补充快速接入步骤。
'@

    "Packages/com.mutant.vr/Documentation~/com.mutant.vr-vrif-integration.md" = @'
# Mutant VR VRIF Integration

后续补充 VRIF 适配说明。
'@

    "Packages/com.mutant.vr/Documentation~/com.mutant.vr-module-lifecycle.md" = @'
# Mutant VR Module Lifecycle

后续补充模块生命周期说明。
'@

    "Packages/com.mutant.vr/Documentation~/com.mutant.vr-sample-setup.md" = @'
# Mutant VR Sample Setup

后续补充 Sample 场景搭建说明。
'@

    "Packages/com.mutant.vr/Runtime/com.mutant.vr.asmdef" = @'
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

    "Packages/com.mutant.vr/Editor/com.mutant.vr.editor.asmdef" = @'
{
  "name": "com.mutant.vr.editor",
  "rootNamespace": "Mutant.VR.Editor",
  "references": [
    "com.mutant.vr"
  ],
  "includePlatforms": [
    "Editor"
  ],
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

    "Packages/com.mutant.vr/Tests/Runtime/com.mutant.vr.tests.runtime.asmdef" = @'
{
  "name": "com.mutant.vr.tests.runtime",
  "rootNamespace": "Mutant.VR.Tests.Runtime",
  "references": [
    "com.mutant.vr",
    "UnityEngine.TestRunner",
    "UnityEditor.TestRunner",
    "nunit.framework"
  ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": true,
  "precompiledReferences": [
    "nunit.framework.dll"
  ],
  "autoReferenced": false,
  "defineConstraints": [
    "UNITY_INCLUDE_TESTS"
  ],
  "versionDefines": [],
  "noEngineReferences": false
}
'@

    "Packages/com.mutant.vr/Tests/Editor/com.mutant.vr.tests.editor.asmdef" = @'
{
  "name": "com.mutant.vr.tests.editor",
  "rootNamespace": "Mutant.VR.Tests.Editor",
  "references": [
    "com.mutant.vr",
    "com.mutant.vr.editor",
    "UnityEngine.TestRunner",
    "UnityEditor.TestRunner",
    "nunit.framework"
  ],
  "includePlatforms": [
    "Editor"
  ],
  "excludePlatforms": [],
  "allowUnsafeCode": false,
  "overrideReferences": true,
  "precompiledReferences": [
    "nunit.framework.dll"
  ],
  "autoReferenced": false,
  "defineConstraints": [
    "UNITY_INCLUDE_TESTS"
  ],
  "versionDefines": [],
  "noEngineReferences": false
}
'@

    "Packages/com.mutant.vr/Samples~/BasicRigSample/README.md" = @'
# BasicRigSample

请在 Unity Editor 中创建 `BasicRigSample.unity` 场景。
'@

    "Packages/com.mutant.vr/Samples~/RayInteractionSample/README.md" = @'
# RayInteractionSample

请在 Unity Editor 中创建 `RayInteractionSample.unity` 场景。
'@

    "Packages/com.mutant.vr/Samples~/GrabInteractionSample/README.md" = @'
# GrabInteractionSample

请在 Unity Editor 中创建 `GrabInteractionSample.unity` 场景。
'@

    "Packages/com.mutant.vr/Samples~/VrUiSample/README.md" = @'
# VrUiSample

请在 Unity Editor 中创建 `VrUiSample.unity` 场景。
'@

    "Packages/com.mutant.vr/Samples~/VrifIntegrationSample/README.md" = @'
# VrifIntegrationSample

请在 Unity Editor 中创建 `VrifIntegrationSample.unity` 场景。
'@
}

foreach ($entry in $staticFiles.GetEnumerator()) {
    Write-TextFile -Path (Join-Path $RootPath $entry.Key) -Content $entry.Value
}

$csharpFiles = @(
    "Runtime/Bootstrap/MutantVrModule.cs",
    "Runtime/Bootstrap/MutantVrInstaller.cs",
    "Runtime/Bootstrap/MutantVrRuntimeEntry.cs",
    "Runtime/Bootstrap/MutantVrRuntimeBinder.cs",

    "Runtime/Contracts/IMutantVrRigService.cs",
    "Runtime/Contracts/IMutantVrInputService.cs",
    "Runtime/Contracts/IMutantVrRayService.cs",
    "Runtime/Contracts/IMutantVrGrabService.cs",
    "Runtime/Contracts/IMutantVrHapticService.cs",
    "Runtime/Contracts/IMutantVrLocomotionService.cs",
    "Runtime/Contracts/IMutantVrTrackingService.cs",
    "Runtime/Contracts/IMutantVrUiPointerService.cs",
    "Runtime/Contracts/IMutantVrPlatformAdapter.cs",

    "Runtime/Config/MutantVrSettings.cs",
    "Runtime/Config/MutantVrRigProfile.cs",
    "Runtime/Config/MutantVrInputProfile.cs",
    "Runtime/Config/MutantVrRayProfile.cs",
    "Runtime/Config/MutantVrGrabProfile.cs",
    "Runtime/Config/MutantVrHapticProfile.cs",
    "Runtime/Config/MutantVrLocomotionProfile.cs",
    "Runtime/Config/MutantVrUiProfile.cs",

    "Runtime/Core/MutantVrServiceRegistry.cs",
    "Runtime/Core/MutantVrServiceLocator.cs",
    "Runtime/Core/MutantVrContext.cs",
    "Runtime/Core/MutantVrState.cs",
    "Runtime/Core/MutantVrFeatureFlags.cs",
    "Runtime/Core/MutantVrConstants.cs",

    "Runtime/Rig/MutantVrRigRoot.cs",
    "Runtime/Rig/MutantVrRigReferences.cs",
    "Runtime/Rig/MutantVrHeadAnchor.cs",
    "Runtime/Rig/MutantVrLeftHandAnchor.cs",
    "Runtime/Rig/MutantVrRightHandAnchor.cs",
    "Runtime/Rig/MutantVrRigValidator.cs",

    "Runtime/Input/MutantVrInputSnapshot.cs",
    "Runtime/Input/MutantVrInputFrame.cs",
    "Runtime/Input/MutantVrButtonState.cs",
    "Runtime/Input/MutantVrAxisState.cs",
    "Runtime/Input/MutantVrControllerSide.cs",
    "Runtime/Input/MutantVrInputRouter.cs",
    "Runtime/Input/MutantVrInputStateUpdater.cs",
    "Runtime/Input/MutantVrInputUtility.cs",

    "Runtime/Tracking/MutantVrTrackingPose.cs",
    "Runtime/Tracking/MutantVrTrackingState.cs",
    "Runtime/Tracking/MutantVrTrackingUpdater.cs",
    "Runtime/Tracking/MutantVrHeadTracker.cs",
    "Runtime/Tracking/MutantVrHandTracker.cs",
    "Runtime/Tracking/MutantVrTrackingUtility.cs",

    "Runtime/Ray/MutantVrRayInteractor.cs",
    "Runtime/Ray/MutantVrRayPose.cs",
    "Runtime/Ray/MutantVrRaycastRequest.cs",
    "Runtime/Ray/MutantVrRaycastResult.cs",
    "Runtime/Ray/MutantVrRayRenderer.cs",
    "Runtime/Ray/MutantVrRayHitMarker.cs",
    "Runtime/Ray/MutantVrRayUtility.cs",

    "Runtime/Grab/MutantVrGrabbable.cs",
    "Runtime/Grab/MutantVrGrabInteractor.cs",
    "Runtime/Grab/MutantVrGrabState.cs",
    "Runtime/Grab/MutantVrGrabPoint.cs",
    "Runtime/Grab/MutantVrThrowEstimator.cs",
    "Runtime/Grab/MutantVrGrabUtility.cs",

    "Runtime/Locomotion/MutantVrLocomotionCoordinator.cs",
    "Runtime/Locomotion/MutantVrMoveProvider.cs",
    "Runtime/Locomotion/MutantVrTurnProvider.cs",
    "Runtime/Locomotion/MutantVrTeleportProvider.cs",
    "Runtime/Locomotion/MutantVrTeleportAnchor.cs",
    "Runtime/Locomotion/MutantVrFadeController.cs",
    "Runtime/Locomotion/MutantVrLocomotionState.cs",

    "Runtime/Haptics/MutantVrHapticClip.cs",
    "Runtime/Haptics/MutantVrHapticRequest.cs",
    "Runtime/Haptics/MutantVrHapticPlayer.cs",
    "Runtime/Haptics/MutantVrHapticUtility.cs",

    "Runtime/UI/MutantVrUiPointer.cs",
    "Runtime/UI/MutantVrUiInputModuleBridge.cs",
    "Runtime/UI/MutantVrWorldCanvasBinder.cs",
    "Runtime/UI/MutantVrUiRaycasterProxy.cs",
    "Runtime/UI/MutantVrUiFocusState.cs",

    "Runtime/Interaction/MutantVrInteractable.cs",
    "Runtime/Interaction/MutantVrHoverState.cs",
    "Runtime/Interaction/MutantVrSelectState.cs",
    "Runtime/Interaction/MutantVrInteractionEventHub.cs",
    "Runtime/Interaction/MutantVrInteractionMask.cs",
    "Runtime/Interaction/MutantVrInteractionUtility.cs",

    "Runtime/Events/MutantVrRigReadyEvent.cs",
    "Runtime/Events/MutantVrInputUpdatedEvent.cs",
    "Runtime/Events/MutantVrRayHitEvent.cs",
    "Runtime/Events/MutantVrGrabStartedEvent.cs",
    "Runtime/Events/MutantVrGrabEndedEvent.cs",
    "Runtime/Events/MutantVrTeleportStartedEvent.cs",
    "Runtime/Events/MutantVrTeleportFinishedEvent.cs",
    "Runtime/Events/MutantVrHapticPlayedEvent.cs",

    "Runtime/Integrations/VRIF/MutantVrIfPlatformAdapter.cs",
    "Runtime/Integrations/VRIF/MutantVrIfRigProvider.cs",
    "Runtime/Integrations/VRIF/MutantVrIfInputProvider.cs",
    "Runtime/Integrations/VRIF/MutantVrIfRayProvider.cs",
    "Runtime/Integrations/VRIF/MutantVrIfGrabProvider.cs",
    "Runtime/Integrations/VRIF/MutantVrIfHapticProvider.cs",
    "Runtime/Integrations/VRIF/MutantVrIfLocomotionProvider.cs",

    "Runtime/Integrations/OpenXR/MutantVrOpenXrPlatformAdapter.cs",
    "Runtime/Integrations/OpenXR/MutantVrOpenXrInputProvider.cs",
    "Runtime/Integrations/OpenXR/MutantVrOpenXrTrackingProvider.cs",

    "Runtime/Integrations/SteamVR/MutantVrSteamPlatformAdapter.cs",
    "Runtime/Integrations/SteamVR/MutantVrSteamInputProvider.cs",
    "Runtime/Integrations/SteamVR/MutantVrSteamHapticProvider.cs",

    "Runtime/Utilities/MutantVrLayerMaskUtility.cs",
    "Runtime/Utilities/MutantVrTransformUtility.cs",
    "Runtime/Utilities/MutantVrPhysicsUtility.cs",
    "Runtime/Utilities/MutantVrMathUtility.cs",
    "Runtime/Utilities/MutantVrGizmoDrawer.cs",

    "Runtime/Diagnostics/MutantVrDebugSettings.cs",
    "Runtime/Diagnostics/MutantVrDebugOverlay.cs",
    "Runtime/Diagnostics/MutantVrRigDebugView.cs",
    "Runtime/Diagnostics/MutantVrInputDebugView.cs",
    "Runtime/Diagnostics/MutantVrRayDebugView.cs",
    "Runtime/Diagnostics/MutantVrRuntimeValidator.cs",

    "Editor/Inspectors/MutantVrSettingsEditor.cs",
    "Editor/Inspectors/MutantVrRigProfileEditor.cs",
    "Editor/Inspectors/MutantVrRayProfileEditor.cs",
    "Editor/Inspectors/MutantVrLocomotionProfileEditor.cs",

    "Editor/Windows/MutantVrSetupWindow.cs",
    "Editor/Windows/MutantVrDiagnosticsWindow.cs",
    "Editor/Windows/MutantVrRigPreviewWindow.cs",

    "Editor/Validation/MutantVrProjectValidator.cs",
    "Editor/Validation/MutantVrDependencyValidator.cs",
    "Editor/Validation/MutantVrSceneValidator.cs",

    "Editor/MenuItems/MutantVrMenuItems.cs",
    "Editor/MenuItems/MutantVrCreateAssetMenu.cs",

    "Editor/AutoSetup/MutantVrRigAutoBinder.cs",
    "Editor/AutoSetup/MutantVrVrifAutoSetup.cs",
    "Editor/AutoSetup/MutantVrDefaultAssetsGenerator.cs",

    "Tests/Runtime/MutantVrInputTests.cs",
    "Tests/Runtime/MutantVrRayTests.cs",
    "Tests/Runtime/MutantVrGrabTests.cs",
    "Tests/Runtime/MutantVrLocomotionTests.cs",

    "Tests/Editor/MutantVrConfigEditorTests.cs",
    "Tests/Editor/MutantVrValidationTests.cs",
    "Tests/Editor/MutantVrSetupTests.cs",

    "Samples~/BasicRigSample/BasicRigBootstrap.cs",
    "Samples~/RayInteractionSample/RayTargetDemo.cs",
    "Samples~/GrabInteractionSample/GrabCubeDemo.cs",
    "Samples~/VrUiSample/VrUiDemoPanel.cs",
    "Samples~/VrifIntegrationSample/VrifRigBootstrap.cs"
)

foreach ($relativeCsPath in $csharpFiles) {
    $fullPath = Join-Path $packageRoot $relativeCsPath
    $content = Get-CsContent -RelativePath $relativeCsPath
    Write-TextFile -Path $fullPath -Content $content
}

Write-Host ""
Write-Host "Mutant VR skeleton generated at:" -ForegroundColor Green
Write-Host $packageRoot -ForegroundColor Cyan
Write-Host ""
Write-Host "Generated file count:" -ForegroundColor Green
$allFiles = Get-ChildItem -Path $packageRoot -Recurse -File
Write-Host $allFiles.Count -ForegroundColor Cyan
Write-Host ""
Write-Host "Top-level tree:" -ForegroundColor Green
Get-ChildItem -Path $packageRoot | Select-Object Mode, LastWriteTime, Name