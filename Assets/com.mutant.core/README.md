# Mutant Core

Mutant Core 是 Mutant 框架的基础运行时包，提供：

- CoreBootstrap
- ModuleManager
- IModule
- EventBus
- CoreRecorder（调试录制）

## 安装方式

将本包放入项目目录：

Packages/com.mutant.core

Unity 会自动识别为本地 UPM 包。

## 首版目标

这一版只做最小核心能力：

- 模块注册
- 生命周期驱动
- 事件总线
- Bootstrap 启动入口

## 使用方式

1. 在场景中创建一个空物体，例如 `[MutantCore]`
2. 挂载 `CoreBootstrap`
3. 在其他初始化脚本中向 `ModuleManager` 注册模块
4. 运行场景，模块会自动收到生命周期调用

## 示例

```csharp
using UnityEngine;
using Mutant.Core.Modules;

public class DemoEntry : MonoBehaviour
{
    private void Awake()
    {
        ModuleManager.Instance.Register(new DemoModule());
    }

    private sealed class DemoModule : IModule
    {
        public int Priority => 0;

        public void Init()
        {
            Debug.Log("DemoModule Init");
        }

        public void Update() { }
        public void LateUpdate() { }
        public void FixedUpdate() { }

        public void Dispose()
        {
            Debug.Log("DemoModule Dispose");
        }
    }
}
```

## 防回归测试建议

- PlayMode 自动化用例：`Assets/com.mutant.core/Tests/PlayMode/CoreBootstrapPlayModeTests.cs`
  - 覆盖“重复 `CoreBootstrap` 实例被销毁时，不应触发模块 `DisposeAll()`”。
  - 覆盖“owner 实例销毁时，仅触发一次模块释放”。
- EditMode 测试规划：`Assets/com.mutant.core/Tests/EditMode/CoreBootstrapEditModeTestPlan.md`
  - 用于记录补充用例和手工/自动化迁移建议。

## 子模块调整项

- 保持 `Assets/com.mutant.core/package.json` 与 `Packages/com.mutant.core/package.json` 同步。
- 若把 Tests 同步到 `Packages/com.mutant.core`，请复核 asmdef 的平台过滤设置。

## CoreRecorder（参考 PLUME Recorder 思路）

`CoreRecorder` 提供轻量级运行时行为录制，默认关闭：

```csharp
using Mutant.Core.Diagnostics;

CoreRecorder.Enabled = true;
CoreRecorder.Clear();

// 运行游戏逻辑后
foreach (var entry in CoreRecorder.GetEntries())
{
    Debug.Log($"[{entry.UtcTime:HH:mm:ss}] {entry.Category}: {entry.Message}");
}
```

当前会录制：
- `CoreBootstrap` owner 初始化 / duplicate 销毁 / owner 销毁
- `ModuleManager` 注册、初始化、销毁、全量生命周期
- `EventBus` 订阅、取消订阅、发布、清理
