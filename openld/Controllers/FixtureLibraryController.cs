using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using openld.Authorization;
using openld.Models;
using openld.Services;
using openld.Utils;

namespace openld.Controllers {
    [ApiController]
    [Route("api/library/[action]")]
    public class FixtureLibraryController {
        private readonly IFixtureService _fixtureService;
        private readonly IFixtureTypeService _fixtureTypeService;

        public FixtureLibraryController(IFixtureService fixtureService, IFixtureTypeService fixtureTypeService) {
            _fixtureService = fixtureService;
            _fixtureTypeService = fixtureTypeService;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<List<Fixture>>>> GetFixtures(SearchParams search) {
            return new JsonResponse<List<Fixture>> { success = true, data = await _fixtureService.SearchAllFixtures(search) };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<object>>> CreateFixture(Fixture fixture) {
            try {
                await _fixtureService.CreateFixture(fixture);
            } catch (KeyNotFoundException e) {
                return new JsonResponse<object> { success = false, msg = e.Message };
            } catch (Exception) {
                return new JsonResponse<object> { success = false, msg = "Unknown error creating fixture" };
            }

            return new JsonResponse<object> { success = true };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<string>>> UploadFixtureImage([FromForm] IFormFile file) {
            string id = "";

            try {
                id = await _fixtureService.UploadFixtureImage(file);
            } catch (ArgumentException e) {
                return new JsonResponse<string> { success = false, msg = e.Message };
            } catch (Exception) {
                return new JsonResponse<string> { success = false, msg = "Unknown error uploading image" };
            }

            return new JsonResponse<string> { success = true, data = id };
        }

        [HttpPost]
        public async Task<ActionResult<JsonResponse<List<FixtureType>>>> GetFixtureTypes() {
            return new JsonResponse<List<FixtureType>> { success = true, data  = await _fixtureTypeService.GetAllTypes() };
        }

        [HttpPost]
        [Authorize]
        public async Task CreateTypes([FromBody] string[] names) {
            await _fixtureTypeService.CreateTypes(names);
        }
    }
}