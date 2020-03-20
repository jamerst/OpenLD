using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using openld.Models;
using openld.Utils;

namespace openld.Hubs {
    public partial class DrawingHub : Hub<IDrawingClient> {
        public async Task<JsonResponse<Label>> AddLabel(Label label) {
            if (!await _authUtils.hasAccess(new View { Id = label.View.Id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                label = await _labelService.AddLabelAsync(label);
            } catch (Exception) {
                return new JsonResponse<Label> { success = false };
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).AddLabel(
                label.View.Id,
                label
            );

            return new JsonResponse<Label> { success = true, data = label };
        }

        public async Task<bool> UpdateLabelPosition(Label label) {
            if (!await _authUtils.hasAccess(label, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                label = await _labelService.UpdatePositionAsync(label);
            } catch (Exception) {
                return false;
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateLabelPosition(label);
            return true;
        }
    }
}