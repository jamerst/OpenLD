using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;

using openld.Models;
using openld.Utils;

namespace openld.Hubs {
    public partial class DrawingHub : Hub<IDrawingClient> {
        public async Task<JsonResponse<dynamic>> UpdateObjectProperty(string type, string modifiedField, Structure structure, RiggedFixture fixture, Label label) {
            if (type == "structure") {
                if (!await _authUtils.hasAccess(structure, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                Structure current = await _structureService.GetStructureAsync(structure.Id);
                var prevValue = current[modifiedField];

                if (structure.Type != null && structure.Type.Id != "") {
                    structure.Type = await _structureService.GetStructureTypeAsync(structure.Type.Id);
                }

                Structure updated;
                try {
                    updated = await _structureService.UpdatePropsAsync(structure);
                } catch (Exception) {
                    return new JsonResponse<dynamic> { success = false };
                }

                await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateObjectProperty(
                    type,
                    modifiedField,
                    (await _structureService.GetViewAsync(updated)).Id,
                    updated,
                    null,
                    null
                );

                return new JsonResponse<dynamic> { success = true, data = prevValue };

            } else if (type == "fixture") {
                if (!await _authUtils.hasAccess(fixture, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                RiggedFixture current = await _rFixtureService.GetRiggedFixtureAsync(fixture.Id);
                var prevValue = current[modifiedField];

                if (fixture.Mode != null && fixture.Mode.Id != "") {
                    fixture.Mode = await _fixtureService.GetFixtureModeAsync(fixture.Mode.Id);
                }

                RiggedFixture updated;
                try {
                    updated = await _rFixtureService.UpdatePropsAsync(fixture);
                } catch (Exception) {
                    return new JsonResponse<dynamic> { success = false };
                }

                await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateObjectProperty(
                    type,
                    modifiedField,
                    (await _rFixtureService.GetViewAsync(updated)).Id,
                    await _rFixtureService.GetStructureAsync(updated),
                    updated,
                    null
                );

                return new JsonResponse<dynamic> { success = true, data = prevValue };
            } else if (type == "label") {
                if (!await _authUtils.hasAccess(label, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                Label current = await _labelService.GetLabelAsync(label.Id);
                var prevValue = current[modifiedField];

                Label updated;
                try {
                    updated = await _labelService.UpdatePropsAsync(label);
                } catch (Exception) {
                    return new JsonResponse<dynamic> { success = false };
                }

                await Clients.Group(connectionDrawing[Context.ConnectionId]).UpdateObjectProperty(
                    type,
                    modifiedField,
                    (await _labelService.GetViewAsync(updated)).Id,
                    null,
                    null,
                    updated
                );

                return new JsonResponse<dynamic> { success = true, data = prevValue };

            } else {
                return new JsonResponse<dynamic> { success = false };
            }
        }

        public async Task<JsonResponse<dynamic>> DeleteObject(string type, string id) {
            if (type == "structure") {
                if (!await _authUtils.hasAccess(new Structure { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                View view = await _structureService.GetViewAsync(new Structure { Id = id });

                try {
                    await _structureService.DeleteAsync(id);
                } catch (Exception) {
                    return new JsonResponse<dynamic> { success = false, data = type };
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeleteObject(
                    type,
                    id,
                    view.Id,
                    null
                );

                return new JsonResponse<dynamic> { success = true, data = new { type = type, id = id, viewId = view.Id, structureId = "" } };
            } else if (type == "fixture") {
                if (!await _authUtils.hasAccess(new RiggedFixture { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                View view = await _rFixtureService.GetViewAsync(new RiggedFixture { Id = id });
                Structure structure = await _rFixtureService.GetStructureAsync(new RiggedFixture { Id = id });

                try {
                    await _rFixtureService.DeleteAsync(id);
                } catch (Exception) {
                    return new JsonResponse<dynamic> { success = false, data = type };
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeleteObject(
                    type,
                    id,
                    view.Id,
                    structure.Id
                );
                return new JsonResponse<dynamic> { success = true, data = new { type = type, id = id, viewId = view.Id, structureId = structure.Id } };
            } else if (type == "label") {
                if (!await _authUtils.hasAccess(new Label { Id = id }, Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                    throw new HubException("401: Unauthorised");
                }

                View view = await _labelService.GetViewAsync(new Label { Id = id });

                try {
                    await _labelService.DeleteAsync(id);
                } catch (Exception) {
                    return new JsonResponse<dynamic> { success = false, data = type };
                }

                await Clients.OthersInGroup(connectionDrawing[Context.ConnectionId]).DeleteObject(
                    type,
                    id,
                    view.Id,
                    null
                );

                return new JsonResponse<dynamic> { success = true, data = new { type = type, id = id, viewId = view.Id, structureId = "" } };
            } else {
                return new JsonResponse<dynamic> { success = false };
            }
        }
    }
}