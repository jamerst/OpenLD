using System;
using System.Collections.Generic;
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
                throw new KeyNotFoundException("View ID not found");
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

        public async Task<Structure> AddStructureAsync(Structure structure) {
            try {
                structure.View = await _context.Views.FirstAsync(v => v.Id == structure.View.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
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

        public async Task<Structure> UpdateStructureProps(Structure structure) {
            Structure existing;
            try {
                existing = await _context.Structures.FirstAsync(s => s.Id == structure.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Structure ID not found");
            }

            _mapper.Map(structure, existing);
            await _context.SaveChangesAsync();

            return existing;
        }
    }

    public interface IStructureService {
        Task<Drawing> GetDrawingAsync(Structure structure);
        Task<View> GetViewAsync(Structure structure);
        Task<Structure> AddStructureAsync(Structure structure);
        Task<Structure> SetStructureGeometryAsync(string structureId, Geometry geometry);
        Task<Structure> UpdateStructureProps(Structure structure);
    }
}