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

        [HttpGet("{drawingId}")]
        [Authorize]
        public async Task<ActionResult<JsonResponse<List<UserDrawings>>>> GetSharedUsers(string drawingId) {
            if (! await _authUtils.hasAccess(drawingId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            List<UserDrawings> users;
            try {
                users = await _drawingService.GetSharedUsersAsync(drawingId);
            } catch (Exception) {
                return new JsonResponse<List<UserDrawings>> { success = false, msg = "Unknown error fetching users" };
            }

            return new JsonResponse<List<UserDrawings>> { success = true, data = users };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<UserDrawings>>> ShareWith(UserDrawings ud) {
            if (! await _authUtils.hasAccess(ud.Drawing.Id, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            try {
                ud = await _drawingService.ShareWithUserAsync(ud.User.Email, ud.Drawing.Id);
            } catch (KeyNotFoundException e) {
                return new JsonResponse<UserDrawings> { success = false, msg = e.Message };
            } catch (Exception) {
                return new JsonResponse<UserDrawings> { success = false, msg = "Unknown error sharing with user" };
            }

            return new JsonResponse<UserDrawings> { success = true, data = ud };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<string>>> UnshareWith(UserDrawings ud) {
            if (! await _authUtils.hasAccess(ud.Drawing.Id, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            string id;
            try {
                id = await _drawingService.UnshareWithUserAsync(ud.Id);
            } catch (Exception) {
                return new JsonResponse<string> { success = false, msg = "Unknown error unsharing with user" };
            }

            return new JsonResponse<string> { success = true, data = id };
        }
    }
}