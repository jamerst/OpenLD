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

        public bool isSharedWith(string drawingId, string userId) {
            if (_context.UserDrawings.Where(u => u.Drawing.Id == drawingId).Where(u => u.User.Id == userId).Any()) {
                return true;
            } else {
                return false;
            }
        }

        public bool isOwner(string drawingId, string userId) {
            try {
                Drawing drawing = _context.Drawings.AsNoTracking()
                    .Include(d => d.Owner)
                    .First(d => d.Id == drawingId);

                if (drawing.Owner.Id == userId) {
                    return true;
                } else {
                    return false;
                }
            } catch (Exception) {
                return false;
            }
        }
    }

    public interface IDrawingService {
        Task<string> createDrawing(string userId);
        Task<Drawing> getDrawing(string id);
        Task<Structure> addStructure(Structure structure);
        bool isSharedWith(string drawingId, string userId);
        bool isOwner(string drawingId, string userId);
    }
}