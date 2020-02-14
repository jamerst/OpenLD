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
        private readonly IStructureService _structureService;
        private readonly ITemplateService _templateService;
        private readonly AuthUtils _authUtils;
        public DrawingController(IDrawingService drawingService, ILabelService labelService, IRiggedFixtureService rFixtureService, IStructureService structureService, ITemplateService templateService, IViewService viewService) {
            _drawingService = drawingService;
            _structureService = structureService;
            _templateService = templateService;
            _authUtils = new AuthUtils(drawingService, labelService, rFixtureService, structureService, viewService);
        }

        public class CreateRequest {
            public Drawing drawing;
            public Template template;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<string>>> CreateDrawing(CreateRequest request) {
            if (request.drawing.Views[0].Width < 1 || request.drawing.Views[0].Height < 1) {
                return new JsonResponse<string> { success = false, msg = "Invalid drawing size" };
            }

            var user = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            string newDrawingId = "";
            try {
                if (request.template.Id != null) {
                    newDrawingId = await _drawingService.CreateDrawingFromTemplateAsync(user, request.drawing, request.template);
                } else {
                    newDrawingId = await _drawingService.CreateDrawingAsync(user, request.drawing);
                }
            } catch (Exception) {
                return new JsonResponse<string> { success = false, msg = "Unknown error creating new drawing" };
            }

            return new JsonResponse<string> { success = true, data = newDrawingId };
        }

        [HttpGet("{drawingId}")]
        [Authorize]
        public async Task<ActionResult<JsonResponse<Drawing>>> GetDrawing(string drawingId) {
            if (! await _drawingService.DrawingExistsAsync(drawingId)) {
                return NotFound();
            }

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

        [HttpGet("{drawingId}")]
        [Authorize]
        public async Task<ActionResult<JsonResponse<PrintDrawing>>> GetPrintDrawing(string drawingId) {
            if (! await _drawingService.DrawingExistsAsync(drawingId)) {
                return NotFound();
            }

            if (! await _authUtils.hasAccess(drawingId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            PrintDrawing drawing;
            try {
                drawing = await _drawingService.GetPrintDrawingAsync(drawingId);
            } catch (KeyNotFoundException) {
                return new JsonResponse<PrintDrawing> { success = false, msg = "Drawing ID not found" };
            } catch (Exception) {
                return new JsonResponse<PrintDrawing> { success = false, msg = "Unknown error loading drawing" };
            }

            return new JsonResponse<PrintDrawing> { success = true, data = drawing };
        }

        [HttpPost("{drawingId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteDrawing(string drawingId) {
            if (! await _drawingService.IsOwnerAsync(drawingId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            try {
                await _drawingService.DeleteDrawingAsync(drawingId);
            } catch (Exception) {
                return false;
            }

            return true;
        }

        [HttpGet("{drawingId}")]
        [Authorize]
        public async Task<ActionResult<JsonResponse<List<UserDrawing>>>> GetSharedUsers(string drawingId) {
            if (! await _authUtils.hasAccess(drawingId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            List<UserDrawing> users;
            try {
                users = await _drawingService.GetSharedUsersAsync(drawingId);
            } catch (Exception) {
                return new JsonResponse<List<UserDrawing>> { success = false, msg = "Unknown error fetching users" };
            }

            return new JsonResponse<List<UserDrawing>> { success = true, data = users };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<UserDrawing>>> ShareWith(UserDrawing ud) {
            if (! await _authUtils.hasAccess(ud.Drawing.Id, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            try {
                ud = await _drawingService.ShareWithUserAsync(ud.User.Email, ud.Drawing.Id);
            } catch (Exception e) when (e is KeyNotFoundException || e is InvalidOperationException) {
                return new JsonResponse<UserDrawing> { success = false, msg = e.Message };
            } catch (Exception) {
                return new JsonResponse<UserDrawing> { success = false, msg = "Unknown error sharing with user" };
            }

            return new JsonResponse<UserDrawing> { success = true, data = ud };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<JsonResponse<string>>> UnshareWith(UserDrawing ud) {
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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<JsonResponse<List<Drawing>>>> GetOwnedDrawings() {
            List<Drawing> drawings;
            try {
                drawings = await _drawingService.GetOwnedDrawingsAsync(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            } catch (Exception) {
                return new JsonResponse<List<Drawing>> { success = false, msg = "Unknown error fetching drawings" };
            }

            return new JsonResponse<List<Drawing>> { success = true, data = drawings };
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<JsonResponse<List<Drawing>>>> GetSharedDrawings() {
            List<Drawing> drawings;
            try {
                drawings = await _drawingService.GetSharedDrawingsAsync(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            } catch (Exception) {
                return new JsonResponse<List<Drawing>> { success = false, msg = "Unknown error fetching drawings" };
            }

            return new JsonResponse<List<Drawing>> { success = true, data = drawings };
        }

        [HttpPost("{drawingId}")]
        [Authorize]
        public async Task<ActionResult> CreateTemplate(string drawingId) {
            if (!await _drawingService.IsOwnerAsync(drawingId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            try {
                await _templateService.CreateTemplateAsync(drawingId);
            } catch (Exception) {
                return StatusCode(500);
            }

            return Ok();
        }

        [HttpPost("{drawingId}")]
        [Authorize]
        public async Task<ActionResult<bool>> CanTemplate(string drawingId) {
            return await _drawingService.IsOwnerAsync(drawingId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                && !await _templateService.IsTemplateAsync(drawingId);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Template>>> GetTemplates() {
            return await _templateService.GetTemplatesAsync();
        }
    }
}