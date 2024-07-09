using System;
using System.Collections.Generic;
using Assets.Scripts.Store;

namespace Markins.Runtime.Game.Store
{
    [Serializable]
    public partial class StoreModel
    {
        public bool HasReward { get; private set; }
        public List<ProductModel> Products { get; private set; } = new();

        public void AddProduct(ProductModel product)
        {
            Products.Add(product);
        }

        public void RemoveProduct(ProductModel product)
        {
            Products.Remove(product);
        }

        public void OpenProduct(string name)
        {
            var product = GetProduct(name);
            product.IsOpen = true;
            product.IsPurchased = true;
        }

        public void SelectProduct(string name)
        {
            var product = GetProduct(name);
            product.IsSelected = true;
        }

        public void DeselectAllByType(ProductType type)
        {
            Products.ForEach(x =>
            {
                if (x.Type == type)
                {
                    x.IsSelected = false;
                }
            });
        }

        public ProductModel GetProduct(string name)
        {
            var product = Products.Find(x => x.Name == name);
            return product;
        }

        public bool IsPurchashedProduct(string nameProduct)
        {
            return Products.Find(x => x.Name == nameProduct).IsPurchased;
        }

        public void BuyProduct(string nameProduct)
        {
            var product = GetProduct(nameProduct);
            product.IsOpen = true;
            product.IsPurchased = true;
        }
    }
}