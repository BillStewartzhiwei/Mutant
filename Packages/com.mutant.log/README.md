# Mutant Log

Mutant Log 是 Mutant 的日志包，提供：

- 统一日志入口 `MutantLogger`
- 日志模块 `MutantLogModule`
- 可配置运行时设置 `MutantLogRuntimeSettings`
- Console / In-Memory / File 三种日志输出目标
- 与 `com.mutant.core` 的模块系统集成

## 功能

- 日志分级
- 多路输出
- 运行时内存缓存
- 文件落盘
- 与 ModuleManager 集成

## 使用方式

1. 在场景中确保已经有 `CoreBootstrap`
2. 创建一个 `MutantLogRuntimeSettings` 资源
3. 创建一个物体并挂载 `MutantLogModuleInstaller`
4. 将设置资源拖入 Installer
5. 运行场景后通过 `MutantLogger` 输出日志

## 示例

```csharp
using Mutant.Log.API;

MutantLogger.Info("Experiment", "TrialStart");
MutantLogger.Warning("VR", "Controller tracking lost");
MutantLogger.Error("LSL", "Resolve failed");