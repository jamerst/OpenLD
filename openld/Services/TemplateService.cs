using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;

namespace openld.Services {
    public class TemplateService : ITemplateService {
        private readonly OpenLDContext _context;
        public TemplateService(OpenLDContext context) {
            _context = context;
        }

        public async Task<List<Template>> GetTemplatesAsync() {
            return await _context.Templates.Include(t => t.Drawing).ToListAsync();
        }

        public async Task CreateTemplateAsync(string drawingId) {
            Drawing drawing;
            try {
                drawing = await _context.Drawings.FirstAsync(d => d.Id == drawingId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Drawing ID not found");
            }

            Template template = new Template();
            template.Drawing = drawing;

            _context.Templates.Add(template);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTemplateAsync(string drawingId) {
            return await _context.Templates.AsNoTracking().AnyAsync(t => t.Drawing.Id == drawingId);
        }
    }

    public interface ITemplateService {
        Task<List<Template>> GetTemplatesAsync();
        Task CreateTemplateAsync(string drawingId);
        Task<bool> IsTemplateAsync(string drawingId);
    }
}