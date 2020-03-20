using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using openld.Models;
using openld.Services;
using openld.Utils;

namespace openld.Hubs {

    [Authorize]
    public partial class DrawingHub : Hub<IDrawingClient> {
        private readonly IDrawingService _drawingService;
        private readonly IFixtureService _fixtureService;
        private readonly ILabelService _labelService;
        private readonly IRiggedFixtureService _rFixtureService;
        private readonly IStructureService _structureService;
        private readonly IUserService _userService;
        private readonly IViewService _viewService;
        private readonly AuthUtils _authUtils;
        // store the assigned group name (drawing ID) for each connection ID
        private static Dictionary<string, string> connectionDrawing = new Dictionary<string, string>();
        // store the users for each drawing
        private static Dictionary<string, HashSet<User>> drawingUsers = new Dictionary<string, HashSet<User>>();

        public DrawingHub(IDrawingService drawingService, IFixtureService fixtureService, ILabelService labelService, IRiggedFixtureService rFixtureService, IStructureService structureService, IUserService userService, IViewService viewService) {
            _drawingService = drawingService;
            _fixtureService = fixtureService;
            _labelService = labelService;
            _rFixtureService = rFixtureService;
            _structureService = structureService;
            _userService = userService;
            _viewService = viewService;
            _authUtils = new AuthUtils(drawingService, labelService, rFixtureService, structureService, viewService);
        }
    }
}