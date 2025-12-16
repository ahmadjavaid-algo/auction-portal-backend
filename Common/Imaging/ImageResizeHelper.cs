using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace AuctionPortal.Common.Imaging
{
    public static class ImageResizeHelper
    {
        /// <summary>
        /// Save a resized/compressed variant of the image to disk.
        /// - Respects maxWidth / maxHeight (preserves aspect ratio).
        /// - For JPEG, optionally keeps shrinking quality until <= maxBytes (if provided).
        /// - For PNG, just resizes; size is controlled mainly by dimensions.
        /// </summary>
        public static async Task SaveVariantAsync(
            Image image,
            string outputPath,
            string extension,
            int maxWidth,
            int maxHeight,
            int initialQuality,
            long? maxBytes = null)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));
            if (string.IsNullOrWhiteSpace(outputPath)) throw new ArgumentNullException(nameof(outputPath));

            extension = extension.ToLowerInvariant();

            // Work on a clone so the original Image instance can be reused for other variants
            using var working = image.Clone(ctx => ctx.AutoOrient());

            // Resize while keeping aspect ratio
            var scale = Math.Min(
                (double)maxWidth / working.Width,
                (double)maxHeight / working.Height);

            if (scale < 1.0)
            {
                var newWidth = (int)Math.Round(working.Width * scale);
                var newHeight = (int)Math.Round(working.Height * scale);

                working.Mutate(x => x.Resize(newWidth, newHeight));
            }

            await using var ms = new MemoryStream();

            if (extension == ".jpg" || extension == ".jpeg")
            {
                int quality = initialQuality;

                while (true)
                {
                    ms.SetLength(0);
                    ms.Position = 0;

                    // Create a NEW encoder each time (Quality is init-only)
                    var encoder = new JpegEncoder { Quality = quality };

                    await working.SaveAsJpegAsync(ms, encoder);

                    if (!maxBytes.HasValue)
                    {
                        // No size constraint -> done after first encode
                        break;
                    }

                    // If we are under the limit OR already at/below our minimum quality -> stop
                    if (ms.Length <= maxBytes.Value || quality <= 40)
                    {
                        break;
                    }

                    // Reduce quality and try again
                    quality -= 5;
                }
            }
            else if (extension == ".png")
            {
                // For PNG we mainly rely on downsizing resolution; PNG compression is lossless.
                var encoder = new PngEncoder(); // default compression
                await working.SaveAsPngAsync(ms, encoder);
            }
            else
            {
                // Fallback: treat as JPEG
                var encoder = new JpegEncoder { Quality = initialQuality };
                await working.SaveAsJpegAsync(ms, encoder);
            }

            ms.Position = 0;

            var directory = Path.GetDirectoryName(outputPath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var fs = File.Create(outputPath);
            await ms.CopyToAsync(fs);
        }
    }
}
