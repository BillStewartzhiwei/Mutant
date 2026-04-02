# Mutant Core

Mutant Core 是 Mutant 框架的基础运行时包，提供：

- CoreBootstrap
- ModuleManager
- IModule
- EventBus

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