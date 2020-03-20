using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using openld.Models;

namespace openld.Hubs {
    public partial class DrawingHub : Hub<IDrawingClient> {
        public async Task SelectObject(string type, string id) {
            if (type == "structure") {
                if (!await _authUtils.hasAccess(new Structure { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    id,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    (await _structureService.GetViewAsync(new Structure { Id = id })).Id,
                    ""
                );
            } else if (type == "fixture") {
                if (!await _authUtils.hasAccess(new RiggedFixture { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    id,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    (await _rFixtureService.GetViewAsync(new RiggedFixture { Id = id })).Id,
                    (await _rFixtureService.GetStructureAsync(new RiggedFixture { Id = id })).Id
                );
            } else if (type == "label") {
                if (!await _authUtils.hasAccess(new Label { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).SelectObject(
                    type,
                    id,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                    (await _labelService.GetViewAsync(new Label { Id = id })).Id,
                    ""
                );
            }
        }

        public async Task DeselectObject(string type, string id) {
            if (type == "structure") {
                if (!await _authUtils.hasAccess(new Structure { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeselectObject(
                    type,
                    id,
                    (await _structureService.GetViewAsync(new Structure { Id = id })).Id,
                    ""
                );

            } else if (type == "fixture") {
                if (!await _authUtils.hasAccess(new RiggedFixture { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeselectObject(
                    type,
                    id,
                    (await _rFixtureService.GetViewAsync(new RiggedFixture { Id = id })).Id,
                    (await _rFixtureService.GetStructureAsync(new RiggedFixture { Id = id })).Id
                );
            } else if (type == "label") {
                if (!await _authUtils.hasAccess(new Label { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeselectObject(
                    type,
                    id,
                    (await _labelService.GetViewAsync(new Label { Id = id })).Id,
                    ""
                );
            }
        }
    }
}