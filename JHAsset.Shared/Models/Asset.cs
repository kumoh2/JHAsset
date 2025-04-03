using System;

namespace JHAsset.Shared.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public AssetCategory Category { get; set; }
        public string SubCategory { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public string AssetNumber { get; set; } = string.Empty;
    }
}
