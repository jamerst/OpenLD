using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using openld.Models;
using openld.Services;
using openld.Utils;

namespace openld.Hubs {

    [Authorize]
    public class DrawingHub : Hub<IDrawingClient> {
        private readonly IDrawingService _drawingService;
        private readonly IFixtureService _fixtureService;
        private readonly IViewService _viewService;
        private readonly IRiggedFixtureService _rFixtureService;
        private readonly IStructureService _structureService;
        private readonly IUserService _userService;
        private readonly AuthUtils _authUtils;
        // store the assigned group name (drawing ID) for each connection ID
        private static Dictionary<string, string> connectionDrawing = new Dictionary<string, string>();
        // store the users for each drawing
        private static Dictionary<string, HashSet<User>> drawingUsers = new Dictionary<string, HashSet<User>>();

        public DrawingHub(IDrawingService drawingService, IFixtureService fixtureService, IViewService viewService, IRiggedFixtureService rFixtureService, IStructureService structureService, IUserService userService) {
            _drawingService = drawingService;
            _fixtureService = fixtureService;
            _viewService = viewService;
            _rFixtureService = rFixtureService;
            _structureService = structureService;
            _userService = userService;
            _authUtils = new AuthUtils(drawingService, rFixtureService, structureService, viewService);
        }

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            if (connectionDrawing.ContainsKey(Context.ConnectionId)) {
                // alert other users in group to disconnecting
                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId])
                    .UserLeft(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionDrawing[Context.ConnectionId]);

                // remove user list and connection drawing items
                drawingUsers[connectionDrawing[Context.ConnectionId]].RemoveWhere(u => u.Id == Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                connectionDrawing.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task OpenDrawing(string id) {
            if (! await _authUtils.hasAccess(id, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            // if already assigned to a drawing
            if (connectionDrawing.ContainsKey(Context.ConnectionId)) {
                // remove from group - can't be in two drawings at once
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionDrawing[Context.ConnectionId]);
                connectionDrawing.Remove(Context.ConnectionId);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, id);

            // add connection id => group name element
            connectionDrawing.Add(Context.ConnectionId, id);

            // create user list if doesn't exist
            if (!drawingUsers.ContainsKey(id)) {
                drawingUsers.Add(id, new HashSet<User>());
            } else if (drawingUsers[id].Any(u => u.Id == Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                drawingUsers[id].RemoveWhere(u => u.Id == Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            await Clients.Caller.ConnectedUsers(drawingUsers[id]);

            // get user details for joining user from DB
            User newUser = await _userService.GetUserDetailsAsync(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            drawingUsers[id].Add(newUser);

            // send joining user details to rest of group
            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId])
                .UserJoined(newUser);

        }

        public async Task<JsonResponse<Structure>> AddStructure(Structure structure) {
            if (! await _authUtils.hasAccess(structure.View, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            Structure newStructure;
            try {
                newStructure = await _structureService.AddStructureAsync(structure);
            } catch (Exception) {
                return new JsonResponse<Structure> {success = false};
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).NewStructure(newStructure);
            return new JsonResponse<Structure> {success = true, data = newStructure};
        }

        public async Task<bool> UpdateStructureGeometry(string structureId, Geometry geometry, List<RiggedFixture> fixtures) {
            if (! await _authUtils.hasAccess(new Structure {Id = structureId}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            Structure updated;
            try {
                updated = await _structureService.SetStructureGeometryAsync(structureId, geometry);
            } catch (Exception) {
                return false;
            }

            try {
                await _structureService.SetRiggedFixturePositionsAsync(structureId, fixtures.Select(f => f.Position).ToList());
            } catch (Exception) {
                return false;
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateStructureGeometry(updated, fixtures);
            return true;
        }

        public async Task<bool> UpdateFixturePosition(RiggedFixture fixture) {
            if (! await _authUtils.hasAccess(fixture, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                fixture = await  _rFixtureService.UpdatePositionAsync(fixture);
            } catch (Exception) {
                return false;
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateFixturePosition(fixture);
            return true;
        }

        public async Task<string> CreateView(View view) {
            if (! await _authUtils.hasAccess(view.Drawing.Id, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            if (view.Width < 1 || view.Height < 1) {
                return "Invalid view size";
            }

            View newView;
            try {
                newView = await _viewService.CreateViewAsync(view);
            } catch (Exception) {
                return "";
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).NewView(newView);
            return "";
        }

        public async Task<string> DeleteView(string viewId) {
            if (! await _authUtils.hasAccess(new View {Id = viewId}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                await _viewService.DeleteViewAsync(viewId);
            } catch (Exception) {
                return "";
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeleteView(viewId);
            return viewId;
        }

        public async Task SelectObject(string type, string id) {
            if (type == "structure") {
                if (! await _authUtils.hasAccess(new Structure {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    (await _structureService.GetViewAsync(new Structure {Id = id})).Id,
                    id,
                    "",
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value
                );
            } else if (type == "fixture") {
                if (! await _authUtils.hasAccess(new RiggedFixture {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    (await _rFixtureService.GetViewAsync(new RiggedFixture {Id = id})).Id,
                    (await _rFixtureService.GetStructureAsync(new RiggedFixture {Id = id})).Id,
                    id,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value
                );
            }
        }

        public async Task DeselectObject(string type, string id) {
            if (type == "structure") {
                if (! await _authUtils.hasAccess(new Structure {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeselectObject(
                    type,
                    (await _structureService.GetViewAsync(new Structure {Id = id})).Id,
                    id,
                    ""
                );

            } else if (type == "fixture") {
                if (! await _authUtils.hasAccess(new RiggedFixture {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeselectObject(
                    type,
                    (await _rFixtureService.GetViewAsync(new RiggedFixture {Id = id})).Id,
                    (await _rFixtureService.GetStructureAsync(new RiggedFixture {Id = id})).Id,
                    id
                );
            }
        }

        public async Task<JsonResponse<dynamic>> UpdateObjectProperty(string type, string modifiedField, Structure structure, RiggedFixture fixture) {
            if (type == "structure") {
                if (! await _authUtils.hasAccess(structure, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                Structure current = await _structureService.GetStructureAsync(structure.Id);
                var prevValue = current[modifiedField];

                if (structure.Type != null && structure.Type.Id != "") {
                    structure.Type = await _structureService.GetStructureTypeAsync(structure.Type.Id);
                }

                Structure updated;
                try {
                    updated = await _structureService.UpdatePropsAsync(structure);
                } catch (Exception) {
                    return new JsonResponse<dynamic> {success = false};
                }

                await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateObjectProperty(
                    type,
                    modifiedField,
                    (await _structureService.GetViewAsync(updated)).Id,
                    updated,
                    null
                );

                return new JsonResponse<dynamic> {success = true, data = prevValue};

            } else if (type == "fixture") {
                if (! await _authUtils.hasAccess(fixture, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                RiggedFixture current = await _rFixtureService.GetRiggedFixtureAsync(fixture.Id);
                var prevValue = current[modifiedField];

                if (fixture.Mode != null && fixture.Mode.Id != "") {
                    fixture.Mode = await _fixtureService.GetFixtureModeAsync(fixture.Mode.Id);
                }

                RiggedFixture updated;
                try {
                    updated = await _rFixtureService.UpdatePropsAsync(fixture);
                } catch (Exception) {
                    return new JsonResponse<dynamic> {success = false};
                }

                await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateObjectProperty(
                    type,
                    modifiedField,
                    (await _rFixtureService.GetViewAsync(updated)).Id,
                    await _rFixtureService.GetStructureAsync(updated),
                    updated
                );

                return new JsonResponse<dynamic> {success = true, data = prevValue};
            } else {
                return new JsonResponse<dynamic> {success = false};
            }
        }

        public async Task<JsonResponse<dynamic>> DeleteObject(string type, string id) {
            if (type == "structure") {
                if (! await _authUtils.hasAccess(new Structure {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                View view = await _structureService.GetViewAsync(new Structure { Id = id });

                try {
                    await _structureService.DeleteAsync(id);
                } catch (Exception) {
                    return new JsonResponse<dynamic> {success = false, data = type};
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeleteObject(
                    type,
                    view.Id,
                    id,
                    null
                );

                return new JsonResponse<dynamic> {success = true, data = new {type = type, viewId = view.Id, structureId = id, fixtureId = ""}};
            } else if (type == "fixture") {
                if (! await _authUtils.hasAccess(new RiggedFixture {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                View view = await _rFixtureService.GetViewAsync(new RiggedFixture { Id = id });
                Structure structure = await _rFixtureService.GetStructureAsync(new RiggedFixture { Id = id });

                try {
                    await _rFixtureService.DeleteAsync(id);
                } catch (Exception) {
                    return new JsonResponse<dynamic> {success = false, data = type};
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeleteObject(
                    type,
                    view.Id,
                    structure.Id,
                    id
                );
                return new JsonResponse<dynamic> {success = true, data = new {type = type, viewId = view.Id, structureId = structure.Id, fixtureId = id}};
            } else {
                return new JsonResponse<dynamic> {success = false};
            }
        }

        public async Task<JsonResponse<RiggedFixture>> AddFixture(RiggedFixture fixture) {
            if (! await _authUtils.hasAccess(new Structure {Id = fixture.Structure.Id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                fixture = await _rFixtureService.AddRiggedFixtureAsync(fixture);
            } catch (Exception) {
                return new JsonResponse<RiggedFixture> {success = false};
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).AddFixture(
                (await _structureService.GetViewAsync(fixture.Structure)).Id,
                fixture
            );

            return new JsonResponse<RiggedFixture> {success = true, data = fixture};
        }
    }
}