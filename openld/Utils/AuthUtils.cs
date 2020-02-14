using System.Threading.Tasks;

using openld.Models;
using openld.Services;

namespace openld.Utils {
    public class AuthUtils {
        private readonly IDrawingService _drawingService;
        private readonly ILabelService _labelService;
        private readonly IRiggedFixtureService _rFixtureService;
        private readonly IStructureService _structureService;
        private readonly IViewService _viewService;

        public AuthUtils(IDrawingService drawingService, ILabelService labelService, IRiggedFixtureService rFixtureService, IStructureService structureService, IViewService viewService) {
            _drawingService = drawingService;
            _labelService = labelService;
            _rFixtureService = rFixtureService;
            _structureService = structureService;
            _viewService = viewService;
        }

        public async Task<bool> hasAccess(string drawingId, string userId) {
            return await _drawingService.IsOwnerAsync(drawingId, userId) || await _drawingService.IsSharedWithAsync(drawingId, userId);
        }

        public async Task<bool> hasAccess(Drawing drawing, string userId) {
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }

        public async Task<bool> hasAccess(View view, string userId) {
            Drawing drawing = await _viewService.GetDrawingAsync(view);
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }

        public async Task<bool> hasAccess(Label label, string userId) {
            Drawing drawing = await _labelService.GetDrawingAsync(label);
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }

        public async Task<bool> hasAccess(Structure structure, string userId) {
            Drawing drawing = await _structureService.GetDrawingAsync(structure);
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }


        public async Task<bool> hasAccess(RiggedFixture fixture, string userId) {
            Drawing drawing = await _rFixtureService.GetDrawingAsync(fixture);
            return await _drawingService.IsOwnerAsync(drawing.Id, userId) || await _drawingService.IsSharedWithAsync(drawing.Id, userId);
        }
    }
}