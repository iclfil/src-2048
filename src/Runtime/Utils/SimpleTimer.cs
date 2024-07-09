using System;
using System.Collections;
using UnityEngine;

namespace Markins.Runtime.Game.Utils
{
    public class SimpleTimer : Singleton<SimpleTimer>
    {
        public void StartTask(float timer, Action callback)
        {
            StartCoroutine(TaskCoroutine(timer, callback));
        }

        private IEnumerator TaskCoroutine(float timer, Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }
    }
}