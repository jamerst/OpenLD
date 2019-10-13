using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace collab_prototype {
    public class TextHub : Hub {
        private static Dictionary<string, string> connectionGroups = new Dictionary<string, string>();
        public override async Task OnConnectedAsync() {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception) {
            if (connectionGroups.ContainsKey(Context.ConnectionId)) {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionGroups[Context.ConnectionId]);
                connectionGroups.Remove(Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task BroadcastText(string text) {
            if (connectionGroups.ContainsKey(Context.ConnectionId)) {
                await Clients.OthersInGroup(connectionGroups[Context.ConnectionId]).SendAsync("ReceiveText", text);
            }
        }

        public async Task JoinGroup(string group) {
            if (connectionGroups.ContainsKey(Context.ConnectionId)) {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connectionGroups[Context.ConnectionId]);
                connectionGroups.Remove(Context.ConnectionId);
            }
            connectionGroups.Add(Context.ConnectionId, group);
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
        }
    }
}