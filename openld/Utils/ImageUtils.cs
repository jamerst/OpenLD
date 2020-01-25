using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Svg;


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

        public static bool isSvg(IFormFile file) {
            try {
                using (Stream stream = file.OpenReadStream()) {
                    SvgDocument doc = SvgDocument.Open<SvgDocument>(stream);
                }
                return true;
            } catch {
                throw new ArgumentException("Failed to verify SVG");
            }
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

        public static async Task SaveAsOptimisedSvg(IFormFile file, string filePath) {
            string tempPath = Path.GetTempFileName();
            using (var stream = File.Create(tempPath)) {
                await file.CopyToAsync(stream);
            }

            bool success = true;
            using (Process svgo = new Process()) {
                svgo.StartInfo = new ProcessStartInfo(
                    "svgo",
                    $"-i {tempPath} -o {filePath}"
                );
                svgo.Start();
                success = svgo.WaitForExit(10000);
                if (svgo.ExitCode != 0) {
                    throw new InvalidOperationException("svgo execution failed");
                }
            }

            if (!success) {
                throw new TimeoutException("svgo took longer than 10s");
            }
        }

        public static void SaveSvgAsPng(string fileIn, string fileOut, int res) {
            bool success = true;
            using (Process svgExport = new Process()) {
                svgExport.StartInfo = new ProcessStartInfo(
                    "rsvg-convert",
                    $"-h {res} {fileIn} -o {fileOut}"
                );
                svgExport.Start();
                success = svgExport.WaitForExit(10000);

                if (svgExport.ExitCode != 0) {
                    throw new InvalidOperationException("rsvg-convert execution failed");
                }
            }

            if (!success) {
                throw new TimeoutException("rsvg-convert took longer than 10s");
            }
        }

        public static string FileHash(Stream fileStream) {
            string hash = "";
            using (MemoryStream mst = new MemoryStream())
            using (var sha = SHA256.Create()) {
                fileStream.CopyTo(mst);
                hash = Convert.ToBase64String(sha.ComputeHash(mst.ToArray()));
            }
            fileStream.Position = 0;

            return hash;
        }
    }
}