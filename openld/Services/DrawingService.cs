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
        public async Task<string> CreateDrawingAsync(string userId, Drawing drawing) {
            try {
                drawing.Owner = await _context.Users.FirstAsync(u => u.Id == userId);
            } catch (InvalidOperationException) {
                throw new UnauthorizedAccessException("Invalid user ID");
            }

            drawing.LastModified = DateTime.Now;

            View view = new View();
            view.Drawing = drawing;
            view.Name = "Default";
            view.Width = drawing.Views[0].Width;
            view.Height = drawing.Views[0].Height;
            drawing.Views = null;

            await _context.Drawings.AddAsync(drawing);
            await _context.Views.AddAsync(view);
            await _context.SaveChangesAsync();

            return drawing.Id;
        }

        public async Task<Drawing> GetDrawingAsync(string id) {
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

        public async Task<Drawing> GetDrawingAsync(View view) {
            try {
                view = await _context.Views
                    .AsNoTracking()
                        .Include(v => v.Drawing)
                        .FirstAsync(v => v.Id == view.Id);

            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            return view.Drawing;
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

        public async Task<bool> DrawingExistsAsync(string id) {
            return await _context.Drawings.Where(d => d.Id == id).AnyAsync();
        }

        public async Task<View> CreateViewAsync(View view) {
            Drawing drawing;
            try {
                drawing = await _context.Drawings.FirstAsync(d => d.Id == view.Drawing.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            View newView = new View();
            newView.Drawing = drawing;
            newView.Name = view.Name;
            newView.Width = view.Width;
            newView.Height = view.Height;
            newView.Type = view.Type;
            newView.Structures = new List<Structure>();

            await _context.Views.AddAsync(newView);
            await _context.SaveChangesAsync();

            return newView;
        }

        public async Task DeleteViewAsync(string viewId) {
            View view;
            try {
                view = await _context.Views.Include(v => v.Drawing).FirstAsync(v => v.Id == viewId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            if (_context.Views.AsNoTracking().Where(v => v.Drawing.Id == view.Drawing.Id).Count() == 1) {
                throw new InvalidOperationException("Cannot delete last view in drawing");
            }

            _context.Views.Remove(view);
            await _context.SaveChangesAsync();
            await UpdateLastModifiedAsync(view.Drawing);
        }

        public async Task<Structure> AddStructureAsync(Structure structure) {
            try {
                structure.View = await _context.Views.FirstAsync(v => v.Id == structure.View.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            await _context.Structures.AddAsync(structure);
            await _context.SaveChangesAsync();

            await UpdateLastModifiedAsync(structure.View);
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

            await UpdateLastModifiedAsync(structure.View);
            return structure;
        }

        public async Task<List<UserDrawings>> GetSharedUsersAsync(string drawingId) {
            List<UserDrawings> userDrawings;
            userDrawings =  await _context.UserDrawings
                .AsNoTracking()
                .Where(ud => ud.Drawing.Id == drawingId)
                .Include(ud => ud.User)
                .ToListAsync();

            return userDrawings;
        }

        public async Task<UserDrawings> ShareWithUserAsync(string email, string drawingId) {
            User user;
            try {
                user = await _context.Users.FirstAsync(u => u.Email == email);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Email not found");
            }

            Drawing drawing;
            try {
                drawing = await _context.Drawings.Include(d => d.Owner).FirstAsync(d => d.Id == drawingId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            if (drawing.Owner.Id == user.Id) {
                throw new InvalidOperationException("Cannot share with drawing owner");
            } else if (await _context.UserDrawings.Where(ud => ud.User == user).Where(ud => ud.Drawing == drawing).AnyAsync()) {
                throw new InvalidOperationException("Drawing already shared with user");
            }

            UserDrawings ud = new UserDrawings();
            ud.Drawing = drawing;
            ud.User = user;

            await _context.UserDrawings.AddAsync(ud);
            await _context.SaveChangesAsync();

            await UpdateLastModifiedAsync(drawing);
            return ud;
        }

        public async Task<string> UnshareWithUserAsync(string userDrawingId) {
            UserDrawings ud;
            try {
                ud = await _context.UserDrawings.FirstAsync(ud => ud.Id == userDrawingId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("UserDrawing id not found");
            }

            _context.UserDrawings.Remove(ud);
            await _context.SaveChangesAsync();

            return userDrawingId;
        }

        public async Task<bool> IsSharedWithAsync(string drawingId, string userId) {
            if (await _context.UserDrawings.Where(u => u.Drawing.Id == drawingId).Where(u => u.User.Id == userId).AnyAsync()) {
                return true;
            } else {
                return false;
            }
        }

        public async Task<bool> IsOwnerAsync(string drawingId, string userId) {
            try {
                Drawing drawing = await _context.Drawings.AsNoTracking()
                    .Include(d => d.Owner)
                    .FirstAsync(d => d.Id == drawingId);

                if (drawing.Owner.Id == userId) {
                    return true;
                } else {
                    return false;
                }
            } catch (Exception) {
                return false;
            }
        }

        public async Task<List<Drawing>> GetOwnedDrawingsAsync(string userId) {
            return await _context.Drawings
                .Include(d => d.Owner)
                .Where(d => d.Owner.Id == userId)
                .Select(d => new Drawing {
                    Id = d.Id,
                    Title = d.Title,
                    Owner = new User { UserName = d.Owner.UserName },
                    LastModified = d.LastModified,
                })
                .OrderBy(d => d.LastModified)
                .ToListAsync();
        }

        public async Task<List<Drawing>> GetSharedDrawingsAsync(string userId) {
            return await _context.Drawings
                .Include(d => d.Owner)
                .Where(d => _context.UserDrawings
                    .Where(ud => ud.Drawing.Id == d.Id)
                    .Where(ud => ud.User.Id == userId)
                    .Any()
                ).Select(d => new Drawing {
                    Id = d.Id,
                    Title = d.Title,
                    Owner = new User { UserName = d.Owner.UserName },
                    LastModified = d.LastModified,
                })
                .OrderBy(d => d.LastModified)
                .ToListAsync();
        }

        public async Task UpdateLastModifiedAsync(Drawing drawing) {
            try {
                drawing = await _context.Drawings.FirstAsync(d => d.Id == drawing.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            drawing.LastModified = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLastModifiedAsync(View view) {
            try {
                view = await _context.Views.Include(v => v.Drawing).FirstAsync(v => v.Id == view.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            view.Drawing.LastModified = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public interface IDrawingService {
        Task<string> CreateDrawingAsync(string userId, Drawing drawing);
        Task<Drawing> GetDrawingAsync(string id);
        Task<Drawing> GetDrawingAsync(View view);
        Task<Drawing> GetDrawingAsync(Structure structure);
        Task<bool> DrawingExistsAsync(string id);
        Task<View> CreateViewAsync(View view);
        Task DeleteViewAsync(string viewId);
        Task<Structure> AddStructureAsync(Structure structure);
        Task<Structure> SetStructureGeometryAsync(string structureId, Geometry geometry);
        Task<List<UserDrawings>> GetSharedUsersAsync(string drawingId);
        Task<UserDrawings> ShareWithUserAsync(string email, string drawingId);
        Task<string> UnshareWithUserAsync(string userDrawingId);
        Task<bool> IsSharedWithAsync(string drawingId, string userId);
        Task<bool> IsOwnerAsync(string drawingId, string userId);
        Task<List<Drawing>> GetOwnedDrawingsAsync(string userId);
        Task<List<Drawing>> GetSharedDrawingsAsync(string userId);
        Task UpdateLastModifiedAsync(Drawing drawing);
        Task UpdateLastModifiedAsync(View view);
    }
}