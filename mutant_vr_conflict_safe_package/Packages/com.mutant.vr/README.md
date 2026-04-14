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
