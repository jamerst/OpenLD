using System;
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
        private readonly AuthUtils _authUtils;
        private static ConcurrentDictionary<string, string> connectionDrawing = new ConcurrentDictionary<string, string>();

        public DrawingHub(IDrawingService drawingService) {
            _drawingService = drawingService;
            _authUtils = new AuthUtils(drawingService);
        }

        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            if (connectionDrawing.ContainsKey(Context.ConnectionId)) {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionDrawing[Context.ConnectionId]);
                connectionDrawing.TryRemove(Context.ConnectionId, out _);
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
                connectionDrawing.TryRemove(Context.ConnectionId, out _);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, id);
            connectionDrawing.TryAdd(Context.ConnectionId, id);
        }

        public async Task AddStructure(Structure structure) {
            if (! await _authUtils.hasAccess(structure.View, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            Structure newStructure;
            try {
                newStructure = await _drawingService.AddStructureAsync(structure);
            } catch (Exception) {
                await Clients.Caller.SendAsync("AddStructureFailure");
                return;
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SendAsync("NewStructure", newStructure);
            await Clients.Caller.SendAsync("AddStructureSuccess", newStructure);
        }

        public async Task CreateView(View view) {
            if (! await _authUtils.hasAccess(view.Drawing.Id, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            View newView;
            try {
                newView = await _drawingService.CreateViewAsync(view);
            } catch (Exception) {
                await Clients.Caller.SendAsync("CreateViewFailure");
                return;
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SendAsync("NewView", newView);
            await Clients.Caller.SendAsync("CreateViewSuccess", newView);
        }
    }
}