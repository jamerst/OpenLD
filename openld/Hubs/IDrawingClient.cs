using System.Collections.Generic;
using System.Threading.Tasks;

using openld.Models;

namespace openld.Hubs {
    public interface IDrawingClient {
        Task UserJoined(User user);
        Task UserLeft(string id);
        Task ConnectedUsers(HashSet<User> users);
        Task NewStructure(Structure structure);
        Task AddStructureSuccess(string id);
        Task AddStructureFailure();
        Task UpdateStructureGeometryFailure();
        Task UpdateStructureGeometry(Structure updated, List<RiggedFixture> fixtures);
        Task NewView(View newView);
        Task CreateViewSuccess(View newView);
        Task CreateViewFailure(string msg = null);
        Task DeleteView(string id);
        Task DeleteViewSuccess(string id);
        Task DeleteViewFailure();
        Task SelectObject(string type, string viewId, string structureId, string fixtureId, string userId);
        Task DeselectObject(string type, string viewId, string structureId, string fixtureId);
        Task UpdateObjectProperty(string type, string modifiedField, string viewId, Structure structure, RiggedFixture fixture);
        Task UpdateObjectPropertySuccess(string type, string id, string modifiedField, object prevValue, object newValue);
        Task UpdateObjectPropertyFailure();
        Task DeleteObject(string type, string viewId, string structureId, string fixtureId);
        Task DeleteObjectSuccess(string type, string viewId, string structureId, string fixtureId);
        Task DeleteObjectFailure(string type);
        Task AddFixture(string viewId, RiggedFixture fixture);
        Task AddFixtureSuccess(string id);
        Task AddFixtureFailure();
    }
}