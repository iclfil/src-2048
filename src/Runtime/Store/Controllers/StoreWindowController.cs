using System.Collections.Generic;
using Markins.Runtime.Game.Storage.Models;
using Markins.Runtime.Game;
using Assets.Scripts.Controllers;
using Assets.Scripts.Store;
using Assets.Scripts.Store.MySignals;
using deVoid.UIFramework;
using Markins.Runtime.Game.GUI.Views;
using Supyrb;
using UnityEngine;
using UnityEngine.UI;

namespace Markins.Runtime.Game.Store
{
    public class StoreWindowController : AWindowController
    {
        [SerializeField] private StoreConfig _config;
        [SerializeField] private ProductType _typePanelOnStartStore = ProductType.ChipSkin;
        [SerializeField] private WalletView _walletView;
        [SerializeField] private BuyButtonView _buttonBuy;
        [SerializeField] private Button _buttonClose;

        [SerializeField] private List<ProductsPanel> _productPanels = new();
        [SerializeField] private ProductsPanel _currentProductsPanel;

        [SerializeField] private Button ChipsPanelButton;
        [SerializeField] private Button FieldPanelButton;
        [SerializeField] private Button EffectsPanelButton;
        [SerializeField] private Button GamePanelButton;


        protected override void Awake()
        {
            base.Awake();
            HideAllPanels();
        }

        protected override void AddListeners()
        {
            _buttonClose.onClick.AddListener(CloseStore);
            Signals.Get<OnMoneyChangedSignal>().AddListener(MoneyChangeHandler);
            Signals.Get<CreateStoreWindowSignal>().AddListener(OnCreateStore);
            Signals.Get<UpdateStoreWindowSignal>().AddListener(OnUpdateStore);

        }

        protected override void RemoveListeners()
        {
            Signals.Get<OnMoneyChangedSignal>().RemoveListener(MoneyChangeHandler);
            Signals.Get<CreateStoreWindowSignal>().RemoveListener(OnCreateStore);
            Signals.Get<UpdateStoreWindowSignal>().RemoveListener(OnUpdateStore);
        }


        private void MoneyChangeHandler(int old, int newValue)
        {
            _walletView.ChangeMoney(old, newValue);
        }


        private void CloseStore()
        {
            Signals.Get<OnClickCloseStoreButton>().Dispatch();
        }

        private void OnCreateStore(StoreModel storeModel)
        {
            foreach (var productModel in storeModel.Products)
            {
                var config = _config.ProductConfigs.Find(x => x.Name == productModel.Name);
                CreateProduct(config, productModel);
            }

            ShowPanel(_typePanelOnStartStore);
        }

        private void OnUpdateStore(StoreModel store)
        {
            var currentProducts = store.Products.FindAll(x => x.Type == _currentProductsPanel.ProductType);

            foreach (var productModel in currentProducts)
            {
                _currentProductsPanel.UpdateProduct(productModel);
            }
        }

        private void HideAllPanels()
        {
            _productPanels.ForEach(x => x.Hide());
        }

        public void SwitchPanel(ProductType selectPanel)
        {
            HideAllPanels();
            ShowPanel(selectPanel);
        }

        private void ShowPanel(ProductType typePanel)
        {
            _currentProductsPanel = _productPanels.Find(x => x.ProductType == typePanel);
            _currentProductsPanel.Show();
        }

        private void HidePanel(ProductType typePanel)
        {
            _currentProductsPanel = _productPanels.Find(x => x.ProductType == typePanel);
            _currentProductsPanel.Hide();
        }

        private void CreateProduct(StoreProductConfig config, ProductModel productModel)
        {
            var panel = _productPanels.Find(x => x.ProductType == productModel.Type);
            panel.CreateProduct(config, productModel);
        }
    }
}