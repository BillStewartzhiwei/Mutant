# CoreIntegrationExample

这个目录给的是最小接线说明：

- `MutantVrInstaller`
- `MutantVrVrifPlatformAdapter`
- `MutantVrCoreInstallExample`

你现有的 Core 生命周期可以这样接：

1. Core Init -> `InstallFromCore()`
2. Core Loop/Update -> `TickFromCore(deltaTime)`
3. Core Dispose -> `ShutdownFromCore()`