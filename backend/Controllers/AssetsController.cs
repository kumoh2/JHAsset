using Microsoft.AspNetCore.Mvc;
using JHAsset.Api.Repositories;
using JHAsset.Api.Models;

namespace JHAsset.Api.Repositories;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly AssetRepository _repo;
    public AssetsController(AssetRepository repo) => _repo = repo;

    [HttpGet]                                          // GET /api/assets
    public async Task<IActionResult> Get()
        => Ok(await _repo.GetAllAsync());

    [HttpPost]                                         // POST /api/assets
    public async Task<IActionResult> Post([FromBody] AssetCreate dto)
    {
        var id = await _repo.InsertAsync(dto);
        return Created($"/api/assets/{id}", new { id });
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Put(long id, [FromBody] AssetUpdate dto)
    {
        if (id != dto.Id) return BadRequest("ID mismatch");
        var rows = await _repo.UpdateAsync(dto);
        return rows > 0 ? NoContent() : NotFound();
    }

    [HttpDelete("{id:long}")]                          // DELETE /api/assets/3
    public async Task<IActionResult> Delete(long id)
        => await _repo.DeleteAsync(id) > 0 ? NoContent() : NotFound();
}
