using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;

namespace openld.Services {
    public class FixtureTypeService : IFixtureTypeService {
        private readonly OpenLDContext _context;
        public FixtureTypeService(OpenLDContext context) {
            _context = context;
        }

        public async Task<List<FixtureType>> GetAllTypes() {
            return await _context.FixtureType.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task CreateTypes(string[] names) {
            foreach (string name in names) {
                FixtureType type = new FixtureType();
                type.Name = name;

                await _context.FixtureType.AddAsync(type);
                await _context.SaveChangesAsync();
            }
        }
    }

    public interface IFixtureTypeService {
        Task<List<FixtureType>> GetAllTypes();
        Task CreateTypes(string[] names);
    }
}