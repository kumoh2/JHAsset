// AssetService.cs
using JHAsset.Shared.Data;
using JHAsset.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace JHAsset.Shared.Services
{
    public class AssetService : IAssetService
    {
        private readonly IDbContextFactory<JHAssetContext> _contextFactory;

        public AssetService(IDbContextFactory<JHAssetContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Asset>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Assets.ToListAsync();
        }

        public async Task AddAsync(Asset asset)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Assets.Add(asset);
            await context.SaveChangesAsync();
        }
    }
}
