using System;
using System.Collections.Generic;
using UnityEngine;

namespace Markins.Runtime.Game
{
    public class LocalLoadSaveDataService : ILoadSaveDataService
    {
        public void Load(string key, Action<bool, string> callback)
        {
            var data = PlayerPrefsUtility.GetEncryptedString(key, string.Empty);

            if (string.IsNullOrEmpty(data))
            {
                callback?.Invoke(false, data);
            }
            else
            {
                callback?.Invoke(true, data);
            }
        }

        public void LoadAll(IEnumerable<string> keys, Action<bool, IList<string>> callback)
        {
            var loadData = new List<string>();

            foreach (var key in keys)
            {
                var data = PlayerPrefsUtility.GetEncryptedString(key, null);
                loadData.Add(data);
            }

            callback?.Invoke(true, loadData);
        }

        public void Save(string key, string data, Action<bool> callback = default)
        {
            PlayerPrefsUtility.SetEncryptedString(key, data);
            callback?.Invoke(true);
        }

        public void Delete(string key, Action<bool> onComplete)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void Delete(List<string> keys, Action<bool> onComplete)
        {
            foreach (var key in keys)
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
    }
}

//public bool LoadField(out List<ChipModel> chips)
//{
//    chips = new List<ChipModel>();

//    string saveData = PlayerPrefsUtility.GetEncryptedString(CHIPS_KEY, string.Empty);

//    if (saveData == string.Empty)
//        return false;

//    // Получили все элементы отдельно.
//    string[] piecesStr = null;
//    piecesStr = saveData.Split(';');
//    char[] ext = { ':', '+' };


//    for (int i = 0; i < piecesStr.Length - 1; i++)
//    {
//        ChipModel chipModel = new ChipModel();
//        string[] d = null;
//        d = piecesStr[i].Split(ext);
//        chipModel.SymbolId = int.Parse(d[0]);
//        chipModel.posX = float.Parse(d[1]);
//        chipModel.posZ = float.Parse(d[2]);
//        chips.Add(chipModel);
//    }

//    return true;
//}

//public void SaveField(IEnumerable<ChipView> chips)
//{
//    string saveData = string.Empty;
//    // Сохраняем все поле в строку в префсах.
//    //foreach (var chip in chips)
//    // saveData += chip.SymbolId + ":" + Math.Round(chip.transform.position.x, 3) + "+" + Math.Round(chip.transform.position.z, 3) + ";";

//    //PlayerPrefsUtility.SetEncryptedString(CHIPS_KEY, saveData);
//}

////public void SavePlayer(PlayerModel playerModel, Action<bool> finishCallback)
////{
////    var data = JsonConvert.SerializeObject(playerModel);
////    PlayerPrefsUtility.SetEncryptedString(LEVEL_DATA_KEY, data);
////}

//public void LoadGameData(Action<bool, GameSaveData> finishCallback)
//{
//    if (PlayerPrefsUtility.IsEncryptedKey(GameConstants.LEVEL_DATA_KEY) == false)
//    {
//        finishCallback?.Invoke(false, new GameSaveData());
//        return;
//    }

//    var data = new GameSaveData()
//    {
//        Level = PlayerPrefsUtility.GetEncryptedInt(GameConstants.LEVEL_DATA_KEY, 1),
//        Score = PlayerPrefsUtility.GetEncryptedInt(GameConstants.SCORE_DATA_KEY, 0),
//        BestScore = PlayerPrefsUtility.GetEncryptedInt(GameConstants.BEST_SCORE_DATA_KEY, 0),
//        Target = PlayerPrefsUtility.GetEncryptedInt(GameConstants.TARGET_SYMBOL_DATA_KEY, 32),
//        Money = PlayerPrefsUtility.GetEncryptedInt(GameConstants.MONEY_DATA_KET, 0),
//    };

//    finishCallback?.Invoke(true, data);
//}

//public void SaveGameData(GameSaveData playerModel, Action<bool> finishCallback)
//{
//    throw new NotImplementedException();
//}