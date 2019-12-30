using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using openld.Authorization;
using openld.Models;
using openld.Services;

namespace openld.Hubs {

    [Authorize]
    public class DrawingHub : Hub {
        private readonly IDrawingService _drawingService;
        private static ConcurrentDictionary<string, string> connectionDrawing = new ConcurrentDictionary<string, string>();

        public DrawingHub(IDrawingService drawingService) {
            _drawingService = drawingService;
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
        [DrawingShared(AccessContext.Drawing)]
        public async Task AddStructure(Structure structure) {
            Structure newStructure;
            try {
                newStructure = await _drawingService.addStructure(structure);
            } catch (Exception) {
                await Clients.Caller.SendAsync("AddStructureFailure");
                return;
            }

            await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SendAsync("NewStructure", newStructure);
            await Clients.Caller.SendAsync("AddStructureSuccess", newStructure);
        }

        public async Task OpenDrawing(string id) {
            // if already assigned to a drawing
            if (connectionDrawing.ContainsKey(Context.ConnectionId)) {
                // remove from group - can't be in two drawings at once
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionDrawing[Context.ConnectionId]);
                connectionDrawing.TryRemove(Context.ConnectionId, out _);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, id);
            connectionDrawing.TryAdd(Context.ConnectionId, id);
        }
    }
}