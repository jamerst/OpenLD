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
                drawing.Owner = await _context.User.FirstAsync(u => u.Id == userId);
            } catch (InvalidOperationException) {
                throw new UnauthorizedAccessException("Invalid user ID");
            }

            drawing.Title = "New Drawing";
            drawing.LastModified = DateTime.Now;

            View view = new View();
            view.Drawing = drawing;
            view.Name = "Default";

            await _context.Drawing.AddAsync(drawing);
            await _context.View.AddAsync(view);
            await _context.SaveChangesAsync();

            return drawing.Id;
        }

        public async Task<Drawing> getDrawing(string id) {
            Drawing drawing;

            try {
                drawing = await _context.Drawing
                .Include(d => d.Views)
                    .ThenInclude(v => v.Structures)
                        .ThenInclude(s => s.Fixtures)
                .FirstAsync(d => d.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            return drawing;
        }
    }

    public interface IDrawingService {
        Task<string> createDrawing(string userId);
        Task<Drawing> getDrawing(string id);
    }
}