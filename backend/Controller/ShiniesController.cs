using Microsoft.AspNetCore.Mvc;
using UltimateShinyDex.Api.Models;

namespace UltimateShinyDex.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiniesController : ControllerBase
    {
        private static readonly List<Shiny> Shinies = new()
        {
            new Shiny
            {
                Id = 1,
                Pokemon = "Gogoat",
                Nickname = "Dame",
                Nature = "Careful",
                Game = "Pokemon Legends Z-A",
                Ball = "Friend Ball"
            },
            new Shiny
            {
                Id = 2,
                Pokemon = "Silvally",
                Nickname = "Riley",
                Nature = "Hardy",
                Game = "Pokemon Ultra Sun",
                Ball = "Poke Ball"
            },
            new Shiny
            {
                Id = 3,
                Pokemon = "Skeledirge",
                Nickname = "Fragola",
                Nature = "Bold",
                Game = "Pokemon Violet",
                Ball = "Love Ball"
            }
        };

        [HttpGet]
        public ActionResult<List<Shiny>> GetAll()
        {
            return Ok(Shinies);
        }
    }
}