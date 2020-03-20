using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using openld.Models;

namespace openld.Hubs {
    public partial class DrawingHub : Hub<IDrawingClient> {
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
    }
}