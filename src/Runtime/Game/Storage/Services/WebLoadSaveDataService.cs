using System;
using System.Collections.Generic;
using System.Linq;
using InstantGamesBridge;

namespace Markins.Runtime.Game
{
    public class WebLoadSaveDataService : ILoadSaveDataService
    {
        public void Save(string key, string data, Action<bool> callback = default)
        {
            Bridge.storage.Set(key, data, callback);
        }

        public void LoadAll(IEnumerable<string> keys, Action<bool, IList<string>> callback)
        {
            Bridge.storage.Get(keys.ToList(), delegate(bool success, List<string> list)
            {
                if (success == false)
                {
                    callback?.Invoke(false, null);
                }
                else
                {
                    callback?.Invoke(true, list);
                }
            });
        }

        public void Load(string key, Action<bool, string> callback)
        {
            Bridge.storage.Get(key, callback);
        }

        public void Delete(List<string> keys, Action<bool> onComplete = null)
        {
            Bridge.storage.Delete(keys, onComplete);
        }

        public void Delete(string key, Action<bool> onComplete = null)
        {
            Bridge.storage.Delete(key, onComplete);
        }
    }
}