using System;
using System.Collections.Generic;
using System.Security.Claims;
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
    [Route("api/drawing/[action]/{drawingId}")]
    public class DrawingController : ControllerBase {
        private readonly IDrawingService _drawingService;
        public DrawingController(IDrawingService drawingService) {
            _drawingService = drawingService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<string>>> CreateDrawing(string drawingId) {
            var user = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string newDrawingId = "";
            try {
                newDrawingId = await _drawingService.createDrawing(user);
            } catch (Exception) {
                return new JsonResponse<string> { success = false, msg = "Unknown error creating new drawing" };
            }

            return new JsonResponse<string> { success = true, data = newDrawingId };
        }

        [HttpGet]
        [Authorize]
        [DrawingShared(AccessContext.Drawing)]
        public async Task<ActionResult<JsonResponse<Drawing>>> GetDrawing(string drawingId) {
            Drawing drawing;
            try {
                drawing = await _drawingService.getDrawing(drawingId);
            } catch (KeyNotFoundException) {
                return new JsonResponse<Drawing> { success = false, msg = "Drawing ID not found" };
            } catch (Exception) {
                return new JsonResponse<Drawing> { success = false, msg = "Unknown error loading drawing" };
            }

            return new JsonResponse<Drawing> { success = true, data = drawing };
        }

        [HttpPost]
        [Authorize]
        [DrawingShared(AccessContext.Drawing)]
        public async Task<ActionResult<JsonResponse<Structure>>> AddStructure(string drawingId, Structure structure) {
            Structure newStructure;
            try {
                newStructure = await _drawingService.addStructure(structure);
            } catch (Exception) {
                return new JsonResponse<Structure> { success = false, msg = "Unknown error adding structure" };
            }

            return new JsonResponse<Structure> { success = true, data = newStructure };
        }
    }
}