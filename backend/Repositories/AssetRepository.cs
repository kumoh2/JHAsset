using System.Data;
using Dapper;
using JHAsset.Api.Models;

namespace JHAsset.Api.Repositories;

public class AssetRepository
{
    private readonly IDbConnection _db;
    public AssetRepository(IDbConnection db) => _db = db;

    public Task<IEnumerable<Asset>> GetAllAsync() =>
        _db.QueryAsync<Asset>("SELECT * FROM asset ORDER BY id");

    public Task<Asset?> GetByIdAsync(long id) =>
        _db.QuerySingleOrDefaultAsync<Asset>(
            "SELECT * FROM asset WHERE id = @id", new { id });

    public async Task<long> InsertAsync(AssetCreate dto)
    {
        const string sql = @"
          INSERT INTO asset (name, ip_address, port, protocol, description)
          VALUES (@Name, @Ip, @Port, @Protocol, @Description)
          RETURNING id;";
        return await _db.ExecuteScalarAsync<long>(sql, new {
            dto.Name, Ip = dto.Ip_Address, dto.Port,
            dto.Protocol, dto.Description
        });
    }

    public Task<int> DeleteAsync(long id) =>
        _db.ExecuteAsync("DELETE FROM asset WHERE id = @id", new { id });
}

public record AssetCreate(
    string Name,
    string Ip_Address,
    int    Port,
    string Protocol = "tcp",
    string? Description = null);
