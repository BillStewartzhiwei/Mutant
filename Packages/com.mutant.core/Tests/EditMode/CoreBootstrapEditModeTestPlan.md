# CoreBootstrap 防回归测试建议（EditMode）

> 该文档用于补充 PlayMode 自动化测试之外的用例设计，聚焦重复 Bootstrap 实例的生命周期风险。

## 建议用例

1. **重复实例销毁不触发模块释放**
   - 前置：注册 `TrackingModule` 到 `ModuleManager`。
   - 步骤：
     1) 创建并激活第一个 `CoreBootstrap`。
     2) 再创建第二个 `CoreBootstrap`，触发其自销毁。
   - 断言：`TrackingModule.DisposeCount == 0`。

2. **Owner 销毁时仅释放一次**
   - 前置：同上。
   - 步骤：销毁 owner 对象。
   - 断言：`TrackingModule.DisposeCount == 1`。

3. **重复启停场景无跨用例污染**
   - 步骤：多轮执行“创建 owner -> 创建 duplicate -> 销毁 owner”。
   - 断言：每轮 `DisposeCount` 与预期一致，`ModuleManager.Instance.GetAllModules().Count == 0`。

## 子模块调整项（建议检查）

1. `Packages/com.mutant.core/package.json` 升级时，应同步检查 README / CHANGELOG / asmdef 兼容性。
2. 发布前建议在 CHANGELOG 记录此防回归测试的覆盖范围，以便子模块升级时验证。

## Recorder 对齐项（参考 PLUME Recorder 方案）

1. 录制默认关闭：`CoreRecorder.Enabled = false`，仅在调试/排障时开启。
2. 控制内存占用：通过 `CoreRecorder.MaxEntries` 限制缓存条目数量（环形裁剪）。
3. 录制维度统一：`Category + Message + UtcTime`，方便后续做可视化或导出。
4. 防回归断言建议：在 PlayMode 用例中校验关键生命周期日志（InitAll/DisposeAll）存在。
