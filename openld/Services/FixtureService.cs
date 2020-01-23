using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
                    .Where(f => EF.Functions.ILike(f.Name, $"%{search.name}%"))
                    .Where(f => EF.Functions.ILike(f.Manufacturer, $"%{search.manufacturer}%"));

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
            fixture.Type = await _context.FixtureTypes.FirstOrDefaultAsync(t => t.Id == fixture.Type.Id);
            fixture.Image = await _context.StoredImages.FirstOrDefaultAsync(i => i.Id == fixture.Image.Id);

            if (fixture.Type == default(FixtureType)) {
                throw new KeyNotFoundException("Type not found");
            } else if (fixture.Image == default(StoredImage)) {
                throw new KeyNotFoundException("Image not found");
            }

            if (fixture.Modes == null || fixture.Modes.Count == 0) {
                fixture.Modes = new List<FixtureMode>();
                fixture.Modes.Add(new FixtureMode { Name = "Default", Channels = new[] {"1"} });
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
                    // check file not already uploaded
                    using (MemoryStream mst = new MemoryStream())
                    using (var sha = SHA256.Create()) {
                        fileStream.CopyTo(mst);
                        hash = Convert.ToBase64String(sha.ComputeHash(mst.ToArray()));

                        StoredImage existing = await _context.StoredImages.AsNoTracking().SingleOrDefaultAsync(i => i.Hash == hash);
                        if (existing != default(StoredImage)) {
                            return existing.Id;
                        }
                    }
                    fileStream.Position = 0;
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
        Task<FixtureMode> GetFixtureModeAsync(string id);
    }
}