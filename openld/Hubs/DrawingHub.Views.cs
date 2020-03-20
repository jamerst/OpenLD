using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using openld.Models;

namespace openld.Hubs {
    public partial class DrawingHub : Hub<IDrawingClient> {
        public async Task<string> CreateView(View view) {
            if (!await _authUtils.hasAccess(view.Drawing.Id, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
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
            if (!await _authUtils.hasAccess(new View { Id = viewId }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
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
    }
}