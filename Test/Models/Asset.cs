namespace Test.Models
{
    public enum AssetType { Server, Workstation, Laptop, Network, Printer, Mobile, Software, Other }
    public enum AssetStatus { InStock, InUse, UnderMaintenance, Retired, Disposed }
    public class Asset
    {
        public int Id { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public AssetType Type { get; set; } = AssetType.Other;
        public AssetStatus Status { get; set; } = AssetStatus.InStock;
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Location { get; set; }
        public string? AssignedTo { get; set; }
        public string? Department { get; set; }
        public string? IpAddress { get; set; }
        public string? OperatingSystem { get; set; }
        public decimal? PurchaseCost { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
    }
}
