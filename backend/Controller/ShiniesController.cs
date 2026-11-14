using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Data;
using UltimateShinyDex.Api.Models;
using UltimateShinyDex.Api.Models.Backup;

namespace UltimateShinyDex.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiniesController : ControllerBase
    {
        private readonly ShinyDbContext _context;

        public ShiniesController(ShinyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Shiny>>> GetAll()
        {
            var shinies = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .ToListAsync();

            return Ok(shinies);
        }

[HttpGet("export")]
public async Task<ActionResult<ShinyBackup>> ExportCollection()
{
    var shinies = await _context.Shinies
        .Include(shiny => shiny.Marks)
        .ToListAsync();

    var disabledDexSlots = await _context.DisabledDexSlots
        .OrderBy(slot => slot.DexNumber)
        .ToListAsync();

    var hunts = await _context.Hunts
        .Include(hunt => hunt.Games)
            .ThenInclude(game => game.Encounters)
        .Include(hunt => hunt.Targets)
        .Include(hunt => hunt.Phases)
            .ThenInclude(phase => phase.FoundShiny)
        .OrderBy(hunt => hunt.StartedAt)
        .ToListAsync();

    var backup = new ShinyBackup
    {
        Version = 3,
        ExportedAt = DateTime.UtcNow,

        Shinies = shinies
            .Select(shiny => new ShinyBackupItem
            {
                SourceId = shiny.Id,

                Pokemon = shiny.Pokemon,
                Nickname = shiny.Nickname,
                Nature = shiny.Nature,
                Game = shiny.Game,
                Ball = shiny.Ball,
                Method = shiny.Method,
                Encounters = shiny.Encounters,
                IsAlpha = shiny.IsAlpha,
                Gender = shiny.Gender,
                Form = shiny.Form,

                Marks = shiny.Marks
                    .Select(mark => mark.MarkName)
                    .ToList()
            })
            .ToList(),

        DisabledDexSlots = disabledDexSlots
            .Select(slot => new DisabledDexSlotBackupItem
            {
                DexNumber = slot.DexNumber,
                Pokemon = slot.Pokemon,
                Form = slot.Form,
                Gender = slot.Gender
            })
            .ToList(),

        Hunts = hunts
            .Select(hunt => new HuntBackupItem
            {
                Name = hunt.Name,
                Status = hunt.Status,
                UsesCounter = hunt.UsesCounter,
                CounterStep = hunt.CounterStep,
                CurrentEncounters = hunt.CurrentEncounters,
                TotalEncounters = hunt.TotalEncounters,
                CurrentPhaseNumber = hunt.CurrentPhaseNumber,
                StartedAt = hunt.StartedAt,
                CompletedAt = hunt.CompletedAt,
                Notes = hunt.Notes,

                Games = hunt.Games
                    .Select(game => new HuntGameBackupItem
                    {
                        Game = game.Game,
                        Label = game.Label,
                        ShinyOdds = game.ShinyOdds,

                        Encounters = game.Encounters
                            .Select(encounter => new HuntEncounterBackupItem
                            {
                                Pokemon = encounter.Pokemon,
                                Form = encounter.Form,
                                EncounterPercentage =
                                    encounter.EncounterPercentage
                            })
                            .ToList()
                    })
                    .ToList(),

                Targets = hunt.Targets
                    .Select(target => new HuntTargetBackupItem
                    {
                        Pokemon = target.Pokemon,
                        Form = target.Form,
                        Gender = target.Gender,
                        Nature = target.Nature,
                        IsAlpha = target.IsAlpha,
                        Mark = target.Mark
                    })
                    .ToList(),

                Phases = hunt.Phases
                    .OrderBy(phase => phase.PhaseNumber)
                    .Select(phase => new HuntPhaseBackupItem
                    {
                        PhaseNumber = phase.PhaseNumber,
                        Encounters = phase.Encounters,

                        FoundShinySourceId =
                            phase.FoundShinyId,

                        WasTarget = phase.WasTarget,
                        CompletedAt = phase.CompletedAt
                    })
                    .ToList()
            })
            .ToList()
    };

    return Ok(backup);
}

        [HttpPost("import")]
public async Task<IActionResult> ImportCollection(
    ShinyBackup backup
)
{
    if (
        backup == null ||
        backup.Shinies == null
    )
    {
        return BadRequest(
            "Invalid backup file."
        );
    }

    await using var transaction =
        await _context.Database
            .BeginTransactionAsync();

    try
    {
        // Version 3 contains the complete hunt history.
        //
        // Therefore, importing a v3 backup replaces the
        // current hunts with the hunts from the backup.
        if (backup.Version >= 3)
        {
            var existingHunts =
                await _context.Hunts
                    .ToListAsync();

            _context.Hunts.RemoveRange(
                existingHunts
            );

            // Hunt -> Games / Encounters / Targets / Phases
            // are cascade relationships.
            await _context.SaveChangesAsync();
        }

        // Remove the current shiny collection.
        var existingShinies =
            await _context.Shinies
                .Include(shiny => shiny.Marks)
                .ToListAsync();

        _context.Shinies.RemoveRange(
            existingShinies
        );

        await _context.SaveChangesAsync();

        // Maps old shiny IDs stored in the backup
        // to the newly created EF entities.
        var shinyMap =
            new Dictionary<int, Shiny>();

        foreach (
            var backupShiny
            in backup.Shinies
        )
        {
            var shiny = new Shiny
            {
                Pokemon =
                    backupShiny.Pokemon,

                Nickname =
                    backupShiny.Nickname,

                Nature =
                    backupShiny.Nature,

                Game =
                    backupShiny.Game,

                Ball =
                    backupShiny.Ball,

                Method =
                    backupShiny.Method,

                Encounters =
                    backupShiny.Encounters,

                IsAlpha =
                    backupShiny.IsAlpha,

                Gender =
                    backupShiny.Gender,

                Form =
                    backupShiny.Form,

                // Custom uploaded sprite files are not
                // currently included in JSON backups.
                CustomSpritePath = null
            };

            foreach (
                var markName
                in backupShiny.Marks
                    ?? new List<string>()
            )
            {
                shiny.Marks.Add(
                    new ShinyMark
                    {
                        MarkName =
                            markName
                    }
                );
            }

            _context.Shinies.Add(
                shiny
            );

            if (
                backup.Version >= 3 &&
                backupShiny.SourceId > 0
            )
            {
                shinyMap[
                    backupShiny.SourceId
                ] = shiny;
            }
        }

        // Version 2+ backups contain Customize Dex
        // preferences.
        //
        // Version 1 did not, so old backups leave
        // current Dex preferences unchanged.
        if (backup.Version >= 2)
        {
            var existingDisabledSlots =
                await _context
                    .DisabledDexSlots
                    .ToListAsync();

            _context
                .DisabledDexSlots
                .RemoveRange(
                    existingDisabledSlots
                );

            await _context.SaveChangesAsync();

            foreach (
                var backupSlot
                in backup.DisabledDexSlots
                    ?? new List<DisabledDexSlotBackupItem>()
            )
            {
                _context.DisabledDexSlots.Add(
                    new DisabledDexSlot
                    {
                        DexNumber =
                            backupSlot.DexNumber,

                        Pokemon =
                            backupSlot.Pokemon,

                        Form =
                            backupSlot.Form,

                        Gender =
                            backupSlot.Gender
                    }
                );
            }
        }

        // Version 3 backups also contain hunts.
        if (backup.Version >= 3)
        {
            foreach (
                var backupHunt
                in backup.Hunts
                    ?? new List<HuntBackupItem>()
            )
            {
                var hunt = new Hunt
                {
                    Name =
                        backupHunt.Name,

                    Status =
                        backupHunt.Status,

                    UsesCounter =
                        backupHunt.UsesCounter,

                    CounterStep =
                        backupHunt.CounterStep,

                    CurrentEncounters =
                        backupHunt.CurrentEncounters,

                    TotalEncounters =
                        backupHunt.TotalEncounters,

                    CurrentPhaseNumber =
                        backupHunt.CurrentPhaseNumber,

                    StartedAt =
                        backupHunt.StartedAt,

                    CompletedAt =
                        backupHunt.CompletedAt,

                    Notes =
                        backupHunt.Notes
                };

                foreach (
                    var backupGame
                    in backupHunt.Games
                        ?? new List<HuntGameBackupItem>()
                )
                {
                    var game =
                        new HuntGame
                        {
                            Game =
                                backupGame.Game,

                            Label =
                                backupGame.Label,

                            ShinyOdds =
                                backupGame.ShinyOdds
                        };

                    foreach (
                        var backupEncounter
                        in backupGame.Encounters
                            ?? new List<HuntEncounterBackupItem>()
                    )
                    {
                        game.Encounters.Add(
                            new HuntEncounter
                            {
                                Pokemon =
                                    backupEncounter.Pokemon,

                                Form =
                                    backupEncounter.Form,

                                EncounterPercentage =
                                    backupEncounter
                                        .EncounterPercentage
                            }
                        );
                    }

                    hunt.Games.Add(
                        game
                    );
                }

                foreach (
                    var backupTarget
                    in backupHunt.Targets
                        ?? new List<HuntTargetBackupItem>()
                )
                {
                    hunt.Targets.Add(
                        new HuntTarget
                        {
                            Pokemon =
                                backupTarget.Pokemon,

                            Form =
                                backupTarget.Form,

                            Gender =
                                backupTarget.Gender,

                            Nature =
                                backupTarget.Nature,

                            IsAlpha =
                                backupTarget.IsAlpha,

                            Mark =
                                backupTarget.Mark
                        }
                    );
                }

                foreach (
                    var backupPhase
                    in backupHunt.Phases
                        ?? new List<HuntPhaseBackupItem>()
                )
                {
                    Shiny? foundShiny =
                        null;

                    if (
                        backupPhase
                            .FoundShinySourceId
                            .HasValue
                    )
                    {
                        shinyMap.TryGetValue(
                            backupPhase
                                .FoundShinySourceId
                                .Value,

                            out foundShiny
                        );
                    }

                    hunt.Phases.Add(
                        new HuntPhase
                        {
                            PhaseNumber =
                                backupPhase.PhaseNumber,

                            Encounters =
                                backupPhase.Encounters,

                            FoundShiny =
                                foundShiny,

                            WasTarget =
                                backupPhase.WasTarget,

                            CompletedAt =
                                backupPhase.CompletedAt
                        }
                    );
                }

                _context.Hunts.Add(
                    hunt
                );
            }
        }

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok(
            new
            {
                version =
                    backup.Version,

                importedShinies =
                    backup.Shinies.Count,

                disabledDexSlots =
                    backup.Version >= 2
                        ? backup
                            .DisabledDexSlots
                            ?.Count ?? 0
                        : (int?)null,

                importedHunts =
                    backup.Version >= 3
                        ? backup.Hunts?.Count ?? 0
                        : (int?)null
            }
        );
    }
    catch (Exception error)
    {
        await transaction.RollbackAsync();

        Console.WriteLine(
            error
        );

        return StatusCode(
            500,
            "Something went wrong while importing the backup."
        );
    }
}

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllShinies()
        {
            var shinies = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .ToListAsync();

            _context.Shinies.RemoveRange(shinies);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Shiny>> GetById(int id)
        {
            var shiny = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .FirstOrDefaultAsync(
                    shiny => shiny.Id == id
                );

            if (shiny == null)
            {
                return NotFound();
            }

            return Ok(shiny);
        }

        [HttpPost]
        public async Task<ActionResult<Shiny>> AddShiny(
            Shiny shiny
        )
        {
            _context.Shinies.Add(shiny);

            await _context.SaveChangesAsync();

            return Ok(shiny);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateShiny(
            int id,
            Shiny updatedShiny
        )
        {
            var shiny = await _context.Shinies
                .Include(shiny => shiny.Marks)
                .FirstOrDefaultAsync(
                    shiny => shiny.Id == id
                );

            if (shiny == null)
            {
                return NotFound();
            }

            shiny.Pokemon = updatedShiny.Pokemon;
            shiny.Nickname = updatedShiny.Nickname;
            shiny.Nature = updatedShiny.Nature;
            shiny.Game = updatedShiny.Game;
            shiny.Ball = updatedShiny.Ball;
            shiny.Method = updatedShiny.Method;
            shiny.Encounters = updatedShiny.Encounters;
            shiny.IsAlpha = updatedShiny.IsAlpha;
            shiny.Gender = updatedShiny.Gender;
            shiny.Form = updatedShiny.Form;

            shiny.Marks.Clear();

            foreach (var updatedMark in updatedShiny.Marks)
            {
                shiny.Marks.Add(
                    new ShinyMark
                    {
                        MarkName = updatedMark.MarkName
                    }
                );
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id:int}/sprite")]
        public async Task<IActionResult> UploadSprite(
            int id,
            IFormFile file
        )
        {
            var shiny = await _context.Shinies
                .FindAsync(id);

            if (shiny == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "shinies"
            );

            Directory.CreateDirectory(
                uploadsFolder
            );

            var extension =
                Path.GetExtension(file.FileName);

            var fileName =
                $"shiny-{id}-{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(
                uploadsFolder,
                fileName
            );

            using (
                var stream = new FileStream(
                    filePath,
                    FileMode.Create
                )
            )
            {
                await file.CopyToAsync(stream);
            }

            shiny.CustomSpritePath =
                $"/uploads/shinies/{fileName}";

            await _context.SaveChangesAsync();

            return Ok(
                new
                {
                    spritePath =
                        shiny.CustomSpritePath
                }
            );
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteShiny(
            int id
        )
        {
            var shiny = await _context.Shinies
                .FindAsync(id);

            if (shiny == null)
            {
                return NotFound();
            }

            _context.Shinies.Remove(shiny);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}