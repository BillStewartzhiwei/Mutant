using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Mutant.Core
{
    /// <summary>
    /// PlayerLoop 注入器。
    /// 使用 GetCurrentPlayerLoop 获取当前 loop，再插入 Mutant 自定义系统。
    /// </summary>
    public static class PlayerLoopInstaller
    {
        private static bool s_isInstalled;
        private static PlayerLoopSystem s_previousLoop;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_isInstalled = false;
            s_previousLoop = default;
        }

        public static bool IsInstalled
        {
            get { return s_isInstalled; }
        }

        public static void Install()
        {
            if (s_isInstalled)
            {
                return;
            }

            var currentLoop = PlayerLoop.GetCurrentPlayerLoop();
            s_previousLoop = currentLoop;

            if (!TryInsertInto(ref currentLoop, typeof(Initialization), CreateSystem(typeof(MutantInitializationPhase), MutantLoop.DispatchInitialization)))
            {
                throw new InvalidOperationException("Failed to insert MutantInitializationPhase into Unity Initialization.");
            }

            if (!TryInsertInto(ref currentLoop, typeof(FixedUpdate), CreateSystem(typeof(MutantFixedUpdatePhase), MutantLoop.DispatchFixedUpdate)))
            {
                throw new InvalidOperationException("Failed to insert MutantFixedUpdatePhase into Unity FixedUpdate.");
            }

            if (!TryInsertInto(ref currentLoop, typeof(Update), CreateSystem(typeof(MutantUpdatePhase), MutantLoop.DispatchUpdate)))
            {
                throw new InvalidOperationException("Failed to insert MutantUpdatePhase into Unity Update.");
            }

            if (!TryInsertInto(ref currentLoop, typeof(PreLateUpdate), CreateSystem(typeof(MutantLateUpdatePhase), MutantLoop.DispatchLateUpdate)))
            {
                throw new InvalidOperationException("Failed to insert MutantLateUpdatePhase into Unity PreLateUpdate.");
            }

            PlayerLoop.SetPlayerLoop(currentLoop);
            s_isInstalled = true;

            Debug.Log("[Mutant.Core] PlayerLoop installed.");
        }

        public static void Uninstall()
        {
            if (!s_isInstalled)
            {
                return;
            }

            PlayerLoop.SetPlayerLoop(s_previousLoop);
            s_isInstalled = false;

            Debug.Log("[Mutant.Core] PlayerLoop restored.");
        }

        private static PlayerLoopSystem CreateSystem(Type systemType, PlayerLoopSystem.UpdateFunction updateFunction)
        {
            return new PlayerLoopSystem
            {
                type = systemType,
                subSystemList = null,
                updateDelegate = updateFunction
            };
        }

        private static bool TryInsertInto(ref PlayerLoopSystem node, Type parentType, PlayerLoopSystem systemToInsert)
        {
            if (node.type == parentType)
            {
                var children = node.subSystemList != null
                    ? new List<PlayerLoopSystem>(node.subSystemList)
                    : new List<PlayerLoopSystem>();

                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i].type == systemToInsert.type)
                    {
                        return true;
                    }
                }

                children.Add(systemToInsert);
                node.subSystemList = children.ToArray();
                return true;
            }

            if (node.subSystemList == null || node.subSystemList.Length == 0)
            {
                return false;
            }

            var subSystems = node.subSystemList;
            for (int i = 0; i < subSystems.Length; i++)
            {
                var child = subSystems[i];
                if (TryInsertInto(ref child, parentType, systemToInsert))
                {
                    subSystems[i] = child;
                    node.subSystemList = subSystems;
                    return true;
                }
            }

            return false;
        }
    }
}