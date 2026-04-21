using System;
using System.Collections.Generic;
using Mutant.Core;
using Mutant.Log;

namespace Mutant.LSL
{
    /// <summary>
    /// 当前版本的可运行 LSL 模块：
    /// - 接入 Mutant 启动编排
    /// - 接入 IMutantUpdatable
    /// - 默认以“轮询模拟器”方式工作，用于先验证模块启动和 Loop 调度
    ///
    /// 后续接 liblsl / LSL.cs 时，替换：
    /// - ConnectStream()
    /// - DisconnectStream()
    /// - PollOnce()
    /// 这三块即可。
    /// </summary>
    public sealed class LslModule : ModuleBase, IMutantUpdatable
    {
        private const float DefaultPollIntervalSeconds = 0.02f;
        private const float DefaultStatusLogIntervalSeconds = 2.0f;

        private bool _isInitialized;
        private bool _isStarted;
        private bool _isStreamConnected;

        private float _pollTimer;
        private float _statusLogTimer;

        private int _pollCount;
        private int _sampleCount;

        public override string ModuleId
        {
            get { return "Mutant.LSL"; }
        }

        public override MutantBootPhase BootPhase
        {
            get { return MutantBootPhase.Platform; }
        }

        public override int Order
        {
            get { return 20; }
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

        public bool IsStreamConnected
        {
            get { return _isStreamConnected; }
        }

        public int PollCount
        {
            get { return _pollCount; }
        }

        public int SampleCount
        {
            get { return _sampleCount; }
        }

        public override void OnRegister()
        {
            UnityEngine.Debug.Log("[Mutant.LSL] OnRegister");
        }

        public override void OnInit()
        {
            var logModule = MutantRuntimeBootstrap.ModuleManager != null
                ? MutantRuntimeBootstrap.ModuleManager.GetModule<LogModule>()
                : null;

            if (logModule == null)
            {
                throw new InvalidOperationException("Mutant.Log is not available for Mutant.LSL.");
            }

            _isInitialized = true;
            _isStarted = false;
            _isStreamConnected = false;
            _pollTimer = 0f;
            _statusLogTimer = 0f;
            _pollCount = 0;
            _sampleCount = 0;

            MutantLog.Info("Mutant.LSL", "OnInit");
        }

        public override void OnStart()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("Mutant.LSL cannot start before initialization.");
            }

            ConnectStream();

            _isStarted = true;
            _pollTimer = DefaultPollIntervalSeconds;
            _statusLogTimer = DefaultStatusLogIntervalSeconds;

            MutantLog.Info("Mutant.LSL", "OnStart");
        }

        public override void OnStop()
        {
            if (!_isStarted)
            {
                return;
            }

            DisconnectStream();
            _isStarted = false;

            MutantLog.Info("Mutant.LSL", "OnStop");
        }

        public override void OnDispose()
        {
            DisconnectStream();

            _isInitialized = false;
            _isStarted = false;
            _pollTimer = 0f;
            _statusLogTimer = 0f;
            _pollCount = 0;
            _sampleCount = 0;

            MutantLog.Info("Mutant.LSL", "OnDispose");
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_isStarted)
            {
                return;
            }

            if (!_isStreamConnected)
            {
                return;
            }

            _pollTimer -= deltaTime;
            while (_pollTimer <= 0f)
            {
                _pollTimer += DefaultPollIntervalSeconds;
                PollOnce();
            }

            _statusLogTimer -= deltaTime;
            if (_statusLogTimer <= 0f)
            {
                _statusLogTimer += DefaultStatusLogIntervalSeconds;

                MutantLog.Info(
                    "Mutant.LSL",
                    string.Format(
                        "Heartbeat connected={0}, pollCount={1}, sampleCount={2}",
                        _isStreamConnected,
                        _pollCount,
                        _sampleCount));
            }
        }

        private void ConnectStream()
        {
            // 这里先用“模拟已连接”的方式证明模块已经真正进入运行态。
            // 后续接 liblsl / LSL.cs 时，把真实的 resolve_stream / inlet 创建放在这里。
            _isStreamConnected = true;
            MutantLog.Info("Mutant.LSL", "ConnectStream success (simulated).");
        }

        private void DisconnectStream()
        {
            if (!_isStreamConnected)
            {
                return;
            }

            // 后续接真实 LSL 时，把 inlet 关闭和资源释放放在这里。
            _isStreamConnected = false;
            MutantLog.Info("Mutant.LSL", "DisconnectStream complete.");
        }

        private void PollOnce()
        {
            _pollCount++;

            // 当前版本先模拟每次轮询拉到 1 个样本。
            // 后续接真实 LSL 时，把 inlet.pull_sample(...) 或缓冲读取放在这里。
            _sampleCount++;
        }
    }
}