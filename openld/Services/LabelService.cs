using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;

namespace openld.Services {
    public class LabelService : ILabelService {
        private readonly OpenLDContext _context;
        private readonly IViewService _viewService;
        private readonly IMapper _mapper;

        public LabelService(OpenLDContext context, IViewService viewService, IMapper mapper) {
            _context = context;
            _viewService = viewService;
            _mapper = mapper;
        }

        public async Task<Label> GetLabelAsync(string labelId) {
            Label label;
            try {
                label = await _context.Labels.FirstAsync(l => l.Id == labelId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Label ID not found");
            }

            return label;
        }

        public async Task<Label> AddLabelAsync(Label label) {
            try {
                label.View = await _context.Views.FirstAsync(v => v.Id == label.View.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("View ID not found");
            }

            _context.Labels.Add(label);
            await _context.SaveChangesAsync();

            return label;
        }

        public async Task<View> GetViewAsync(Label label) {
            try {
                label = await _context.Labels.Include(l => l.View).FirstAsync(l => l.Id == label.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Label ID not found");
            }

            return label.View;
        }

        public async Task<Drawing> GetDrawingAsync(Label label) {
            try {
                label = await _context.Labels
                    .AsNoTracking()
                    .Include(s => s.View)
                        .ThenInclude(v => v.Drawing)
                    .FirstAsync(s => s.Id == label.Id);

            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Label ID not found");
            }

            return label.View.Drawing;
        }

        public async Task<Label> UpdatePropsAsync(Label label) {
            Label existing;
            try {
                existing = await _context.Labels
                    .FirstAsync(l => l.Id == label.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Label ID not found");
            }

            _mapper.Map(label, existing);
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<Label> UpdatePositionAsync(Label label) {
            Label existing;
            try {
                existing = await _context.Labels
                    .Include(l => l.View)
                    .FirstAsync(l => l.Id == label.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Label ID not found");
            }

            existing.Position = label.Position;
            await _context.SaveChangesAsync();

            await _viewService.UpdateLastModifiedAsync(existing.View);
            return existing;
        }

        public async Task DeleteAsync(string labelId) {
            Label label;
            try {
                label = await _context.Labels.FirstAsync(l => l.Id == labelId);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Label ID not found");
            }

            _context.Labels.Remove(label);
            await _context.SaveChangesAsync();
        }
    }

    public interface ILabelService {
        Task<Label> GetLabelAsync(string labelId);
        Task<Label> AddLabelAsync(Label label);
        Task<View> GetViewAsync(Label label);
        Task<Drawing> GetDrawingAsync(Label label);
        Task<Label> UpdatePropsAsync(Label label);
        Task<Label> UpdatePositionAsync(Label label);
        Task DeleteAsync(string labelId);
    }
}