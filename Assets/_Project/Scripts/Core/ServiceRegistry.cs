using System;
using System.Collections.Generic;

namespace _Project.Scripts.Core
{
    public sealed class ServiceRegistry
    {
        private readonly Dictionary<Type, object> map = new();

        public void Register<T>(T instance) where T : class
        {
            map[typeof(T)] = instance;
        }

        public T Get<T>() where T : class
        {
            return map.TryGetValue(typeof(T), out var obj) ? (T)obj : null;
        }
    }
}