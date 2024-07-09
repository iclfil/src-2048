using System;
using System.Collections.Generic;

namespace Markins.Runtime.Game
{
    public interface ILoadSaveDataService
    {
        void Save(string key, string data, Action<bool> callback = default);
        void Load(string key, Action<bool, string> callback);
        void LoadAll(IEnumerable<string> keys, Action<bool,IList<string>> callback);
        void Delete(string key, Action<bool> onComplete);
        void Delete(List<string> keys, Action<bool> onComplete);
    }
}