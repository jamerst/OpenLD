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

        public async Task<List<FixtureType>> GetAllTypesAsync() {
            return await _context.FixtureTypes.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task CreateTypesAsync(string[] names) {
            foreach (string name in names) {
                FixtureType type = new FixtureType();
                type.Name = name;

                await _context.FixtureTypes.AddAsync(type);
                await _context.SaveChangesAsync();
            }
        }
    }

    public interface IFixtureTypeService {
        Task<List<FixtureType>> GetAllTypesAsync();
        Task CreateTypesAsync(string[] names);
    }
}