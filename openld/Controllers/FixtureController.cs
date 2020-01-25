using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;
using openld.Services;

namespace openld.Controllers {
    [ApiController]
    [Route("api/fixture/[action]")]
    public class FixtureController : ControllerBase {
        private readonly OpenLDContext _context;
        private readonly IFixtureService _fixtureService;

        public FixtureController(OpenLDContext context, IFixtureService fixtureService) {
            _context = context;
            _fixtureService = fixtureService;
        }

        [HttpGet("{id}.{ext?}")]
        public async Task<IActionResult> GetImage(string id) {
            Fixture fixture;
            try {
                fixture = await _context.Fixtures.Include(f => f.Image).FirstAsync(f => f.Id == id);
            } catch (InvalidOperationException) {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(fixture.Image.Path);
            return File(image, "image/jpeg");
        }

        [HttpGet("{id}.{ext?}")]
        public async Task<IActionResult> GetSymbol(string id) {
            Fixture fixture;
            try {
                fixture = await _context.Fixtures
                    .Include(f => f.Symbol)
                    .FirstAsync(f => f.Id == id);
            } catch (InvalidOperationException) {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(fixture.Symbol.Path);
            return File(image, "image/svg+xml");
        }

        [HttpGet("{id}.{ext?}")]
        public async Task<IActionResult> GetSymbolBitmap(string id) {
            Fixture fixture;
            try {
                fixture = await _context.Fixtures
                    .Include(f => f.Symbol)
                    .ThenInclude(s => s.Bitmap)
                    .FirstAsync(f => f.Id == id);
            } catch (InvalidOperationException) {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(fixture.Symbol.Bitmap.Path);
            return File(image, "image/png");
        }

        [HttpPost("{id}")]
        [Authorize]
        public async Task<ActionResult<Fixture>> AddFixtureSymbol(string id, [FromForm] IFormFile file) {
            string symbolId = await _fixtureService.UploadFixtureSymbolAsync(file);

            Symbol symbol = await _context.Symbols.FirstAsync(s => s.Id == symbolId);

            StoredImage bitmap = await _fixtureService.CreateSymbolBitmapAsync(symbol.Path);

            symbol.Bitmap = bitmap;

            Fixture fixture = await _context.Fixtures.Include(f => f.Symbol).FirstAsync(f => f.Id == id);

            fixture.Symbol = symbol;

            await _context.SaveChangesAsync();

            return fixture;
        }
    }

}