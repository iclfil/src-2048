using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Store
{
    public class StoreProductView : MonoBehaviour
    {
        public event Action<StoreProductView> OnClickProductAction;
        public event Action<StoreProductView> OnClickBuyButton;

        [SerializeField] private Image _icon;
        [SerializeField] private Image _select;
        [SerializeField] private Image _lock;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _buyButton;
        private string _name;
        private int _price;
        public string Name => _name;

        public void Init(string name, int price, Sprite icon, bool isOpen, bool isPushashed)
        {
            _name = name;
            _price = price;
            _icon.sprite = icon;
            Purchashed(isPushashed);
            SetPrice(_price);
            Open(isOpen);
            AddListeners();
        }

        private void AddListeners()
        {
            _buyButton.onClick.AddListener(BuyButtonClickHandler);
        }

        private void RemoveListeners()
        {

        }

        public void OnSelectButtonClick()
        {
            Debug.Log("CLICK SELECT BUTTON");
            OnClickProductAction?.Invoke(this);
        }

        public void Free()
        {
            RemoveListeners();
        }

        public void SetPrice(int price)
        {
            _priceText.text = price.ToString();
        }

        private void BuyButtonClickHandler()
        {
            OnClickBuyButton?.Invoke(this);
        }

        public void Select(bool select)
        {
            _select.gameObject.SetActive(select);
        }

        public void Open(bool open)
        {
            _lock.gameObject.SetActive(!open);
        }

        public void Purchashed(bool purchased)
        {
            _buyButton.gameObject.SetActive(!purchased);
        }
    }
}