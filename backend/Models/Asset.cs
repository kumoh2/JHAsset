namespace JHAsset.Api.Models;

public class Asset
{
    public long     Id          { get; set; }
    public string   Name        { get; set; } = null!;
    public string   IpAddress   { get; set; } = null!;
    public int      Port        { get; set; }
    public string   Protocol    { get; set; } = "tcp";
    public string?  Description { get; set; }
    public bool     IsActive    { get; set; }
    public DateTime CreatedAt   { get; set; }
    public DateTime UpdatedAt   { get; set; }
}
