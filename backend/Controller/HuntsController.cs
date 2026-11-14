using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Data;
using UltimateShinyDex.Api.Models;

namespace UltimateShinyDex.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HuntsController : ControllerBase
    {
        private readonly ShinyDbContext _context;

        public HuntsController(ShinyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Hunt>>> GetAll()
        {
            var hunts = await _context.Hunts
                .Include(hunt => hunt.Games)
                    .ThenInclude(game => game.Encounters)
                .Include(hunt => hunt.Targets)
                .Include(hunt => hunt.Phases)
                    .ThenInclude(phase => phase.FoundShiny)
                .OrderByDescending(hunt => hunt.StartedAt)
                .ToListAsync();

            return Ok(hunts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Hunt>> GetById(int id)
        {
            var hunt = await _context.Hunts
                .Include(hunt => hunt.Games)
                    .ThenInclude(game => game.Encounters)
                .Include(hunt => hunt.Targets)
                .Include(hunt => hunt.Phases)
                    .ThenInclude(phase => phase.FoundShiny)
                        .ThenInclude(shiny => shiny!.Marks)
                .FirstOrDefaultAsync(
                    hunt => hunt.Id == id
                );

            if (hunt == null)
            {
                return NotFound();
            }

            return Ok(hunt);
        }

        [HttpPost]
        public async Task<ActionResult<Hunt>> Create(Hunt hunt)
        {
            hunt.Id = 0;
            hunt.Status = "Active";
            hunt.CurrentPhaseNumber = 1;
            hunt.CurrentEncounters = 0;
            hunt.TotalEncounters = 0;
            hunt.StartedAt = DateTime.UtcNow;
            hunt.CompletedAt = null;

            foreach (var game in hunt.Games)
            {
                game.Id = 0;
                game.HuntId = 0;

                foreach (var encounter in game.Encounters)
                {
                    encounter.Id = 0;
                    encounter.HuntGameId = 0;
                }
            }

            foreach (var target in hunt.Targets)
            {
                target.Id = 0;
                target.HuntId = 0;
            }

            hunt.Phases.Clear();

            _context.Hunts.Add(hunt);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = hunt.Id },
                hunt
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateHuntRequest request
        )
        {
            var hunt = await _context.Hunts
                .Include(hunt => hunt.Games)
                    .ThenInclude(game => game.Encounters)
                .Include(hunt => hunt.Targets)
                .FirstOrDefaultAsync(
                    hunt => hunt.Id == id
                );

            if (hunt == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(
                    "A hunt name is required."
                );
            }

            if (
                request.UsesCounter &&
                request.CounterStep < 1
            )
            {
                return BadRequest(
                    "Counter step must be at least 1."
                );
            }

            if (request.Games.Count < 1)
            {
                return BadRequest(
                    "A hunt needs at least one game."
                );
            }

            hunt.Name =
                request.Name.Trim();

            hunt.UsesCounter =
                request.UsesCounter;

            hunt.CounterStep =
                request.CounterStep < 1
                    ? 1
                    : request.CounterStep;

            hunt.Notes =
                string.IsNullOrWhiteSpace(
                    request.Notes
                )
                    ? null
                    : request.Notes.Trim();

            _context.HuntGames.RemoveRange(
                hunt.Games
            );

            hunt.Games.Clear();

            foreach (var gameRequest in request.Games)
            {
                if (
                    string.IsNullOrWhiteSpace(
                        gameRequest.Game
                    )
                )
                {
                    return BadRequest(
                        "Every game instance needs a game."
                    );
                }

                var game = new HuntGame
                {
                    Game =
                        gameRequest.Game,

                    Label =
                        string.IsNullOrWhiteSpace(
                            gameRequest.Label
                        )
                            ? null
                            : gameRequest.Label.Trim(),

                    ShinyOdds =
                        gameRequest.ShinyOdds
                };

                foreach (
                    var encounterRequest
                    in gameRequest.Encounters
                )
                {
                    if (
                        string.IsNullOrWhiteSpace(
                            encounterRequest.Pokemon
                        )
                    )
                    {
                        return BadRequest(
                            "Every encounter needs a Pokémon."
                        );
                    }

                    game.Encounters.Add(
                        new HuntEncounter
                        {
                            Pokemon =
                                encounterRequest
                                    .Pokemon
                                    .Trim(),

                            Form =
                                string.IsNullOrWhiteSpace(
                                    encounterRequest.Form
                                )
                                    ? null
                                    : encounterRequest
                                        .Form
                                        .Trim(),

                            EncounterPercentage =
                                encounterRequest
                                    .EncounterPercentage
                        }
                    );
                }

                hunt.Games.Add(game);
            }

            _context.HuntTargets.RemoveRange(
                hunt.Targets
            );

            hunt.Targets.Clear();

            foreach (
                var targetRequest
                in request.Targets
            )
            {
                if (
                    string.IsNullOrWhiteSpace(
                        targetRequest.Pokemon
                    )
                )
                {
                    return BadRequest(
                        "Every target needs a Pokémon."
                    );
                }

                hunt.Targets.Add(
                    new HuntTarget
                    {
                        Pokemon =
                            targetRequest
                                .Pokemon
                                .Trim(),

                        Form =
                            string.IsNullOrWhiteSpace(
                                targetRequest.Form
                            )
                                ? null
                                : targetRequest
                                    .Form
                                    .Trim(),

                        Gender =
                            string.IsNullOrWhiteSpace(
                                targetRequest.Gender
                            )
                                ? null
                                : targetRequest.Gender,

                        Nature =
                            string.IsNullOrWhiteSpace(
                                targetRequest.Nature
                            )
                                ? null
                                : targetRequest.Nature,

                        IsAlpha =
                            targetRequest.IsAlpha,

                        Mark =
                            string.IsNullOrWhiteSpace(
                                targetRequest.Mark
                            )
                                ? null
                                : targetRequest
                                    .Mark
                                    .Trim()
                    }
                );
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hunt = await _context.Hunts
                .FirstOrDefaultAsync(
                    hunt => hunt.Id == id
                );

            if (hunt == null)
            {
                return NotFound();
            }

            _context.Hunts.Remove(hunt);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id:int}/counter")]
        public async Task<IActionResult> AdjustCounter(
            int id,
            AdjustCounterRequest request
        )
        {
            var hunt = await _context.Hunts
                .FindAsync(id);

            if (hunt == null)
            {
                return NotFound();
            }

            if (!hunt.UsesCounter)
            {
                return BadRequest(
                    "This hunt does not use an encounter counter."
                );
            }

            if (request.Amount == 0)
            {
                return BadRequest(
                    "Counter adjustment cannot be 0."
                );
            }

            if (request.Amount > 0)
            {
                hunt.CurrentEncounters +=
                    request.Amount;

                hunt.TotalEncounters +=
                    request.Amount;
            }
            else
            {
                var requestedDecrease =
                    Math.Abs(request.Amount);

                var actualDecrease =
                    Math.Min(
                        requestedDecrease,
                        hunt.CurrentEncounters
                    );

                hunt.CurrentEncounters -=
                    actualDecrease;

                hunt.TotalEncounters =
                    Math.Max(
                        0,
                        hunt.TotalEncounters -
                        actualDecrease
                    );
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                currentEncounters =
                    hunt.CurrentEncounters,

                totalEncounters =
                    hunt.TotalEncounters
            });
        }

        [HttpPut("{id:int}/counter-step")]
        public async Task<IActionResult> UpdateCounterStep(
            int id,
            UpdateCounterStepRequest request
        )
        {
            var hunt = await _context.Hunts
                .FindAsync(id);

            if (hunt == null)
            {
                return NotFound();
            }

            if (request.Step < 1)
            {
                return BadRequest(
                    "Counter step must be at least 1."
                );
            }

            hunt.CounterStep = request.Step;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                counterStep =
                    hunt.CounterStep
            });
        }

        [HttpPost("{id:int}/shiny-found")]
        public async Task<IActionResult> ShinyFound(
            int id,
            ShinyFoundRequest request
        )
        {
            var hunt = await _context.Hunts
                .Include(hunt => hunt.Games)
                .FirstOrDefaultAsync(
                    hunt => hunt.Id == id
                );

            if (hunt == null)
            {
                return NotFound();
            }

            if (hunt.Status == "Completed")
            {
                return BadRequest(
                    "This hunt has already been completed."
                );
            }

            var huntGame = hunt.Games
                .FirstOrDefault(
                    game =>
                        game.Id ==
                        request.HuntGameId
                );

            if (huntGame == null)
            {
                return BadRequest(
                    "The selected game does not belong to this hunt."
                );
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.Shiny.Pokemon
                )
            )
            {
                return BadRequest(
                    "A Pokémon is required."
                );
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.Shiny.Nature
                )
            )
            {
                return BadRequest(
                    "A nature is required."
                );
            }

            if (
                string.IsNullOrWhiteSpace(
                    request.Shiny.Ball
                )
            )
            {
                return BadRequest(
                    "A Poké Ball is required."
                );
            }

            var shiny = new Shiny
            {
                Pokemon =
                    request.Shiny.Pokemon,

                Nickname =
                    request.Shiny.Nickname,

                Nature =
                    request.Shiny.Nature,

                Game =
                    huntGame.Game,

                Ball =
                    request.Shiny.Ball,

                Method =
                    request.Shiny.Method,

                Encounters =
                    hunt.UsesCounter
                        ? hunt.CurrentEncounters
                        : null,

                IsAlpha =
                    request.Shiny.IsAlpha,

                Gender =
                    request.Shiny.Gender,

                Form =
                    request.Shiny.Form,

                CustomSpritePath = null
            };

            foreach (
                var mark
                in request.Shiny.Marks
            )
            {
                shiny.Marks.Add(
                    new ShinyMark
                    {
                        MarkName =
                            mark.MarkName
                    }
                );
            }

            var phase = new HuntPhase
            {
                HuntId =
                    hunt.Id,

                PhaseNumber =
                    hunt.CurrentPhaseNumber,

                Encounters =
                    hunt.UsesCounter
                        ? hunt.CurrentEncounters
                        : null,

                FoundShiny =
                    shiny,

                WasTarget =
                    request.WasTarget,

                CompletedAt =
                    DateTime.UtcNow
            };

            _context.HuntPhases.Add(
                phase
            );

            if (request.ContinueHunt)
            {
                hunt.CurrentPhaseNumber++;
                hunt.CurrentEncounters = 0;
            }
            else
            {
                hunt.Status = "Completed";

                hunt.CompletedAt =
                    DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                shinyId =
                    shiny.Id,

                phaseId =
                    phase.Id,

                huntId =
                    hunt.Id,

                huntStatus =
                    hunt.Status,

                currentPhaseNumber =
                    hunt.CurrentPhaseNumber,

                currentEncounters =
                    hunt.CurrentEncounters,

                totalEncounters =
                    hunt.TotalEncounters
            });
        }
    }

    public class AdjustCounterRequest
    {
        public int Amount { get; set; }
    }

    public class UpdateCounterStepRequest
    {
        public int Step { get; set; }
    }

    public class ShinyFoundRequest
    {
        public bool WasTarget { get; set; }

        public bool ContinueHunt { get; set; }

        public int HuntGameId { get; set; }

        public ShinyFoundShinyRequest Shiny { get; set; }
            = new();
    }

    public class ShinyFoundShinyRequest
    {
        public string Pokemon { get; set; }
            = string.Empty;

        public string? Nickname { get; set; }

        public string Nature { get; set; }
            = string.Empty;

        public string Ball { get; set; }
            = string.Empty;

        public string Method { get; set; }
            = string.Empty;

        public bool IsAlpha { get; set; }

        public string? Gender { get; set; }

        public string? Form { get; set; }

        public List<ShinyFoundMarkRequest> Marks { get; set; }
            = new();
    }

    public class ShinyFoundMarkRequest
    {
        public string MarkName { get; set; }
            = string.Empty;
    }

    public class UpdateHuntRequest
    {
        public string Name { get; set; }
            = string.Empty;

        public bool UsesCounter { get; set; }

        public int CounterStep { get; set; } = 1;

        public string? Notes { get; set; }

        public List<UpdateHuntGameRequest> Games { get; set; }
            = new();

        public List<UpdateHuntTargetRequest> Targets { get; set; }
            = new();
    }

    public class UpdateHuntGameRequest
    {
        public string Game { get; set; }
            = string.Empty;

        public string? Label { get; set; }

        public int? ShinyOdds { get; set; }

        public List<UpdateHuntEncounterRequest> Encounters { get; set; }
            = new();
    }

    public class UpdateHuntEncounterRequest
    {
        public string Pokemon { get; set; }
            = string.Empty;

        public string? Form { get; set; }

        public decimal? EncounterPercentage { get; set; }
    }

    public class UpdateHuntTargetRequest
    {
        public string Pokemon { get; set; }
            = string.Empty;

        public string? Form { get; set; }

        public string? Gender { get; set; }

        public string? Nature { get; set; }

        public bool? IsAlpha { get; set; }

        public string? Mark { get; set; }
    }
}