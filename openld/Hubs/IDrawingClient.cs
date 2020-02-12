using System.Collections.Generic;
using System.Threading.Tasks;

using openld.Models;

namespace openld.Hubs {
    public interface IDrawingClient {
        Task UserJoined(User user);
        Task UserLeft(string id);
        Task ConnectedUsers(HashSet<User> users);
        Task NewStructure(Structure structure);
        Task UpdateStructureGeometry(Structure updated, List<RiggedFixture> fixtures);
        Task NewView(View newView);
        Task DeleteView(string id);
        Task SelectObject(string type, string viewId, string structureId, string fixtureId, string userId);
        Task DeselectObject(string type, string viewId, string structureId, string fixtureId);
        Task UpdateObjectProperty(string type, string modifiedField, string viewId, Structure structure, RiggedFixture fixture);
        Task DeleteObject(string type, string viewId, string structureId, string fixtureId);
        Task AddFixture(string viewId, RiggedFixture fixture);
        Task UpdateFixturePosition(RiggedFixture updated);
    }
}