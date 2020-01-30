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
        private readonly IStructureService _structureService;
        private readonly IMapper _mapper;
        public RiggedFixtureService(OpenLDContext context, IStructureService structureService, IMapper mapper) {
            _context = context;
            _structureService = structureService;
            _mapper = mapper;
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

            fixture.Angle = 0;

            await _context.RiggedFixtures.AddAsync(fixture);
            await _context.SaveChangesAsync();

            await _structureService.UpdateLastModifiedAsync(fixture.Structure);

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

        public async Task<RiggedFixture> UpdatePropsAsync(RiggedFixture fixture) {
            RiggedFixture existing;
            try {
                existing = await _context.RiggedFixtures.Include(rf => rf.Mode).Include(rf => rf.Structure).FirstAsync(f => f.Id == fixture.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            _mapper.Map(fixture, existing);
            await _context.SaveChangesAsync();

            await _structureService.UpdateLastModifiedAsync(existing.Structure);
            return existing;
        }

        public async Task DeleteAsync(string id) {
            RiggedFixture fixture;
            try {
                fixture = await _context.RiggedFixtures.Include(rf => rf.Structure).FirstAsync(f => f.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            await _structureService.UpdateLastModifiedAsync(fixture.Structure);

            _context.RiggedFixtures.Remove(fixture);
            await _context.SaveChangesAsync();
        }
    }

    public interface IRiggedFixtureService {
        Task<RiggedFixture> AddRiggedFixtureAsync(RiggedFixture fixture);
        Task<Drawing> GetDrawingAsync(RiggedFixture fixture);
        Task<View> GetViewAsync(RiggedFixture fixture);
        Task<Structure> GetStructureAsync(RiggedFixture fixture);
        Task<RiggedFixture> UpdatePropsAsync(RiggedFixture fixture);
        Task DeleteAsync(string id);
    }
}