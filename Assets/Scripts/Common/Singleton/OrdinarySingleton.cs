using System;

namespace Common.Singleton
{
    public class OrdinarySingleton<T>
    {
        private static T _instance;

        public static T Instance => _instance ??= Activator.CreateInstance<T>();
    }
}