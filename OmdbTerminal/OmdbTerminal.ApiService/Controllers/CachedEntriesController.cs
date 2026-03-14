using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using OmdbTerminal.ApiService.Data;
using OmdbTerminal.ApiService.Services;
using OmdbTerminal.Shared;

namespace OmdbTerminal.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CachedEntriesController(ICachedEntriesService cachedService) : ControllerBase
    {
        [HttpGet]
        [EnableQuery] // This single attribute turns this into a full OData endpoint
        public IQueryable<MovieEntity> Get()
        {
            // OData intercepts this IQueryable and applies the URL filters - including the expansion by default (design decision)
            return cachedService.GetQueryable().Include(m => m.Ratings);
        }

        // CRUD: READ (Specific Entry)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var movie = await cachedService.GetByIdAsync(id);
            return movie == null ? NotFound() : Ok(movie);
        }

        // CRUD: CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovieDetails movie)
        {
            var success = await cachedService.CreateAsync(movie.ToEntity());
            if (!success) return Conflict($"A movie with this IMDb ID {movie.ImdbId} already exists in the cache");

            return CreatedAtAction(nameof(GetById), new { id = movie.ImdbId }, movie);
        }

        // CRUD: UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] MovieDetails updatedMovie)
        {
            if (id != updatedMovie.ImdbId) return BadRequest($"URL ID {id} does not match body ID {updatedMovie.ImdbId}");

            var success = await cachedService.UpdateAsync(id, updatedMovie.ToEntity());
            return success ? NoContent() : NotFound();
        }

        // CRUD: DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await cachedService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCache()
        {
            var deletedCount = await cachedService.ClearCacheAsync();
            return Ok(new ClearCacheResponse { Success = true, Message = $"Successfully cleared {deletedCount} cached entries" });
        }
    }
}
