using System;
using System.Collections.Generic;
using Assets.Scripts.Store;
using Markins.Runtime.Game.Utils;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game.Storage.Models
{
    public class OnMoneyChangedSignal : Signal<int, int> { }
    public class OnSelectEffectSkinChangedSignal : Signal<string> { }
    public class OnSelectChipSkinChangedSignal : Signal<string> { }
    public class OnSelectGameSkinChangedSignal : Signal<string> { }
    public class OnSelectFieldSkinChangedSignal : Signal<string> { }


    //по сути контроллер не нужен, StoreController должен работать с двумя моделями.
    [Serializable]
    public class InventoryController : Singleton<InventoryController>
    {
        private GameStorage _storage;
        [SerializeField]
        private InventoryModel _model;

        public IEnumerable<string> Effects => _model.Effects;
        public IEnumerable<string> Chips => _model.ChipSkins;

        public IEnumerable<string> Fields => _model.Fields;
        public IEnumerable<string> GameThemes => _model.GameThemes;


        public void Init(InventoryModel model)
        {
            _model = model;
            _model.OnMoneyChanged += MoneyChangedHandler;

            _model.OnChipSkinChanged += ChipSkinChangeHandler;
            _model.OnFieldSkinChanged += FieldSkinChangeHandler;
            _model.OnEffectSkinChanged += EffectSkinChangeHandler;
            _model.OnGameSkinChanged += GameSkinChangeHandler;
        }

        private void GameSkinChangeHandler(string nameSkin)
        {
            Signals.Get<OnSelectGameSkinChangedSignal>().Dispatch(nameSkin);
        }

        private void EffectSkinChangeHandler(string nameSkin)
        {
            Signals.Get<OnSelectEffectSkinChangedSignal>().Dispatch(nameSkin);
        }

        private void FieldSkinChangeHandler(string nameSkin)
        {
            Signals.Get<OnSelectFieldSkinChangedSignal>().Dispatch(nameSkin);
        }

        private void ChipSkinChangeHandler(string nameSkin)
        {
            Signals.Get<OnSelectChipSkinChangedSignal>().Dispatch(nameSkin);
        }

        private void MoneyChangedHandler(int old, int current)
        {
            Signals.Get<OnMoneyChangedSignal>().Dispatch(old, current);

        }

        public void Free()
        {

        }

        public bool SpendMoney(int value)
        {
            if (value > _model.Money)
                return false;

            var oldValue = _model.Money;
            _model.SpendMoney(value);

            return true;
        }

        public void AddItem(ProductType type, string name)
        {
            if (type == ProductType.ChipSkin)
                _model.AddChip(name);
            if (type == ProductType.Effects)
                _model.AddEffect(name);
            if (type == ProductType.FieldSkin)
                _model.AddField(name);
            if (type == ProductType.GameSkin)
                _model.AddGame(name);
        }

        public void SelectItem(ProductType type, string name)
        {
            switch (type)
            {
                case ProductType.ChipSkin:
                    _model.SelectChip(name);
                    break;
                case ProductType.Effects:
                    _model.SelectEffect(name);
                    break;
                case ProductType.FieldSkin:
                    _model.SelectField(name);
                    break;
                case ProductType.GameSkin:
                    _model.SelectGame(name);
                    break;
            }
        }

        public void AddMoney(int value)
        {
            _model.AddMoney(value);
        }

        //FIX BUGS
        public void RewardMoney(int value)
        {
            _model.RewardMoney(value);
        }
    }
}