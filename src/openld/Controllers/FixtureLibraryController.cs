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
        public class SearchParams {
            public string name;
            public string manufacturer;
            public string type;
        }
        private readonly OpenLDContext _context;

        public FixtureLibraryController(OpenLDContext context) {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<JsonResponse<List<Fixture>>>> GetFixtures(SearchParams search) {
            FixtureType type = _context.FixtureType.FirstOrDefault(t => t.Id == search.type);

            List<Fixture> fixtures;

            if (type != default(FixtureType)) {
                fixtures = _context.Fixture
                    .Where(f => EF.Functions.ILike(f.Name, $"%{search.name}%"))
                    .Where(f => EF.Functions.ILike(f.Manufacturer, $"%{search.manufacturer}%"))
                    .Where(f => f.Type == type)
                    .Include(f => f.Type)
                    .ToList();
            } else {
                fixtures = _context.Fixture
                    .Where(f => EF.Functions.ILike(f.Name, $"%{search.name}%"))
                    .Where(f => EF.Functions.ILike(f.Manufacturer, $"%{search.manufacturer}%"))
                    .Include(f => f.Type)
                    .ToList();
            }


            return new JsonResponse<List<Fixture>> { success = true, data = fixtures };
        }

        [HttpPost]
        public async Task<ActionResult<JsonResponse<object>>> CreateFixture(Fixture fixture) {
            fixture.Type = _context.FixtureType.FirstOrDefault(t => t.Id == fixture.Type.Id);

            if (fixture.Type == default(FixtureType)) {
                return new JsonResponse<object> { success = false, msg = "Type ID not found" };
            }

            try {
                _context.Fixture.Add(fixture);
                _context.SaveChanges();
            } catch (Exception e) {
                return new JsonResponse<object> { success = false, msg = "Failed to create new fixture: " + e.Message };
            }

            return new JsonResponse<object> { success = true };
        }

        [HttpPost]
        public async Task<ActionResult<JsonResponse<List<FixtureType>>>> GetFixtureTypes() {
            List<FixtureType> types = _context.FixtureType.OrderBy(t => t.Name).ToList();

            return new JsonResponse<List<FixtureType>> { success = true, data  = types };
        }

        [HttpPost]
        public void CreateType([FromBody] string[] names) {
            foreach (string name in names) {
                FixtureType type = new FixtureType();
                type.Name = name;

                _context.FixtureType.Add(type);
                _context.SaveChanges();
            }
        }
    }
}