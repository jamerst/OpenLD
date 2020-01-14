using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using openld.Models;
using openld.Services;
using openld.Utils;

namespace openld.Controllers {
    [ApiController]
    [Route("api/structure/[action]")]
    public class StructureController {
        private readonly IStructureService _structureService;

        public StructureController(IStructureService structureService) {
            _structureService = structureService;
        }

        [HttpPost]
        [Authorize]
        public async Task CreateTypes([FromBody] string[] types) {
            await _structureService.CreateTypesAsync(types);
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResponse<List<StructureType>>> GetStructureTypes() {
            List<StructureType> types;
            try {
                types = await _structureService.GetAllTypesAsync();
            } catch (Exception) {
                return new JsonResponse<List<StructureType>> {success = false, msg = "Unknown error fetching types"};
            }

            return new JsonResponse<List<StructureType>> {success = true, data = types};
        }
    }
}