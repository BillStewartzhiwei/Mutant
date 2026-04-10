# Quick Start

这份文档用于快速接入 `com.mutant.log`。

---

## 1. 前置条件

确保项目中已经存在：

- `com.mutant.core`
- 一个场景内的 `CoreBootstrap`

`Mutant.Log` 依赖 `Mutant.Core` 的模块生命周期。

---

## 2. 创建运行时设置

在 Unity 中创建：

`Create > Mutant > Log > Runtime Settings`

得到一个 `MutantLogRuntimeSettings` 资源。

你可以在这里配置：

- 最低日志等级
- 是否输出到 Unity Console
- 是否启用 In-Memory Sink
- 是否启用 File Sink
- 输出目录与文件名规则

---

## 3. 场景中安装日志模块

在场景里创建一个物体，例如：

`[Bootstrap]/MutantLogModuleInstaller`

并挂载组件：

- `MutantLogModuleInstaller`

然后把刚才创建的 `MutantLogRuntimeSettings` 资源拖进去。

推荐结构：

```text
[Bootstrap]
 ├─ CoreBootstrap
 └─ MutantLogModuleInstaller