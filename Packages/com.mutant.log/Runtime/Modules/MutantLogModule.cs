using System;
using System.Collections.Generic;
using UnityEngine;
using Mutant.Core.Modules;
using Mutant.Log.Config;
using Mutant.Log.Models;
using Mutant.Log.Services;
using Mutant.Log.Sinks;

namespace Mutant.Log.Modules
{
    public sealed class MutantLogModule : ModuleBase
    {
        private static MutantLogRuntimeSettings _configuredSettingsAsset;
        private static bool _ownsFallbackSettingsAsset;

        public static MutantLogService ActiveService { get; private set; }

        private MutantLogRuntimeSettings _runtimeSettingsAsset;
        private MutantInMemoryLogSink _memorySinkInstance;
        private MutantUnityConsoleLogSink _consoleSinkInstance;
        private MutantFileLogSink _fileSinkInstance;

        public override string Name => "MutantLogModule";
        public override int Priority => -500;

        public static void Configure(MutantLogRuntimeSettings logSettingsAsset)
        {
            _configuredSettingsAsset = logSettingsAsset;
            _ownsFallbackSettingsAsset = false;
        }

        public static IReadOnlyList<MutantLogRecord> GetBufferedRecordSnapshot()
        {
            MutantLogModule moduleInstance = ModuleManager.Instance.GetModule<MutantLogModule>();
            if (moduleInstance == null || moduleInstance._memorySinkInstance == null)
                return Array.Empty<MutantLogRecord>();

            return moduleInstance._memorySinkInstance.GetSnapshot();
        }

        protected override void OnInit()
        {
            _runtimeSettingsAsset = _configuredSettingsAsset;

            if (_runtimeSettingsAsset == null)
            {
                _runtimeSettingsAsset = MutantLogRuntimeSettings.CreateRuntimeFallback();
                _ownsFallbackSettingsAsset = true;
            }

            ActiveService = new MutantLogService(_runtimeSettingsAsset.minimumSeverity);

            if (_runtimeSettingsAsset.enableUnityConsoleSink)
            {
                _consoleSinkInstance = new MutantUnityConsoleLogSink();
                ActiveService.AddSink(_consoleSinkInstance);
            }

            if (_runtimeSettingsAsset.enableInMemorySink)
            {
                _memorySinkInstance = new MutantInMemoryLogSink(_runtimeSettingsAsset.retainedRecordCapacity);
                ActiveService.AddSink(_memorySinkInstance);
            }

            if (_runtimeSettingsAsset.enableFileSink)
            {
                _fileSinkInstance = new MutantFileLogSink(
                    _runtimeSettingsAsset.outputFolderName,
                    _runtimeSettingsAsset.fileNamePrefix,
                    _runtimeSettingsAsset.appendDateToFileName,
                    _runtimeSettingsAsset.flushAfterEachWrite);

                ActiveService.AddSink(_fileSinkInstance);
            }

            ActiveService.Write(MutantLogSeverity.Info, "Log", "MutantLogModule initialized.");
        }

        protected override void OnDispose()
        {
            if (ActiveService != null)
            {
                ActiveService.Write(MutantLogSeverity.Info, "Log", "MutantLogModule disposed.");
                ActiveService.Flush();
                ActiveService.Dispose();
                ActiveService = null;
            }

            _memorySinkInstance = null;
            _consoleSinkInstance = null;
            _fileSinkInstance = null;

            if (_ownsFallbackSettingsAsset && _runtimeSettingsAsset != null)
            {
                UnityEngine.Object.Destroy(_runtimeSettingsAsset);
                _runtimeSettingsAsset = null;
                _ownsFallbackSettingsAsset = false;
            }
        }
    }
}