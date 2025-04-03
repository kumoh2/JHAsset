using Microsoft.EntityFrameworkCore;

namespace JHAsset.Shared.Data
{
    public class JHAssetContext : DbContext
    {
        public JHAssetContext(DbContextOptions<JHAssetContext> options)
            : base(options)
        {
        }

        public DbSet<Models.User> Users => Set<Models.User>();
        public DbSet<Models.Asset> Assets => Set<Models.Asset>();
    }
}
