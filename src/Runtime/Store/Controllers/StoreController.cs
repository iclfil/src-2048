using System;
using Markins.Runtime.Game.Storage;
using Markins.Runtime.Game.Storage.Models;
using Assets.Scripts.Store.MySignals;
using Markins.Runtime.Game.GUI.MySignals;
using Markins.Runtime.Game.Utils;
using Supyrb;
using UnityEngine;

namespace Markins.Runtime.Game.Store
{
    public class OnSelectProductSignal : Signal<string> { }
    public class OnBuyProductSignal : Signal<string> { }
    public class CreateStoreWindowSignal : Signal<StoreModel> { }
    public class UpdateStoreWindowSignal : Signal<StoreModel> { }

    public class StoreController : Singleton<StoreController>
    {
        private StoreModel _model;
        public StoreModel Model => _model;

        private InventoryController _inventoryController;

        public void Init(StoreModel model, InventoryController inventoryController)
        {
            _model = model;
            _inventoryController = inventoryController;

            Signals.Get<OnClickOpenStoreSignal>().AddListener(Open);
            Signals.Get<OnClickCloseStoreButton>().AddListener(Close);

            Signals.Get<OnBuyProductSignal>().AddListener(BuyProductHandler);
            Signals.Get<OnSelectProductSignal>().AddListener(SelectProductHandler);
         
        }

        public void Free()
        {
            Signals.Get<OnClickOpenStoreSignal>().RemoveListener(Open);
            Signals.Get<OnClickCloseStoreButton>().RemoveListener(Close);
        }

        public void CreateStore()
        {

            foreach (var data in _inventoryController.Chips)
            {
                _model.OpenProduct(data);
            }

            foreach (var data in _inventoryController.Effects)
            {
                _model.OpenProduct(data);
            }

            foreach (var data in _inventoryController.Fields)
            {
                _model.OpenProduct(data);
            }

            foreach (var data in _inventoryController.GameThemes)
            {
                _model.OpenProduct(data);
            }

            Signals.Get<CreateStoreWindowSignal>().Dispatch(_model);
        }

        public void Open()
        {
            Signals.Get<OnStoreOpenedSignal>().Dispatch();
            UIManager.instance.Frame.OpenWindow(ScreenIds.StoreWindow);
        }

        public void Close()
        {
            Signals.Get<OnStoreClosedSignal>().Dispatch();
            UIManager.instance.Frame.CloseCurrentWindow();
        }

        private void SelectProductHandler(string nameProduct)
        {
            var product = _model.Products.Find(x => x.Name == nameProduct);

            if (_model.IsPurchashedProduct(nameProduct))
            {
                _inventoryController.SelectItem(product.Type, nameProduct);
                _model.DeselectAllByType(product.Type);
                _model.SelectProduct(nameProduct);
                Signals.Get<UpdateStoreWindowSignal>().Dispatch(_model);
            }
        }

        private void BuyProductHandler(string nameProduct)
        {
            Debug.Log("But Prroduct But name" + nameProduct);
            //если продукт уже куплен
            if (_model.IsPurchashedProduct(nameProduct))
            {
                Debug.Log("Товар уже куплен");
                return;
            }

            var product = _model.GetProduct(nameProduct);

            if (_inventoryController.SpendMoney(product.Price))
            {
                _inventoryController.AddItem(product.Type, product.Name);
                _model.BuyProduct(nameProduct);
                Signals.Get<UpdateStoreWindowSignal>().Dispatch(_model);
            }
        }
    }
}