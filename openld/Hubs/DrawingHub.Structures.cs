using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using openld.Models;
using openld.Utils;

namespace openld.Hubs {
    public partial class DrawingHub : Hub<IDrawingClient> {
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
    }
}