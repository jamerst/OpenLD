using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;

using openld.Models;
using openld.Data;

namespace openld.Services {
    public class RiggedFixtureService : IRiggedFixtureService {
        private readonly OpenLDContext _context;
        public RiggedFixtureService(OpenLDContext context) {
            _context = context;
        }
        public async Task<RiggedFixture> AddRiggedFixtureAsync(RiggedFixture fixture) {
            try {
                fixture.Structure = await _context.Structures.FirstAsync(s => s.Id == fixture.Structure.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            try {
                fixture.Fixture = await _context.Fixtures.Include(f => f.Modes).FirstAsync(f => f.Id == fixture.Fixture.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Fixture ID not found");
            }

            if (fixture.Fixture.Modes.Count > 0) {
                fixture.Mode = fixture.Fixture.Modes[0];
            }

            await _context.RiggedFixtures.AddAsync(fixture);
            await _context.SaveChangesAsync();

            return fixture;
        }

        public async Task<Drawing> GetDrawingAsync(RiggedFixture fixture) {
            try {
                fixture = await _context.RiggedFixtures
                    .Include(rf => rf.Structure)
                        .ThenInclude(s => s.View)
                            .ThenInclude(v => v.Drawing)
                    .FirstAsync(rf => rf.Id == fixture.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("RiggedFixture ID not found");
            }

            return fixture.Structure.View.Drawing;
        }

        public async Task<View> GetViewAsync(RiggedFixture fixture) {
            try {
                fixture = await _context.RiggedFixtures
                    .Include(rf => rf.Structure)
                        .ThenInclude(s => s.View)
                    .FirstAsync(rf => rf.Id == fixture.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("RiggedFixture ID not found");
            }

            return fixture.Structure.View;
        }

        public async Task<Structure> GetStructureAsync(RiggedFixture fixture) {
            try {
                fixture = await _context.RiggedFixtures
                    .Include(rf => rf.Structure)
                    .FirstAsync(rf => rf.Id == fixture.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("RiggedFixture ID not found");
            }

            return fixture.Structure;
        }
    }

    public interface IRiggedFixtureService {
        Task<RiggedFixture> AddRiggedFixtureAsync(RiggedFixture fixture);
        Task<Drawing> GetDrawingAsync(RiggedFixture fixture);
        Task<View> GetViewAsync(RiggedFixture fixture);
        Task<Structure> GetStructureAsync(RiggedFixture fixture);
    }
}