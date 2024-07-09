using Assets.Scripts.Store;

namespace Markins.Runtime.Game.Store
{
    public class ProductModel
    {
        public string Name { get; set; }
        public ProductType Type { get; set; }
        public int Price { get; set; }
        public bool IsPurchased { get; set; }
        public bool IsOpen { get; set; }
        public bool IsSelected { get; set; }
    }
}