using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;

namespace openld.Services {
    public class DrawingService : IDrawingService {
        private readonly OpenLDContext _context;
        public DrawingService(OpenLDContext context) {
            _context = context;
        }
        public async Task<string> createDrawing(string userId) {
            Drawing drawing = new Drawing();

            try {
                drawing.Owner = await _context.Users.FirstAsync(u => u.Id == userId);
            } catch (InvalidOperationException) {
                throw new UnauthorizedAccessException("Invalid user ID");
            }

            drawing.Title = "New Drawing";
            drawing.LastModified = DateTime.Now;

            View view = new View();
            view.Drawing = drawing;
            view.Name = "Default";

            await _context.Drawings.AddAsync(drawing);
            await _context.Views.AddAsync(view);
            await _context.SaveChangesAsync();

            return drawing.Id;
        }

        public async Task<Drawing> getDrawing(string id) {
            Drawing drawing;

            try {
                drawing = await _context.Drawings
                .Include(d => d.Views)
                    .ThenInclude(v => v.Structures)
                        .ThenInclude(s => s.Fixtures)
                .FirstAsync(d => d.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            return drawing;
        }

        public async Task<Structure> addStructure(Structure structure) {
            try {
                structure.View = await _context.Views.FirstAsync(v => v.Id == structure.View.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            await _context.Structures.AddAsync(structure);
            await _context.SaveChangesAsync();

            return structure;
        }
    }

    public interface IDrawingService {
        Task<string> createDrawing(string userId);
        Task<Drawing> getDrawing(string id);
        Task<Structure> addStructure(Structure structure);
    }
}