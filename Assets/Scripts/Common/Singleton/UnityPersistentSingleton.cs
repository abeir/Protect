using UnityEngine;

namespace Common.Singleton
{
    public class UnityPersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        private static bool _quit;
        
        public static T Instance
        {
            get
            {
                if (_quit)
                {
                    return _instance;
                }
                if (_instance != null)
                {
                    return _instance;
                }
                _instance = FindObjectOfType<T>();
                if (_instance != null)
                {
                    return _instance;
                }
                var obj = new GameObject
                {
                    name = typeof(T) + "_Auto"
                };
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }else if(this != _instance)
            {
                Destroy(gameObject);
            }
        }

        
        protected virtual void OnDestroy()
        {
            _quit = true;
        }
    }
}