using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Markins.Runtime.Game.Store;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Markins.Runtime.Game
{
    [CreateAssetMenu(fileName = "Store Config", menuName = "Game/Store/Create Store")]
    public class StoreConfig : SerializedScriptableObject
    {
        [SerializeField] private List<StoreProductConfig> _productConfigs;
        public List<StoreProductConfig> ProductConfigs => _productConfigs;

        public StoreModel CloneStoreModel()
        {
            var clone = new StoreModel();
            foreach (var productConfig in _productConfigs)
            {
                clone.AddProduct(productConfig.CloneProductModel());
            }

            return clone;
        }
    }

}