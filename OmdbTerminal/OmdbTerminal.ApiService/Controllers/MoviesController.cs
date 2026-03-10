using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OmdbTerminal.ApiService.Services;
using OmdbTerminal.Shared;
using static System.Net.WebRequestMethods;

namespace OmdbTerminal.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    /// <summary>
    /// Searches the external OMDB database for movies matching a title
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string title, [FromQuery] int page = 1)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest("Search title cannot be empty");
        }

        var result = await movieService.SearchAsync(title, page);
        return Ok(result);
    }

    /// <summary>
    /// Fetches the full details of a specific movie by its IMDB ID
    /// </summary>
    [HttpGet("details/{id}")]
    [ProducesResponseType(typeof(MovieDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetails(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Search title cannot be empty");
        }

        var movie = await movieService.GetDetailsAsync(id);

        return movie == null
            ? NotFound($"Movie with ID {id} not found.")
            : Ok(movie);
    }
}