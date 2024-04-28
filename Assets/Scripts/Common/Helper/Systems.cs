using Common.Singleton;
using UnityEngine;

namespace Common.Helper
{
    public class Systems : OrdinarySingleton<Systems>, ISingletonEvent
    {
        public Vector2 ReferenceResolution { get; private set; }

        public void OnCreate()
        {
            ReferenceResolution = new Vector2(1080, 1920);
        }
    }
}