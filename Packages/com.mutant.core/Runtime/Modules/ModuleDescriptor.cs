using System;

namespace Mutant.Core
{
    /// <summary>
    /// 模块描述对象。
    /// 启动编排和依赖解析都基于 Descriptor，而不是直接在排序逻辑里操作模块实例。
    /// </summary>
    public sealed class ModuleDescriptor
    {
        public ModuleDescriptor(
            IMutantModule instance,
            int registrationIndex,
            bool autoStart,
            bool required,
            bool allowFailure,
            string displayName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (string.IsNullOrWhiteSpace(instance.ModuleId))
            {
                throw new ArgumentException("ModuleId cannot be null or empty.", nameof(instance));
            }

            Instance = instance;
            RegistrationIndex = registrationIndex;
            AutoStart = autoStart;
            Required = required;
            AllowFailure = required ? false : allowFailure;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? instance.ModuleId : displayName;
        }

        public IMutantModule Instance { get; }

        public string ModuleId
        {
            get { return Instance.ModuleId; }
        }

        public Type ModuleType
        {
            get { return Instance.GetType(); }
        }

        public MutantBootPhase BootPhase
        {
            get { return Instance.BootPhase; }
        }

        public int Order
        {
            get { return Instance.Order; }
        }

        public System.Collections.Generic.IReadOnlyList<string> Dependencies
        {
            get { return Instance.Dependencies; }
        }

        public int RegistrationIndex { get; }

        public bool AutoStart { get; }

        public bool Required { get; }

        public bool AllowFailure { get; }

        public string DisplayName { get; }

        public bool IsCritical
        {
            get { return Required || !AllowFailure; }
        }

        public override string ToString()
        {
            return string.Format(
                "{0} ({1}, Phase={2}, Order={3}, RegistrationIndex={4})",
                ModuleId,
                ModuleType.Name,
                BootPhase,
                Order,
                RegistrationIndex);
        }
    }
}