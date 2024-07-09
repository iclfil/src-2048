using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Markins.Runtime.Game.Controllers;
using Markins.Runtime.Game.Models;
using Markins.Runtime.Game.Storage.Converter;
using Markins.Runtime.Game.Storage.Models;
using UnityEngine;

namespace Markins.Runtime.Game.Storage
{
    public class GameStorage
    {
        public const string LEVEL_KEY = "Level";
        public const string SCORE_KEY = "Score";
        public const string BEST_SCORE_KEY = "BestScore";
        public const string TARGET_KEY = "Target";
        public const string MONEY_KEY = "Money";
        public const string FIELD_CHIPS_KEY = "FC";


        public const string CHIP_SKINS_KEY = "Chips";
        public const string EFFECT_SKINS = "Effects";
        public const string FIELD_SKINS = "Field";
        public const string GAME_THEMES = "Games";

        private readonly ILoadSaveDataService _loadSaveDataService;

        private GameModel _gameModel;
        private InventoryModel _inventoryModel;
        private SettingsModel _settingsModel;

        private int _level;
        private int _money;
        private int _score;
        private int _bestScore;
        private int _targetChip;
        private List<string> _chips = new();
        private List<string> _effects = new();
        private List<string> _fields = new();
        private List<string> _gamesThemes = new();

        private List<StorageKey> _keys = new();

        public GameStorage(ILoadSaveDataService loadSaveDataService)
        {
            _loadSaveDataService = loadSaveDataService;

            _keys.Add(CreateKey(Keys.LEVEL, LEVEL_KEY));
            _keys.Add(CreateKey(Keys.MONEY, MONEY_KEY));
            _keys.Add(CreateKey(Keys.SCORE, SCORE_KEY));
            _keys.Add(CreateKey(Keys.TARGET, TARGET_KEY));
            _keys.Add(CreateKey(Keys.CHIPS, CHIP_SKINS_KEY));
            _keys.Add(CreateKey(Keys.EFFECTS, EFFECT_SKINS));
            _keys.Add(CreateKey(Keys.FIELDS, FIELD_SKINS));
            _keys.Add(CreateKey(Keys.GAME_THEMES, GAME_THEMES));
            _keys.Add(CreateKey(Keys.FIELD_CHIPS, FIELD_CHIPS_KEY));
        }

        private FieldController _fieldController;//плохо, а что поделать, релиз уже рядом

        public void Init(GameModel gameModel, InventoryModel inventoryModel, SettingsModel settingsModel, FieldController fieldController)
        {
            _gameModel = gameModel;
            _inventoryModel = inventoryModel;
            _settingsModel = settingsModel;
            _fieldController = fieldController;
        }


        private StorageKey CreateKey(Keys key, string keyValue)
        {
            return new StorageKey() { Key = key, Value = keyValue };
        }


        public enum LoadingState
        {
            Start,
            Loading,
            Success,
            Failed,
        }

        public LoadingState State;

        public IEnumerator _Load()
        {
            if (State == LoadingState.Loading)
                yield break;

            State = LoadingState.Start;

            Debug.Log("Start Loading");
            yield return _Loading();
            Debug.Log("Finish Loading" + State);
        }

        public IEnumerator _SaveField(IEnumerable<ChipController> chips)
        {
            Debug.Log("SAVE FIELD");
            var data = DataConverter.FieldChipsToData(chips);
            _loadSaveDataService.Save(FIELD_CHIPS_KEY, data);
            yield break;
        }

        public IEnumerator _LoadField(Action<bool, IEnumerable<DataConverter.ChipData>> resultCallback)
        {

            var timeToloading = 5f;
            var finishLoading = false;
            (bool, string) resultLoading = (false, null);

            _loadSaveDataService.Load(FIELD_CHIPS_KEY, (succes, data) =>
            {
                finishLoading = true;
                resultLoading.Item1 = succes;
                resultLoading.Item2 = data;
            });

            while (timeToloading > 0f)
            {
                if (finishLoading)
                    break;

                yield return new WaitForEndOfFrame();
                timeToloading -= Time.deltaTime;
            }

            if (resultLoading.Item1 == true)
            {
                var chips = DataConverter.DataToFieldChips(resultLoading.Item2);
                resultCallback?.Invoke(true, chips);
            }
            else
            {
                resultCallback?.Invoke(false, null);
            }
        }


        //самый простой способ сделать сейв, но не очень гибкий - дубляж загруженных данных и проверка их с данными из моделей, при сохранении
        public IEnumerator Save()
        {

            if (_score != _gameModel.Score)
            {
                _score = _gameModel.Score;
                _loadSaveDataService.Save(SCORE_KEY, ScoreToData(_score));
            }

            if (_level != _gameModel.Level)
            {
                _level = _gameModel.Level;
                _loadSaveDataService.Save(LEVEL_KEY, LevelToData(_level));
            }

            if (_money != _inventoryModel.Money)
            {

                _money = _inventoryModel.Money;
                _loadSaveDataService.Save(MONEY_KEY, MoneyToData(_money));
            }

            if (_targetChip != _gameModel.TargetChip)
            {
                _targetChip = _gameModel.TargetChip;
                _loadSaveDataService.Save(TARGET_KEY, TargetChipToData(_targetChip));
            }

            if (_bestScore != _gameModel.BestScore)
            {
                _bestScore = _gameModel.BestScore;
                _loadSaveDataService.Save(BEST_SCORE_KEY, BestScoreToData(_bestScore));
            }

            if (_chips.SequenceEqual(_inventoryModel.ChipSkins) == false)
            {

                _chips = _inventoryModel.ChipSkins;
                _loadSaveDataService.Save(CHIP_SKINS_KEY, ChipsSkinsToData(_chips));
            }

            if (_effects.SequenceEqual(_inventoryModel.Effects) == false)
            {

                _effects = _inventoryModel.Effects;
                _loadSaveDataService.Save(EFFECT_SKINS, EffectsSkinsToData(_effects));
            }

            if (_fields.SequenceEqual(_inventoryModel.Fields) == false)
            {

                _fields = _inventoryModel.Fields;
                _loadSaveDataService.Save(FIELD_SKINS, FieldSkinsToData(_fields));
            }

            if (_gamesThemes.SequenceEqual(_inventoryModel.GameThemes) == false)
            {
                _gamesThemes = _inventoryModel.GameThemes;
                _loadSaveDataService.Save(GAME_THEMES, GameThemesToData(_gamesThemes));
            }


            yield break;
        }

        private string TargetChipToData(int targetChip)
        {
            return DataConverter.TargetChipToData(targetChip);
        }


        private IEnumerator _Loading()
        {
            var timeToloading = 5f;

            while (timeToloading > 0f)
            {
                switch (State)
                {
                    case LoadingState.Start:
                        State = LoadingState.Loading;
                        var keys = _keys.ToList().Select(x => x.Value);
                        _loadSaveDataService.LoadAll(keys, FinishLoaded);
                        break;

                    case LoadingState.Loading:
                        if (timeToloading < 0f)
                            State = LoadingState.Failed;

                        Debug.Log("TIME LOADING" + timeToloading);

                        break;

                    case LoadingState.Success:
                        Debug.Log("State Success");
                        yield break;

                    case LoadingState.Failed:
                        Debug.Log("State Failed");
                        yield break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                yield return new WaitForEndOfFrame();

                timeToloading -= Time.deltaTime;
            }

        }

        private void FinishLoaded(bool success, IList<string> data)
        {
            if (success == false)
            {
                State = LoadingState.Failed;
                return;
            }

            //есть ли у соответствующего ключа данные
            for (var i = 0; i < _keys.Count; i++)
            {
                var key = _keys[i];
                var index = i;

                if (index < data.Count)
                {
                    if (string.IsNullOrEmpty(data[index]) == false)
                    {
                        HandleData(key.Key, data[index]);
                    }
                }
            }

            State = LoadingState.Success;
            Debug.Log("FINISH LOADED" + State);
        }

        private void HandleData(Keys keyType, string data)
        {
            if (keyType == Keys.LEVEL)
            {
                _level = DataToLevel(data);
                _gameModel.SetLevel(_level);
            }

            if (keyType == Keys.MONEY)
            {
                _money = DataToMoney(data);
                _inventoryModel.SetMoney(_money);
            }

            if (keyType == Keys.SCORE)
            {
                _score = DataToScore(data);
                _gameModel.SetScore(_score);
            }

            if (keyType == Keys.TARGET)
            {
                _targetChip = DataToTargetChip(data);
                _gameModel.SetTargetChip(_targetChip);
            }

            if (keyType == Keys.FIELD_CHIPS)
            {
                var chips = DataConverter.DataToFieldChips(data);
                _fieldController.AddChips(chips);
            }

            if (keyType == Keys.BEST_SCORE)
            {
                _bestScore = DataToBestScore(data);
                _gameModel.SetBestScore(_bestScore);
            }

            if (keyType == Keys.CHIPS)
            {
                _chips = DataToChips(data);
                _inventoryModel.SetChipSkins(_chips);
            }

            if (keyType == Keys.EFFECTS)
            {
                _effects = DataToEffectSkins(data);
                _inventoryModel.SetEffectSkins(_effects);
            }

            if (keyType == Keys.FIELDS)
            {
                _fields = DataToFieldSkins(data);
                _inventoryModel.SetFieldSkins(_fields);
            }

            if (keyType == Keys.GAME_THEMES)
            {
                _gamesThemes = DataToGameThemes(data);
                _inventoryModel.SetGameSkins(_gamesThemes);
            }
        }

        private int DataToTargetChip(string data)
        {
            return DataConverter.DataToTargetChip(data);
        }

        private string ScoreToData(int score) => DataConverter.ScoreToData(score);
        private string MoneyToData(int money) => DataConverter.MoneyToData(money);
        private string LevelToData(int level) => DataConverter.LevelToData(level);
        private string BestScoreToData(int bestScore) => DataConverter.BestScoreToData(bestScore);
        private string GameThemesToData(List<string> chips) => DataConverter.GameThemesToData(chips);
        private string FieldSkinsToData(List<string> chips) => DataConverter.FieldSkinsToData(chips);

        private string EffectsSkinsToData(List<string> effects) => DataConverter.EffectsToData(effects);

        private List<string> DataToChips(string data) => DataConverter.DataToChips(data);

        private string ChipsSkinsToData(List<string> chips)
        {
            return DataConverter.ChipsSkinsToData(chips);
        }

        private int DataToLevel(string data)
        {
            return DataConverter.DataToLevel(data);
        }

        private int DataToMoney(string data)
        {
            return DataConverter.DataToMoney(data);
        }

        private int DataToScore(string data)
        {
            return DataConverter.DataToScore(data);
        }

        private int DataToBestScore(string data)
        {
            return DataConverter.DataToBestScore(data);
        }

        private List<string> DataToGameThemes(string data)
        {
            return DataConverter.DataToGameThemes(data);
        }

        private List<string> DataToFieldSkins(string data)
        {
            return DataConverter.DataToFieldsSkins(data);
        }

        private List<string> DataToEffectSkins(string data)
        {
            return DataConverter.DataToEffectSkins(data);
        }

        public void LoadLevel(Action<int> levelLoaded)
        {
            _loadSaveDataService.Load(LEVEL_KEY, (success, data) =>
            {
                if (success)
                {
                    var level = DataToLevel(data);
                    levelLoaded?.Invoke(level);
                }
            });
        }

        public void DeleteChipOnField()
        {
            _loadSaveDataService.Save(FIELD_CHIPS_KEY,"");
        }

        public void DeleteAll()
        {
            Debug.Log("Delete All");
            foreach (var storageKey in _keys)
            {
                _loadSaveDataService.Delete(storageKey.Value,null);
            }
        }

        public void ResetData()
        {
            Debug.Log("Reset All");
            foreach (var storageKey in _keys)
            {
                _loadSaveDataService.Save(storageKey.Value,"");
            }
        }
    }
}