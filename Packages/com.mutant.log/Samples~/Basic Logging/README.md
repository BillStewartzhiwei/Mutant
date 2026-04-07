# Basic Logging

这个 sample 演示：

1. 如何创建 `MutantLogRuntimeSettings`
2. 如何在场景中挂载 `MutantLogModuleInstaller`
3. 如何通过 `MutantLogger` 输出日志
4. 如何查看内存中的最近日志记录

## 使用步骤

1. 先确保场景中已经有 `CoreBootstrap`
2. 在 Package Manager 中导入 `Basic Logging`
3. 创建一个 `MutantLogRuntimeSettings` 资源
4. 创建一个空物体并挂载 `MutantLogModuleInstaller`
5. 将设置资源拖给 Installer
6. 再创建一个空物体挂载 `LoggingSampleEntry`
7. 运行场景并观察 Console 与日志文件输出