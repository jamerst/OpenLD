using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;
using openld.Utils;

namespace openld.Controllers {
    [ApiController]
    [Route("api/library/[action]")]
    public class FixtureLibraryController {
        private readonly OpenLDContext _context;

        public FixtureLibraryController(OpenLDContext context) {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<JsonResponse<List<Fixture>>>> GetFixtures([FromBody] string search = "") {
            List<Fixture> fixtures = _context.Fixture.Where(f => EF.Functions.ILike(f.Name, $"%{search}%")).ToList();

            return new JsonResponse<List<Fixture>> { success = true, data = fixtures };
        }

        [HttpPost]
        public async Task<ActionResult<JsonResponse<object>>> CreateFixture(Fixture fixture) {
            fixture.Type = _context.FixtureType.FirstOrDefault(t => t.Id == fixture.Type.Id);

            try {
                _context.Fixture.Add(fixture);
                _context.SaveChanges();
            } catch (Exception e) {
                return new JsonResponse<object> { success = false, msg = "Failed to create new fixture: " + e.Message };
            }

            return new JsonResponse<object> { success = true };
        }
    }
}