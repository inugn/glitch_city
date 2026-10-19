using System;
using System.Collections.Generic;

namespace _Project.Scripts.Core
{
    public sealed class ServiceLocator
    {
        private readonly Dictionary<Type, object> map = new();

        public void Register<T>(T instance) where T : class
            => map[typeof(T)] = instance;

        public void Register<TInterface>(object instance) where TInterface : class
            => map[typeof(TInterface)] = instance;

        public T Get<T>() where T : class
            => map.TryGetValue(typeof(T), out var obj) ? (T)obj : null;
    }
}