using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;

namespace openld.Controllers {
    [ApiController]
    [Route("api/fixture/[action]")]
    public class FixtureController : ControllerBase {
        private readonly OpenLDContext _context;

        public FixtureController(OpenLDContext context) {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(string id) {
            Fixture fixture = await _context.Fixtures.Include(f => f.Image).FirstOrDefaultAsync(f => f.Id == id);

            if (fixture == default(Fixture)) {
                return NotFound();
            }

            var image = System.IO.File.OpenRead(fixture.Image.Path);
            return File(image, "image/jpeg");
        }
    }

}