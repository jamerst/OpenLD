using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;

namespace openld.Services {
    public class DrawingService : IDrawingService {
        private readonly OpenLDContext _context;
        private readonly ITemplateService _templateService;
        private readonly IViewService _viewService;
        private readonly IMapper _mapper;
        public DrawingService(OpenLDContext context, ITemplateService templateService, IViewService viewService, IMapper mapper) {
            _context = context;
            _templateService = templateService;
            _viewService = viewService;
            _mapper = mapper;
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

            _context.Drawings.Add(drawing);
            _context.Views.Add(view);
            await _context.SaveChangesAsync();

            return drawing.Id;
        }

        public async Task<string> CreateDrawingFromTemplateAsync(string userId, Drawing drawing, Template template) {
            try {
                drawing.Owner = await _context.Users.FirstAsync(u => u.Id == userId);
            } catch (InvalidOperationException) {
                throw new UnauthorizedAccessException("Invalid user ID");
            }

            try {
                template = await _context.Templates.Include(t => t.Drawing).FirstAsync(t => t.Id == template.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Template ID not found");
            }

            Drawing templateDrawing;
            try {
                templateDrawing = await _context.Drawings
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Fixtures)
                                .ThenInclude(rf => rf.Fixture)
                                    .ThenInclude(f => f.Modes)
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Fixtures)
                                .ThenInclude(f => f.Mode)
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Type)
                    .FirstAsync(d => d.Id == template.Drawing.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            Drawing newDrawing = templateDrawing.Clone();
            newDrawing.Owner = drawing.Owner;
            newDrawing.Title = drawing.Title;

            newDrawing.LastModified = DateTime.Now;

            _context.Drawings.Add(newDrawing);
            await _context.SaveChangesAsync();

            return newDrawing.Id;
        }


        public async Task<Drawing> GetDrawingAsync(string id) {
            Drawing drawing;

            try {
                drawing = await _context.Drawings.AsNoTracking()
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Fixtures)
                                .ThenInclude(rf => rf.Fixture)
                                    .ThenInclude(f => f.Modes)
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Fixtures)
                                .ThenInclude(f => f.Mode)
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Type)
                    .FirstAsync(d => d.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            return drawing;
        }

        public async Task<PrintDrawing> GetPrintDrawingAsync(string id) {
            Drawing drawing;

            try {
                drawing = await _context.Drawings.AsNoTracking()
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Fixtures)
                                .ThenInclude(rf => rf.Fixture)
                    .Include(d => d.Views)
                        .ThenInclude(v => v.Structures)
                            .ThenInclude(s => s.Fixtures)
                                .ThenInclude(f => f.Mode)
                    .Include(d => d.Owner)
                    .FirstAsync(d => d.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            // only return the username, not the password hash lol
            drawing.Owner = new User { UserName = drawing.Owner.UserName };

            PrintDrawing result = new PrintDrawing();
            result.Drawing = drawing;
            result.UsedFixtures = new List<List<UsedFixtureResult>>();
            result.RiggedFixtures = new List<RiggedFixture>();

            foreach(View view in drawing.Views) {
                var fixtures = await _viewService.GetUsedFixturesAsync(view.Id);
                result.UsedFixtures.Add(fixtures.Item1);
                result.RiggedFixtures.AddRange(fixtures.Item2);
            }

            return result;
        }

        public async Task DeleteDrawingAsync(string id) {
            Drawing drawing;
            try {
                drawing = await _context.Drawings.FirstAsync(d => d.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            // remove templates if they exist
            if (await _templateService.IsTemplateAsync(id)) {
                _context.Templates.Remove(await _context.Templates.FirstAsync(t => t.Drawing.Id == id));
            }

            _context.Drawings.Remove(drawing);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DrawingExistsAsync(string id) {
            return await _context.Drawings.Where(d => d.Id == id).AnyAsync();
        }

        public async Task<List<UserDrawing>> GetSharedUsersAsync(string drawingId) {
            List<UserDrawing> UserDrawing;
            UserDrawing =  await _context.UserDrawings
                .AsNoTracking()
                .Where(ud => ud.Drawing.Id == drawingId)
                .Include(ud => ud.User)
                .ToListAsync();

            return UserDrawing;
        }

        public async Task<UserDrawing> ShareWithUserAsync(string email, string drawingId) {
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

            UserDrawing ud = new UserDrawing();
            ud.Drawing = drawing;
            ud.User = user;

            await _context.UserDrawings.AddAsync(ud);
            await _context.SaveChangesAsync();

            await UpdateLastModifiedAsync(drawing);
            return ud;
        }

        public async Task<string> UnshareWithUserAsync(string userDrawingId) {
            UserDrawing ud;
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
                .OrderByDescending(d => d.LastModified)
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
                .OrderByDescending(d => d.LastModified)
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
    }

    public interface IDrawingService {
        Task<string> CreateDrawingAsync(string userId, Drawing drawing);
        Task<string> CreateDrawingFromTemplateAsync(string userId, Drawing drawing, Template template);
        Task<Drawing> GetDrawingAsync(string id);
        Task<PrintDrawing> GetPrintDrawingAsync(string id);
        Task DeleteDrawingAsync(string id);
        Task<bool> DrawingExistsAsync(string id);
        Task<List<UserDrawing>> GetSharedUsersAsync(string drawingId);
        Task<UserDrawing> ShareWithUserAsync(string email, string drawingId);
        Task<string> UnshareWithUserAsync(string userDrawingId);
        Task<bool> IsSharedWithAsync(string drawingId, string userId);
        Task<bool> IsOwnerAsync(string drawingId, string userId);
        Task<List<Drawing>> GetOwnedDrawingsAsync(string userId);
        Task<List<Drawing>> GetSharedDrawingsAsync(string userId);
        Task UpdateLastModifiedAsync(Drawing drawing);
    }
}