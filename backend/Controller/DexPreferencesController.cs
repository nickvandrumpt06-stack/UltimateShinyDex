using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltimateShinyDex.Api.Data;
using UltimateShinyDex.Api.Models;

namespace UltimateShinyDex.Api.Controllers
{
    [ApiController]
    [Route("api/dex-preferences")]
    public class DexPreferencesController : ControllerBase
    {
        private readonly ShinyDbContext _context;

        public DexPreferencesController(
            ShinyDbContext context)
        {
            _context = context;
        }

        [HttpGet("disabled")]
        public async Task<ActionResult<List<DisabledDexSlot>>>
            GetDisabledSlots()
        {
            return await _context.DisabledDexSlots
                .OrderBy(d => d.DexNumber)
                .ToListAsync();
        }

        [HttpPost("disable")]
        public async Task<IActionResult> DisableSlot(
            DisabledDexSlot slot)
        {
            var alreadyExists =
                await _context.DisabledDexSlots.AnyAsync(d =>
                    d.DexNumber == slot.DexNumber &&
                    d.Pokemon == slot.Pokemon &&
                    d.Form == slot.Form &&
                    d.Gender == slot.Gender
                );

            if (alreadyExists)
            {
                return NoContent();
            }

            _context.DisabledDexSlots.Add(slot);

            await _context.SaveChangesAsync();

            return Ok(slot);
        }

        [HttpPost("enable")]
        public async Task<IActionResult> EnableSlot(
            DisabledDexSlot slot)
        {
            var existing =
                await _context.DisabledDexSlots.FirstOrDefaultAsync(d =>
                    d.DexNumber == slot.DexNumber &&
                    d.Pokemon == slot.Pokemon &&
                    d.Form == slot.Form &&
                    d.Gender == slot.Gender
                );

            if (existing == null)
            {
                return NoContent();
            }

            _context.DisabledDexSlots.Remove(existing);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("enable-all")]
        public async Task<IActionResult> EnableAll()
        {
            var disabledSlots =
                await _context.DisabledDexSlots.ToListAsync();

            _context.DisabledDexSlots.RemoveRange(disabledSlots);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}