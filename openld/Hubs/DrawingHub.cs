using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using openld.Models;
using openld.Services;
using openld.Utils;

namespace openld.Hubs {

    [Authorize]
    public class DrawingHub : Hub {
        private readonly IDrawingService _drawingService;
        private readonly IViewService _viewService;
        private readonly IStructureService _structureService;
        private readonly IUserService _userService;
        private readonly AuthUtils _authUtils;
        // store the assigned group name (drawing ID) for each connection ID
        private static Dictionary<string, string> connectionDrawing = new Dictionary<string, string>();
        // store the users for each drawing
        private static Dictionary<string, List<User>> drawingUsers = new Dictionary<string, List<User>>();

        public DrawingHub(IDrawingService drawingService, IViewService viewService, IStructureService structureService, IUserService userService) {
            _drawingService = drawingService;
            _viewService = viewService;
            _structureService = structureService;
            _userService = userService;
            _authUtils = new AuthUtils(drawingService, structureService, viewService);
        }

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            if (connectionDrawing.ContainsKey(Context.ConnectionId)) {
                // alert other users in group to disconnecting
                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId])
                    .SendAsync("UserLeft",
                        Context.User.FindFirst(ClaimTypes.NameIdentifier).Value
                    );

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionDrawing[Context.ConnectionId]);

                // remove user list and connection drawing items
                drawingUsers[connectionDrawing[Context.ConnectionId]].RemoveAll(u => u.Id == Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
                drawingUsers.Add(id, new List<User>());
            }
            await Clients.Caller.SendAsync("ConnectedUsers", drawingUsers[id]);

            // get user details for joining user from DB
            User newUser = await _userService.GetUserDetailsAsync(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            drawingUsers[id].Add(newUser);

            // send joining user details to rest of group
            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId])
                .SendAsync("UserJoined", newUser);

        }

        public async Task AddStructure(Structure structure) {
            if (! await _authUtils.hasAccess(structure.View, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            Structure newStructure;
            try {
                newStructure = await _structureService.AddStructureAsync(structure);
            } catch (Exception) {
                await Clients.Caller.SendAsync("AddStructureFailure");
                return;
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SendAsync("NewStructure", newStructure);
            await Clients.Caller.SendAsync("AddStructureSuccess", newStructure);
        }

        public async Task UpdateStructureGeometry(string structureId, Geometry geometry) {
            if (! await _authUtils.hasAccess(new Structure {Id = structureId}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            Structure updated;
            try {
                updated = await _structureService.SetStructureGeometryAsync(structureId, geometry);
            } catch (Exception) {
                await Clients.Caller.SendAsync("UpdateStructureGeometryFailure");
                return;
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SendAsync("UpdateStructureGeometry", updated);
        }

        public async Task CreateView(View view) {
            if (! await _authUtils.hasAccess(view.Drawing.Id, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            if (view.Width < 1 || view.Height < 1) {
                await Clients.Caller.SendAsync("CreateViewFailure", "Invalid view size");
                return;
            }

            View newView;
            try {
                newView = await _viewService.CreateViewAsync(view);
            } catch (Exception) {
                await Clients.Caller.SendAsync("CreateViewFailure");
                return;
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SendAsync("NewView", newView);
            await Clients.Caller.SendAsync("CreateViewSuccess", newView);
        }

        public async Task DeleteView(string viewId) {
            if (! await _authUtils.hasAccess(new View {Id = viewId}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                await _viewService.DeleteViewAsync(viewId);
            } catch (Exception) {
                await Clients.Caller.SendAsync("DeleteViewFailure");
                return;
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SendAsync("DeleteView", viewId);
            await Clients.Caller.SendAsync("DeleteViewSuccess", viewId);
        }

        public async Task SelectStructure(string structureId) {
            if (! await _authUtils.hasAccess(new Structure {Id = structureId}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId])
                .SendAsync(
                    "SelectStructure",
                    (await _structureService.GetViewAsync(new Structure {Id = structureId})).Id,
                    structureId,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value
                );
        }

        public async Task DeselectStructure(string structureId) {
            if (! await _authUtils.hasAccess(new Structure {Id = structureId}, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId])
                .SendAsync(
                    "DeselectStructure",
                    (await _structureService.GetViewAsync(new Structure {Id = structureId})).Id,
                    structureId
                );
        }
    }
}