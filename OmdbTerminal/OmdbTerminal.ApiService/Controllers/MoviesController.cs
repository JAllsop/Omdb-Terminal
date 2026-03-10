using Microsoft.AspNetCore.Mvc;

namespace OmdbTerminal.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        [HttpGet(Name = "GetMovies")]
        public string Get()
        {
            return "Hello, World!";
        }
    }
}
