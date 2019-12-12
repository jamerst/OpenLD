using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using openld.Data;
using openld.Models;
using openld.Utils;

namespace openld.Controllers {
    [ApiController]
    [Route("api/library/[action]")]
    public class FixtureLibraryController {
        public class SearchParams {
            public string name;
            public string manufacturer;
            public string type;
        }
        private readonly OpenLDContext _context;

        public FixtureLibraryController(OpenLDContext context) {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<JsonResponse<List<Fixture>>>> GetFixtures(SearchParams search) {
            FixtureType type = _context.FixtureType.FirstOrDefault(t => t.Id == search.type);

            List<Fixture> fixtures;

            if (type != default(FixtureType)) {
                fixtures = _context.Fixture
                    .Where(f => EF.Functions.ILike(f.Name, $"%{search.name}%"))
                    .Where(f => EF.Functions.ILike(f.Manufacturer, $"%{search.manufacturer}%"))
                    .Where(f => f.Type == type)
                    .Include(f => f.Type)
                    .ToList();
            } else {
                fixtures = _context.Fixture
                    .Where(f => EF.Functions.ILike(f.Name, $"%{search.name}%"))
                    .Where(f => EF.Functions.ILike(f.Manufacturer, $"%{search.manufacturer}%"))
                    .Include(f => f.Type)
                    .ToList();
            }


            return new JsonResponse<List<Fixture>> { success = true, data = fixtures };
        }

        [HttpPost]
        public async Task<ActionResult<JsonResponse<object>>> CreateFixture(Fixture fixture) {
            fixture.Type = await _context.FixtureType.FirstOrDefaultAsync(t => t.Id == fixture.Type.Id);
            fixture.Image = await _context.StoredImages.FirstOrDefaultAsync(i => i.Id == fixture.Image.Id);

            if (fixture.Type == default(FixtureType)) {
                return new JsonResponse<object> { success = false, msg = "Type ID not found" };
            } else if (fixture.Image == default(StoredImage)) {
                return new JsonResponse<object> { success = false, msg = "Image ID not found" };
            }

            try {
                await _context.Fixture.AddAsync(fixture);
                await _context.SaveChangesAsync();
            } catch (Exception e) {
                return new JsonResponse<object> { success = false, msg = "Failed to create new fixture: " + e.Message };
            }

            return new JsonResponse<object> { success = true };
        }

        [HttpPost]
        public async Task<ActionResult<JsonResponse<string>>> UploadFixtureImage([FromForm] IFormFile file) {
            try {
                if (VerifyImage.IsImage(file)) {
                    using (var fileStream = file.OpenReadStream()) {
                        string hash = "";
                        // check file not already uploaded
                        using (MemoryStream mst = new MemoryStream())
                        using (var sha = SHA256.Create()) {
                            fileStream.CopyTo(mst);
                            hash = Convert.ToBase64String(sha.ComputeHash(mst.ToArray()));

                            StoredImage existing = await _context.StoredImages.AsNoTracking().SingleOrDefaultAsync(i => i.Hash == hash);
                            if (existing != default(StoredImage)) {
                                return new JsonResponse<string> { success = true, data = existing.Id };
                            }
                        }

                        string filePath = Path.Combine("/openld-data/fixture-images", Path.GetRandomFileName());

                        // convert image to jpeg if necessary, with white background
                        if (file.ContentType.ToLower() != "image/jpg" && file.ContentType.ToLower() != "image/jpeg") {
                            using (Bitmap uploaded = new Bitmap(fileStream))
                            using (Bitmap result = new Bitmap(uploaded.Width, uploaded.Height)) {
                                result.SetResolution(uploaded.HorizontalResolution, uploaded.VerticalResolution);

                                using (var g = Graphics.FromImage(result)) {
                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(uploaded, 0, 0);
                                }

                                // write file
                                using (var stream = File.Create(filePath)) {
                                    result.Save(stream, ImageFormat.Jpeg);
                                }
                            }
                        } else {
                            using (var stream = File.Create(filePath)) {
                                await file.CopyToAsync(stream);
                            }
                        }

                        StoredImage image = new StoredImage { Path = filePath, Hash = hash };

                        try {
                            await _context.StoredImages.AddAsync(image);
                            await _context.SaveChangesAsync();
                        } catch {
                            return new JsonResponse<string> { success = false, msg = "Failed to add database record" };
                        }

                        return new JsonResponse<string> { success = true, data = image.Id };
                    }

                } else {
                    return new JsonResponse<string> { success = false, msg  = "Uploaded file is not an image" };
                }
            } catch {
                return new JsonResponse<string> { success = false, msg  = "Failed to process image" };
            }
        }

        [HttpPost]
        public async Task<ActionResult<JsonResponse<List<FixtureType>>>> GetFixtureTypes() {
            List<FixtureType> types = _context.FixtureType.OrderBy(t => t.Name).ToList();

            return new JsonResponse<List<FixtureType>> { success = true, data  = types };
        }

        [HttpPost]
        public void CreateType([FromBody] string[] names) {
            foreach (string name in names) {
                FixtureType type = new FixtureType();
                type.Name = name;

                _context.FixtureType.Add(type);
                _context.SaveChanges();
            }
        }
    }
}