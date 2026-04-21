using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mutant.Core
{
    /// <summary>
    /// Mutant 运行时调度中心。
    /// 这里只负责调度，不负责模块启动顺序。
    /// </summary>
    public static class MutantLoop
    {
        private sealed class UpdateEntry
        {
            public string ModuleId;
            public IMutantUpdatable Callback;
        }

        private sealed class FixedUpdateEntry
        {
            public string ModuleId;
            public IMutantFixedUpdatable Callback;
        }

        private sealed class LateUpdateEntry
        {
            public string ModuleId;
            public IMutantLateUpdatable Callback;
        }

        private static readonly List<UpdateEntry> s_updateEntries = new List<UpdateEntry>();
        private static readonly List<FixedUpdateEntry> s_fixedUpdateEntries = new List<FixedUpdateEntry>();
        private static readonly List<LateUpdateEntry> s_lateUpdateEntries = new List<LateUpdateEntry>();

        private static readonly HashSet<string> s_updateIds = new HashSet<string>(StringComparer.Ordinal);
        private static readonly HashSet<string> s_fixedUpdateIds = new HashSet<string>(StringComparer.Ordinal);
        private static readonly HashSet<string> s_lateUpdateIds = new HashSet<string>(StringComparer.Ordinal);

        private static bool s_firstInitializationTickDone;

        public static int UpdateCount
        {
            get { return s_updateEntries.Count; }
        }

        public static int FixedUpdateCount
        {
            get { return s_fixedUpdateEntries.Count; }
        }

        public static int LateUpdateCount
        {
            get { return s_lateUpdateEntries.Count; }
        }

        public static void Clear()
        {
            s_updateEntries.Clear();
            s_fixedUpdateEntries.Clear();
            s_lateUpdateEntries.Clear();

            s_updateIds.Clear();
            s_fixedUpdateIds.Clear();
            s_lateUpdateIds.Clear();

            s_firstInitializationTickDone = false;
        }

        public static void RegisterModule(string moduleId, object module)
        {
            if (string.IsNullOrWhiteSpace(moduleId))
            {
                throw new ArgumentException("moduleId cannot be null or empty.", nameof(moduleId));
            }

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            var updatable = module as IMutantUpdatable;
            if (updatable != null && s_updateIds.Add(moduleId))
            {
                s_updateEntries.Add(new UpdateEntry
                {
                    ModuleId = moduleId,
                    Callback = updatable
                });
            }

            var fixedUpdatable = module as IMutantFixedUpdatable;
            if (fixedUpdatable != null && s_fixedUpdateIds.Add(moduleId))
            {
                s_fixedUpdateEntries.Add(new FixedUpdateEntry
                {
                    ModuleId = moduleId,
                    Callback = fixedUpdatable
                });
            }

            var lateUpdatable = module as IMutantLateUpdatable;
            if (lateUpdatable != null && s_lateUpdateIds.Add(moduleId))
            {
                s_lateUpdateEntries.Add(new LateUpdateEntry
                {
                    ModuleId = moduleId,
                    Callback = lateUpdatable
                });
            }
        }

        public static void DispatchInitialization()
        {
            if (s_firstInitializationTickDone)
            {
                return;
            }

            s_firstInitializationTickDone = true;
            Debug.Log("[Mutant.Core] PlayerLoop initialization phase entered.");
        }

        public static void DispatchUpdate()
        {
            var deltaTime = Time.deltaTime;

            for (int i = 0; i < s_updateEntries.Count; i++)
            {
                var entry = s_updateEntries[i];

                try
                {
                    entry.Callback.OnUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(
                        new InvalidOperationException(
                            string.Format("Mutant Update failed in module '{0}'.", entry.ModuleId),
                            exception));
                }
            }
        }

        public static void DispatchFixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;

            for (int i = 0; i < s_fixedUpdateEntries.Count; i++)
            {
                var entry = s_fixedUpdateEntries[i];

                try
                {
                    entry.Callback.OnFixedUpdate(fixedDeltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(
                        new InvalidOperationException(
                            string.Format("Mutant FixedUpdate failed in module '{0}'.", entry.ModuleId),
                            exception));
                }
            }
        }

        public static void DispatchLateUpdate()
        {
            var deltaTime = Time.deltaTime;

            for (int i = 0; i < s_lateUpdateEntries.Count; i++)
            {
                var entry = s_lateUpdateEntries[i];

                try
                {
                    entry.Callback.OnLateUpdate(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(
                        new InvalidOperationException(
                            string.Format("Mutant LateUpdate failed in module '{0}'.", entry.ModuleId),
                            exception));
                }
            }
        }
    }
}