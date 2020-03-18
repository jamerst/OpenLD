using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;
using openld.Utils;

namespace openld.Services {
    public class FixtureService : IFixtureService {
        private readonly OpenLDContext _context;
        public FixtureService(OpenLDContext context) {
            _context = context;
        }

        public async Task<List<Fixture>> SearchAllFixturesAsync(SearchParams search) {
            var query = _context.Fixtures.AsNoTracking()
                    .Where(f => EF.Functions.Like(f.Name.ToLower(), $"%{search.name.ToLower()}%"))
                    .Where(f => EF.Functions.Like(f.Manufacturer.ToLower(), $"%{search.manufacturer.ToLower()}%"));

            // if type specified
            if (search.type != "") {
                query = query.Where(f => f.Type.Id == search.type);
            }

            return await query
                .Include(f => f.Type)
                .OrderBy(f => f.Name)
                .ToListAsync();
        }

        public async Task CreateFixtureAsync(Fixture fixture) {
            try {
                fixture.Type = await _context.FixtureTypes.FirstAsync(t => t.Id == fixture.Type.Id);
                fixture.Image = await _context.StoredImages.FirstAsync(i => i.Id == fixture.Image.Id);
                fixture.Symbol = await _context.Symbols.Include(s => s.Bitmap).FirstAsync(i => i.Id == fixture.Symbol.Id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("Related record not found");
            }

            if (fixture.Modes == null || fixture.Modes.Count == 0) {
                fixture.Modes = new List<FixtureMode>();
                fixture.Modes.Add(new FixtureMode { Name = "Default", Channels = 1});
            }

            // if symbol bitmap not already created
            if (fixture.Symbol.Bitmap == null) {
                fixture.Symbol.Bitmap = await CreateSymbolBitmapAsync(fixture.Symbol.Path);
            }

            await _context.Fixtures.AddAsync(fixture);
            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadFixtureImageAsync(IFormFile file) {
            bool isImage = false;
            try {
                isImage = ImageUtils.IsImage(file);
            } catch (ArgumentException) {
                throw new ArgumentException("Image not valid");
            }

            if (isImage) {
                string hash = "";
                using (var fileStream = file.OpenReadStream()) {
                    hash = ImageUtils.FileHash(fileStream);
                }

                // check if already exists
                StoredImage existing = await _context.StoredImages.AsNoTracking().SingleOrDefaultAsync(i => i.Hash == hash);
                if (existing != default(StoredImage)) {
                    return existing.Id;
                }

                string filePath = Path.Combine("/openld-data/fixture-images", Path.GetRandomFileName());

                // convert image to jpeg if necessary, with white background
                await ImageUtils.SaveAsJpeg(file, filePath);

                StoredImage image = new StoredImage { Path = filePath, Hash = hash };

                await _context.StoredImages.AddAsync(image);
                await _context.SaveChangesAsync();

                return image.Id;
            } else {
                throw new ArgumentException("Image not valid");
            }
        }

        public async Task<string> UploadFixtureSymbolAsync(IFormFile file) {
            bool isSvg = false;
            try {
                isSvg = ImageUtils.isSvg(file);
            } catch (ArgumentException) {
                throw new ArgumentException("Symbol not valid");
            }

            if (isSvg) {
                string hash = "";
                using (var fileStream = file.OpenReadStream()) {
                    hash = ImageUtils.FileHash(fileStream);
                }

                Symbol existing = await _context.Symbols.AsNoTracking().SingleOrDefaultAsync(i => i.Hash == hash);
                if (existing != default(Symbol)) {
                    return existing.Id;
                }

                string filePath = Path.Combine("/openld-data/fixture-symbols", Path.GetRandomFileName());

                await ImageUtils.SaveAsOptimisedSvg(file, filePath);
                Symbol symbol = new Symbol { Path = filePath, Hash = hash };

                await _context.Symbols.AddAsync(symbol);
                await _context.SaveChangesAsync();

                return symbol.Id;
            } else {
                throw new ArgumentException("Symbol not valid");
            }
        }

        public async Task<StoredImage> CreateSymbolBitmapAsync(string svgPath) {
            string bitmapPath = Path.Combine("/openld-data/fixture-symbol-bitmaps", Path.GetRandomFileName());
            ImageUtils.SaveSvgAsPng(svgPath, bitmapPath, 500);

            string hash = "";
            using (var fileStream = new FileStream(bitmapPath, FileMode.Open, FileAccess.Read)) {
                hash = ImageUtils.FileHash(fileStream);
            }

            StoredImage bitmap = new StoredImage { Path = bitmapPath, Hash = hash};

            _context.StoredImages.Add(bitmap);
            await _context.SaveChangesAsync();

            return bitmap;
        }

        public async Task<FixtureMode> GetFixtureModeAsync(string id) {
            FixtureMode mode;
            try {
                mode = await _context.FixtureModes.FirstAsync(m => m.Id == id);
            } catch (InvalidOperationException) {
                throw new KeyNotFoundException("FixtureMode ID not found");
            }

            return mode;
        }
    }

    public interface IFixtureService {
        Task<List<Fixture>> SearchAllFixturesAsync(SearchParams search);
        Task CreateFixtureAsync(Fixture fixture);
        Task<string> UploadFixtureImageAsync(IFormFile file);
        Task<string> UploadFixtureSymbolAsync(IFormFile file);
        Task<StoredImage> CreateSymbolBitmapAsync(string svgPath);
        Task<FixtureMode> GetFixtureModeAsync(string id);
    }
}