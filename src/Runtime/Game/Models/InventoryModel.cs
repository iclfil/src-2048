using System;
using System.Collections;
using System.Collections.Generic;
using Markins.Runtime.Game.Configs;
using UnityEngine;

namespace Markins.Runtime.Game.Storage.Models
{
    [Serializable]
    public class InventoryModel
    {
        [SerializeField] private int _money;

        [SerializeField] private List<string> _chipsSkins = new();
        [SerializeField] private List<string> _effects = new();
        [SerializeField] private List<string> _fields = new();
        [SerializeField] private List<string> _gamesThemes = new();

        public List<string> Effects => _effects;
        public List<string> ChipSkins => _chipsSkins;
        public List<string> Fields => _fields;
        public List<string> GameThemes => _gamesThemes;

        public int Money => _money;

        private string _gameSkin;
        public string GameSkin => _gameSkin;
        public Action<string> OnGameSkinChanged;

        public string _fieldSkin;
        public string FieldSkin => _fieldSkin;
        public Action<string> OnFieldSkinChanged;

        private string _chipSkin;
        public string ChipSkin => _chipSkin;
        public Action<string> OnChipSkinChanged;

        private string _effectSkin;
        public string EffectSkin => _effectSkin;

        public Action<string> OnEffectSkinChanged;


        private GameStorage _gameStorage;

        public Action<int, int> OnMoneyChanged;
        private InventoryConfig _config;

        public void Init(InventoryConfig config)
        {
            _config = config;
            SetMoney(_config.PresetInventoryModel.Money);
        }

        public void Free()
        {
        }

        public void SetGameSkins(List<string> skins)
        {
            _gamesThemes = new List<string>(skins);
        }

        public void SetEffectSkins(List<string> skins)
        {
            _effects = new List<string>(skins);
        }

        public void SetFieldSkins(List<string> skins)
        {
            _fields = new List<string>(skins);
        }

        public void SetChipSkins(List<string> skins)
        {
            _chipsSkins = new List<string>(skins);
        }

        public void SetMoney(int money)
        {
            if (money < 0)
                money = 0;

            var prev = _money;

            _money = money;

            OnMoneyChanged?.Invoke(prev, _money);
        }

        public void SpendMoney(int value)
        {
            SetMoney(_money - value);
        }

        public void AddMoney(int value)
        {
            SetMoney(_money + value);
        }

        public void AddChip(string name)
        {
            _chipsSkins.Add(name);
        }

        public void AddEffects(List<string> effects)
        {
            _effects.AddRange(effects);
        }

        public void AddEffect(string name)
        {
            _effects.Add(name);
        }

        public void SelectEffect(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (_effectSkin == name)
                return;

            _effectSkin = name;
            OnEffectSkinChanged?.Invoke(_effectSkin);
        }

        public bool HasEffectSkin(string name)
        {
            return _effects.Contains(name);
        }

        public void AddGame(string name)
        {
            _gamesThemes.Add(name);
        }

        public void AddField(string name)
        {
            _fields.Add(name);
        }

        public void SelectChip(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (_chipSkin == name)
                return;


            _chipSkin = name;
            OnChipSkinChanged?.Invoke(_chipSkin);
        }

        public void SelectGame(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (_gameSkin == name)
                return;

            _gameSkin = name;
            OnGameSkinChanged?.Invoke(_gameSkin);
        }

        public void SelectField(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (_fieldSkin == name)
                return;

            _fieldSkin = name;

            OnFieldSkinChanged?.Invoke(_fieldSkin);
        }

        public void RewardMoney(int value)
        {
        }
    }
}