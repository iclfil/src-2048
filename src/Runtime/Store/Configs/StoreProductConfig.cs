using Assets.Scripts.Store;
using Markins.Runtime.Game.Store;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    [CreateAssetMenu(fileName = "StoreProductConfig", menuName = "Game/Store/Create StoreProductConfig")]
    public class StoreProductConfig : SerializedScriptableObject
    {
        [SerializeField] private StoreProductView _storeProductPrefabView;
        [SerializeField] private string _name;
        [SerializeField] private int _price;
        [SerializeField] private ProductType _productType;
        [SerializeField] private bool _isOpen;
        [SerializeField] private bool _isPurchased;


        [PreviewField(64)] public Sprite _icon;

        public Sprite Icon => _icon;
        public StoreProductView PrefabView => _storeProductPrefabView;
        public string Name => _name;
        public int Price => _price;

        public ProductModel CloneProductModel()
        {
            return new ProductModel()
            {
                IsOpen = _isOpen,
                IsPurchased = _isPurchased,
                Name = _name,
                Price = _price,
                Type = _productType
            };
        }
    }
}