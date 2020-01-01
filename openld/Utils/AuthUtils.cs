using System.Threading.Tasks;

using openld.Models;
using openld.Services;

namespace openld.Utils {
    public class AuthUtils {
        private readonly IDrawingService _drawingService;

        public AuthUtils(IDrawingService drawingService) {
            _drawingService = drawingService;
        }

        public async Task<bool> hasAccess(string drawingId, string userId) {
            return await _drawingService.IsOwnerAsync(drawingId, userId) || await _drawingService.IsSharedWithAsync(drawingId, userId);
        }

        public async Task<bool> hasAccess(Drawing drawing, string userId) {
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }

        public async Task<bool> hasAccess(View view, string userId) {
            Drawing drawing = await _drawingService.GetDrawingAsync(view);
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }

        public async Task<bool> hasAccess(Structure structure, string userId) {
            Drawing drawing = await _drawingService.GetDrawingAsync(structure);
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }
    }
}