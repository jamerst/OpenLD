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
#region Connection Management and Init
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
#endregion
#region Views
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
#endregion
#region Structures
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
#endregion
#region Selection
        public async Task SelectObject(string type, string id) {
            if (type == "structure") {
                if (! await _authUtils.hasAccess(new Structure {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    id,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    (await _structureService.GetViewAsync(new Structure {Id = id})).Id,
                    ""
                );
            } else if (type == "fixture") {
                if (! await _authUtils.hasAccess(new RiggedFixture {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    id,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    (await _rFixtureService.GetViewAsync(new RiggedFixture {Id = id})).Id,
                    (await _rFixtureService.GetStructureAsync(new RiggedFixture {Id = id})).Id
                );
            } else if (type == "label") {
                if (! await _authUtils.hasAccess(new Label {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    id,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    (await _labelService.GetViewAsync(new Label {Id = id})).Id,
                    ""
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
                    id,
                    (await _structureService.GetViewAsync(new Structure {Id = id})).Id,
                    ""
                );

            } else if (type == "fixture") {
                if (! await _authUtils.hasAccess(new RiggedFixture {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeselectObject(
                    type,
                    id,
                    (await _rFixtureService.GetViewAsync(new RiggedFixture {Id = id})).Id,
                    (await _rFixtureService.GetStructureAsync(new RiggedFixture {Id = id})).Id
                );
            } else if (type == "label") {
                if (! await _authUtils.hasAccess(new Label {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeselectObject(
                    type,
                    id,
                    (await _labelService.GetViewAsync(new Label {Id = id})).Id,
                    ""
                );
            }
        }
#endregion
#region Update/Delete Objects
        public async Task<JsonResponse<dynamic>> UpdateObjectProperty(string type, string modifiedField, Structure structure, RiggedFixture fixture, Label label) {
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
                    null,
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
                    updated,
                    null
                );

                return new JsonResponse<dynamic> {success = true, data = prevValue};
            } else if (type == "label") {
                if (! await _authUtils.hasAccess(label, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                Label current = await _labelService.GetLabelAsync(label.Id);
                var prevValue = current[modifiedField];

                Label updated;
                try {
                    updated = await _labelService.UpdatePropsAsync(label);
                } catch (Exception) {
                    return new JsonResponse<dynamic> {success = false};
                }

                await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateObjectProperty(
                    type,
                    modifiedField,
                    (await _labelService.GetViewAsync(updated)).Id,
                    null,
                    null,
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
                    id,
                    view.Id,
                    null
                );

                return new JsonResponse<dynamic> {success = true, data = new {type = type, id = id, viewId = view.Id, structureId = ""}};
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
                    id,
                    view.Id,
                    structure.Id
                );
                return new JsonResponse<dynamic> {success = true, data = new {type = type, id = id, viewId = view.Id, structureId = structure.Id}};
            } else if (type == "label") {
                if (! await _authUtils.hasAccess(new Label {Id = id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                View view = await _labelService.GetViewAsync(new Label { Id = id });

                try {
                    await _labelService.DeleteAsync(id);
                } catch (Exception) {
                    return new JsonResponse<dynamic> {success = false, data = type};
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeleteObject(
                    type,
                    id,
                    view.Id,
                    null
                );

                return new JsonResponse<dynamic> {success = true, data = new {type = type, id = id, viewId = view.Id, structureId = ""}};
            } else {
                return new JsonResponse<dynamic> {success = false};
            }
        }
#endregion
#region Fixtures
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
#endregion
#region Labels
        public async Task<JsonResponse<Label>> AddLabel(Label label) {
            if (! await _authUtils.hasAccess(new View {Id = label.View.Id}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                label = await _labelService.AddLabelAsync(label);
            } catch (Exception) {
                return new JsonResponse<Label> {success = false};
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).AddLabel(
                label.View.Id,
                label
            );

            return new JsonResponse<Label> {success = true, data = label};
        }

        public async Task<bool> UpdateLabelPosition(Label label) {
            if (! await _authUtils.hasAccess(label, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                label = await  _labelService.UpdatePositionAsync(label);
            } catch (Exception) {
                return false;
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateLabelPosition(label);
            return true;
        }
        #endregion
    }
}