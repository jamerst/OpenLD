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
    [Route("api/view/[action]")]
    public class ViewController : ControllerBase {
        private readonly IViewService _viewService;
        private readonly AuthUtils _authUtils;
        public ViewController(IDrawingService drawingService, IRiggedFixtureService rFixtureService, IStructureService structureService, IViewService viewService) {
            _viewService = viewService;
            _authUtils = new AuthUtils(drawingService, rFixtureService, structureService, viewService);
        }
    }
}