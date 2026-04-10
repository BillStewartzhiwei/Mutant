# Mutant Log

Mutant Log 是 Mutant 的日志包，提供：

- 统一日志入口 `MutantLogger`
- 日志模块 `MutantLogModule`
- 可配置运行时设置 `MutantLogRuntimeSettings`
- Console / In-Memory / File 三种日志输出目标
- 与 `com.mutant.core` 的模块系统集成

---

## 功能

- 日志分级：`Trace / Info / Warning / Error / Fatal`
- 统一公开 API：业务模块只依赖 `MutantLogger`
- 多路输出：控制台、内存缓存、文件
- 运行时日志缓冲快照
- 与 `ModuleManager` 生命周期集成

---

## 公开 API

推荐所有业务模块都只使用：

```csharp
using Mutant.Log.API;

MutantLogger.Info("Experiment", "Trial started.");
MutantLogger.Warning("VR", "Controller tracking lost.");
MutantLogger.Error("LSL", "Resolve failed.");