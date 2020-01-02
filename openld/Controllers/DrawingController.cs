using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using openld.Models;
using openld.Services;
using openld.Utils;

namespace openld.Controllers {
    [ApiController]
    [Route("api/drawing/[action]")]
    public class DrawingController : ControllerBase {
        private readonly IDrawingService _drawingService;
        private readonly AuthUtils _authUtils;
        public DrawingController(IDrawingService drawingService) {
            _drawingService = drawingService;
            _authUtils = new AuthUtils(drawingService);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<string>>> CreateDrawing(Drawing drawing) {
            if (drawing.Width < 1 || drawing.Height < 1) {
                return new JsonResponse<string> { success = false, msg = "Invalid drawing size" };
            }

            var user = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string newDrawingId = "";
            try {
                newDrawingId = await _drawingService.CreateDrawingAsync(user, drawing);
            } catch (Exception) {
                return new JsonResponse<string> { success = false, msg = "Unknown error creating new drawing" };
            }

            return new JsonResponse<string> { success = true, data = newDrawingId };
        }

        [HttpGet("{drawingId}")]
        [Authorize]
        public async Task<ActionResult<JsonResponse<Drawing>>> GetDrawing(string drawingId) {
            if (! await _authUtils.hasAccess(drawingId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            Drawing drawing;
            try {
                drawing = await _drawingService.GetDrawingAsync(drawingId);
            } catch (KeyNotFoundException) {
                return new JsonResponse<Drawing> { success = false, msg = "Drawing ID not found" };
            } catch (Exception) {
                return new JsonResponse<Drawing> { success = false, msg = "Unknown error loading drawing" };
            }

            return new JsonResponse<Drawing> { success = true, data = drawing };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<Structure>>> AddStructure(Structure structure) {
            if (! await _authUtils.hasAccess(structure.View, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            Structure newStructure;
            try {
                newStructure = await _drawingService.AddStructureAsync(structure);
            } catch (Exception) {
                return new JsonResponse<Structure> { success = false, msg = "Unknown error adding structure" };
            }

            return new JsonResponse<Structure> { success = true, data = newStructure };
        }
    }
}