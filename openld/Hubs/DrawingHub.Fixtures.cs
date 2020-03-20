using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using openld.Models;
using openld.Utils;

namespace openld.Hubs {
    public partial class DrawingHub : Hub<IDrawingClient> {
        public async Task<JsonResponse<RiggedFixture>> AddFixture(RiggedFixture fixture) {
            if (!await _authUtils.hasAccess(new Structure { Id = fixture.Structure.Id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                fixture = await _rFixtureService.AddRiggedFixtureAsync(fixture);
            } catch (Exception) {
                return new JsonResponse<RiggedFixture> { success = false };
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).AddFixture(
                (await _structureService.GetViewAsync(fixture.Structure)).Id,
                fixture
            );

            return new JsonResponse<RiggedFixture> { success = true, data = fixture };
        }

        public async Task<bool> UpdateFixturePosition(RiggedFixture fixture) {
            if (!await _authUtils.hasAccess(fixture, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                throw new HubException("401: Unauthorised");
            }

            try {
                fixture = await _rFixtureService.UpdatePositionAsync(fixture);
            } catch (Exception) {
                return false;
            }

            await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateFixturePosition(fixture);
            return true;
        }
    }
}