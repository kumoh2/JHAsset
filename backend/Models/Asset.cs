namespace JHAsset.Api.Models;

public class Asset
{
    public long     id          { get; set; }
    public string   name        { get; set; } = null!;
    public string   ip_address   { get; set; } = null!;
    public int      port        { get; set; }
    public string   protocol    { get; set; } = "tcp";
    public string?  description { get; set; }
    public bool     is_active    { get; set; }
    public DateTime created_at   { get; set; }
    public DateTime updated_at   { get; set; }
}
