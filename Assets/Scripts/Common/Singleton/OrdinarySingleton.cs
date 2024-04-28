using System;

namespace Common.Singleton
{

    public interface ISingletonEvent
    {
        public void OnCreate();
    }


    public class OrdinarySingleton<T>
    {

        private static readonly Lazy<T> _lazy = new Lazy<T>(_Create);

        public static T Instance => _lazy.Value;

        private static T _Create()
        {
            var instance = Activator.CreateInstance<T>();
            if (instance is ISingletonEvent e)
            {
                e.OnCreate();
            }

            return instance;
        }

        protected OrdinarySingleton()
        {
        }
    }
}