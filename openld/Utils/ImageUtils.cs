using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace openld.Utils {
    public static class ImageUtils {
        public const int ImageMinimumBytes = 512;

        public static bool IsImage(IFormFile postedFile) {
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (postedFile.ContentType.ToLower() != "image/jpg" &&
                        postedFile.ContentType.ToLower() != "image/jpeg" &&
                        postedFile.ContentType.ToLower() != "image/pjpeg" &&
                        postedFile.ContentType.ToLower() != "image/gif" &&
                        postedFile.ContentType.ToLower() != "image/x-png" &&
                        postedFile.ContentType.ToLower() != "image/png") {
                throw new ArgumentException("Invalid MIME type");
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".gif"
                && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg") {
                throw new ArgumentException("Invalid file extension");
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try {
                if (!postedFile.OpenReadStream().CanRead) {
                    throw new ArgumentException("File not readable");
                }
                //------------------------------------------
                //check whether the image size exceeding the limit or not
                //------------------------------------------
                if (postedFile.Length < ImageMinimumBytes) {
                    throw new ArgumentException("File too small");
                }

                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline)) {
                    throw new ArgumentException("Invalid file contents");
                }
            } catch (Exception) {
                throw new ArgumentException("Failed to verify file");
            }

            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------

            try {
                using (var bitmap = new Bitmap(postedFile.OpenReadStream())) {
                }
            } catch (Exception) {
                throw new ArgumentException("Failed to open file as bitmap");
            } finally {
                postedFile.OpenReadStream().Position = 0;
            }

            return true;
        }

        public static async Task SaveAsJpeg(IFormFile file, string filePath) {
            using (var fileStream = file.OpenReadStream()) {
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
            }
        }
    }
}