using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;

using openld.Models;
using openld.Data;

namespace openld.Services {
    public class StructureService : IStructureService {
        private readonly OpenLDContext _context;
        private readonly IDrawingService _drawingService;
        private readonly IViewService _viewService;
        private readonly IMapper _mapper;
        public StructureService(OpenLDContext context, IDrawingService drawingService, IViewService viewService, IMapper mapper) {
            _context = context;
            _drawingService = drawingService;
            _viewService = viewService;
            _mapper = mapper;
        }

        public async Task<Drawing> GetDrawingAsync(Structure structure) {
            try {
                structure = await _context.Structures
                    .AsNoTracking()
                        .Include(s => s.View)
                            .ThenInclude(v => v.Drawing)
                        .FirstAsync(s => s.Id == structure.Id);

            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            return structure.View.Drawing;
        }
        public async Task<View> GetViewAsync(Structure structure) {
            try {
                structure = await _context.Structures.Include(s => s.View).FirstAsync(s => s.Id == structure.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            return structure.View;
        }

        public async Task<Structure> GetStructureAsync(string structureId) {
            Structure structure;
            try {
                structure = await _context.Structures.AsNoTracking()
                    .Include(s => s.Type)
                    .FirstAsync(s => s.Id == structureId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            return structure;
        }

        public async Task<Structure> AddStructureAsync(Structure structure) {
            try {
                structure.View = await _context.Views.FirstAsync(v => v.Id == structure.View.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            if (structure.Type != null) {
                structure.Type = await _context.StructureTypes.FirstAsync(st => st.Id == structure.Type.Id);
            } else {
                structure.Type = await GetStructureTypeByNameAsync("Bar");
            }

            if (structure.Fixtures != null) {
                foreach (RiggedFixture fixture in structure.Fixtures) {
                    fixture.Fixture = await _context.Fixtures.FirstAsync(f => f.Id == fixture.Fixture.Id);
                    fixture.Mode = await _context.FixtureModes.FirstAsync(fm => fm.Id == fixture.Mode.Id);
                }
            }

            await _context.Structures.AddAsync(structure);
            await _context.SaveChangesAsync();

            await _viewService.UpdateLastModifiedAsync(structure.View);
            return structure;
        }

        public async Task<Structure> SetStructureGeometryAsync(string structureId, Geometry geometry) {
            Structure structure;
            try {
                structure = await _context.Structures
                    .Include(s => s.View)
                    .FirstAsync(s => s.Id == structureId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            structure.Geometry = geometry;
            await _context.SaveChangesAsync();

            await _viewService.UpdateLastModifiedAsync(structure.View);
            return structure;
        }

        public async Task<List<RiggedFixture>> SetRiggedFixturePositionsAsync(string structureId, List<Point> points) {
            Structure structure;
            try {
                structure = await _context.Structures
                    .Include(s => s.Fixtures)
                    .FirstAsync(s => s.Id == structureId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            for (int i = 0; i < structure.Fixtures.Count; i++) {
                structure.Fixtures[i].Position = points[i];
            }

            await _context.SaveChangesAsync();

            return structure.Fixtures;
        }


        public async Task<Structure> UpdatePropsAsync(Structure structure) {
            Structure existing;
            try {
                existing = await _context.Structures
                    .Include(s => s.Type)
                    .FirstAsync(s => s.Id == structure.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            _mapper.Map(structure, existing);
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task DeleteAsync(string structureId) {
            Structure structure;
            try {
                structure = await _context.Structures.FirstAsync(s => s.Id == structureId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            _context.Structures.Remove(structure);
            await _context.SaveChangesAsync();
        }

        public async Task CreateTypesAsync(string[] types) {
            foreach (string typeName in types) {
                StructureType type = new StructureType();
                type.Name = typeName;

                await _context.StructureTypes.AddAsync(type);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<StructureType>> GetAllTypesAsync() {
            return await _context.StructureTypes.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<StructureType> GetStructureTypeAsync(string id) {
            StructureType type;
            try {
                type = await _context.StructureTypes.FirstAsync(st => st.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("StructureType ID not found");
            }

            return type;
        }

        public async Task<StructureType> GetStructureTypeByNameAsync(string name) {
            StructureType type;
            try {
                type = await _context.StructureTypes.FirstAsync(st => st.Name == name);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("StructureType name not found");
            }

            return type;
        }

        public async Task UpdateLastModifiedAsync(Structure structure) {
            try {
                structure = await _context.Structures.Include(s => s.View).ThenInclude(v => v.Drawing).FirstAsync(s => s.Id == structure.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            structure.View.Drawing.LastModified = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public interface IStructureService {
        Task<Drawing> GetDrawingAsync(Structure structure);
        Task<View> GetViewAsync(Structure structure);
        Task<Structure> GetStructureAsync(string structureId);
        Task<Structure> AddStructureAsync(Structure structure);
        Task<Structure> SetStructureGeometryAsync(string structureId, Geometry geometry);
        Task<List<RiggedFixture>> SetRiggedFixturePositionsAsync(string structureId, List<Point> points);
        Task<Structure> UpdatePropsAsync(Structure structure);
        Task DeleteAsync(string structureID);
        Task CreateTypesAsync(string[] types);
        Task<List<StructureType>> GetAllTypesAsync();
        Task<StructureType> GetStructureTypeAsync(string structureTypeId);
        Task<StructureType> GetStructureTypeByNameAsync(string name);
        Task UpdateLastModifiedAsync(Structure structure);
    }
}