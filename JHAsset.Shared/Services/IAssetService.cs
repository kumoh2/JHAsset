// IAssetService.cs
namespace JHAsset.Shared.Services
{
    public interface IAssetService
    {
        Task<List<Models.Asset>> GetAllAsync();
        Task AddAsync(Models.Asset asset);
        // 필요 시 Edit, Remove, GetById 등
    }
}