using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using openld.Models;
using openld.Data;

namespace openld.Services {
    public class ViewService : IViewService {
        private readonly OpenLDContext _context;
        public ViewService(OpenLDContext context) {
            _context = context;
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

            await UpdateLastModifiedAsync(newView);

            return newView;
        }

        public async Task DeleteViewAsync(string viewId) {
            View view;
            try {
                view = await _context.Views.Include(v => v.Drawing).FirstAsync(v => v.Id == viewId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            await UpdateLastModifiedAsync(view);

            if (_context.Views.AsNoTracking().Where(v => v.Drawing.Id == view.Drawing.Id).Count() == 1) {
                throw new InvalidOperationException("Cannot delete last view in drawing");
            }

            _context.Views.Remove(view);
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

        public async Task<List<UsedFixtureResult>> GetUsedFixturesAsync(string id) {
            View view;
            try {
                view = await _context.Views.Include(v => v.Drawing).FirstAsync(v => v.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            List<Structure> structures = await _context.Structures.Where(s => s.View.Id == id).ToListAsync();

            List<RiggedFixture> riggedFixtures = await _context.RiggedFixtures
                .Include(f => f.Fixture)
                .Where(rf => structures.Contains(rf.Structure))
                .ToListAsync();

            return riggedFixtures
                .GroupBy(rf => rf.Fixture)
                .Select(g => new UsedFixtureResult {Fixture = g.Key, Count = g.Count()})
                .ToList();
        }

    }

    public interface IViewService {
        Task<Drawing> GetDrawingAsync(View view);
        Task<View> CreateViewAsync(View view);
        Task DeleteViewAsync(string viewId);
        Task UpdateLastModifiedAsync(View view);
        Task<List<UsedFixtureResult>> GetUsedFixturesAsync(string id);
    }
}