using System;
using System.Collections.Generic;

namespace Mutant.VR.Core
{
    public sealed class MutantVrServiceRegistry
    {
        private readonly Dictionary<Type, object> _serviceMap = new Dictionary<Type, object>();

        public void Register<TService>(TService service)
            where TService : class
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            _serviceMap[typeof(TService)] = service;
        }

        public bool TryGet<TService>(out TService service)
            where TService : class
        {
            if (_serviceMap.TryGetValue(typeof(TService), out object boxed))
            {
                service = boxed as TService;
                return service != null;
            }

            service = null;
            return false;
        }

        public bool Contains<TService>()
            where TService : class
        {
            return _serviceMap.ContainsKey(typeof(TService));
        }

        public void Clear()
        {
            _serviceMap.Clear();
        }
    }
}
