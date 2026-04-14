using System;
using System.Collections.Generic;
using Mutant.Core;
using Mutant.Log;

namespace Mutant.VR
{
    public sealed class VrModule : ModuleBase
    {
        private bool _isInitialized;
        private bool _isStarted;

        public override string ModuleId
        {
            get { return "Mutant.VR"; }
        }

        public override MutantBootPhase BootPhase
        {
            get { return MutantBootPhase.Platform; }
        }

        public override int Order
        {
            get { return 10; }
        }

        public override IReadOnlyList<string> Dependencies
        {
            get { return new[] { "Mutant.Log" }; }
        }

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        public bool IsStarted
        {
            get { return _isStarted; }
        }

        public override void OnRegister()
        {
            // Register 阶段不要依赖日志服务是否已准备好，所以这里只用最安全的输出方式也行。
            UnityEngine.Debug.Log("[Mutant.VR] OnRegister");
        }

        public override void OnInit()
        {
            var logModule = MutantRuntimeBootstrap.ModuleManager?.GetModule<LogModule>();
            if (logModule == null)
            {
                throw new InvalidOperationException("Mutant.Log is not available for Mutant.VR.");
            }

            // 这里先做“准备”，不要在 Init 里直接开始跑交互逻辑
            // 例如后面可以放：
            // - 读取 VR 配置
            // - 解析输入适配器配置
            // - 缓存 Rig 查找策略
            // - 初始化交互状态机
            _isInitialized = true;

            MutantLog.Info("Mutant.VR", "OnInit");
        }

        public override void OnStart()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Mutant.VR cannot start before initialization.");
            }

            // 这里再开始真正的 VR 运行时准备
            // 例如后面可放：
            // - 启用输入提供器
            // - 激活射线交互服务
            // - 绑定 VR Rig
            _isStarted = true;

            MutantLog.Info("Mutant.VR", "OnStart");
        }

        public override void OnStop()
        {
            if (!_isStarted)
            {
                return;
            }

            _isStarted = false;
            MutantLog.Info("Mutant.VR", "OnStop");
        }

        public override void OnDispose()
        {
            _isInitialized = false;
            _isStarted = false;
            MutantLog.Info("Mutant.VR", "OnDispose");
        }
    }
}