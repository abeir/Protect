using System;
using System.Collections;
using UnityEngine;

namespace Common.Helper
{
    /// <summary>
    /// 启动一个协程来每帧执行回调，直至达到指定的次数。
    /// 若启动的协程未执行完毕，无法再次调用 Start，可以使用 Stop 来停止协程。
    /// </summary>
    public class IntervalPerFrame
    {
        private int times;
        private Action callback;
        private float delay;
        private MonoBehaviour mono;

        private IEnumerator coroutine;

        public static IntervalPerFrame Create(MonoBehaviour mono)
        {
            return new IntervalPerFrame(mono);
        }

        private IntervalPerFrame(MonoBehaviour mono)
        {
            this.mono = mono;
        }

        public IntervalPerFrame SetTimes(int t)
        {
            times = t;
            return this;
        }

        public IntervalPerFrame SetCallback(Action func)
        {
            callback = func;
            return this;
        }

        /// <summary>
        /// 设置延迟启动时间（毫秒）
        /// </summary>
        public IntervalPerFrame SetDelay(float d)
        {
            delay = d;
            return this;
        }

        public void Start()
        {
            if (times <= 0 || coroutine != null)
            {
                return;
            }
            coroutine = Coroutine();
            mono.StartCoroutine(coroutine);
        }

        public void Stop()
        {
            if (coroutine == null)
            {
                return;
            }
            mono.StopCoroutine(coroutine);
            coroutine = null;
        }

        private IEnumerator Coroutine()
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            for (var i=0; i<times; i++)
            {
                callback?.Invoke();
                yield return null;
            }
            coroutine = null;
        }
    }
}