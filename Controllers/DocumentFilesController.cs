using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Imaging;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
namespace AuctionPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentFilesController : APIBaseController
    {
        #region Ctor / fields

        private readonly IDocumentFileApplication _documentFileApplication;

        public DocumentFilesController(
            IDocumentFileApplication documentFileApplication,
            IHeaderValue headerValue,
            IConfiguration configuration)
            : base(headerValue, configuration)
        {
            _documentFileApplication = documentFileApplication
                ?? throw new ArgumentNullException(nameof(documentFileApplication));
        }

        #endregion

        private static readonly string[] AllowedExtensions =
            { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };

        private static bool IsImageExtension(string ext)
        {
            ext = ext.ToLowerInvariant();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png";
        }

        /// <summary>
        /// Upload a file and create a DocumentFile entry.
        /// Images:
        ///     - original variant: max 1920x1920, JPEG/PNG compressed, target <= 5MB (JPEG).
        ///     - thumbnail: max 400x400, more aggressive compression.
        /// Non-images:
        ///     - only original; thumbnail is null.
        /// </summary>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<int> Upload(
            IFormFile file,
            [FromForm] int documentTypeId,
            [FromForm] string? documentName,
            [FromForm] int? createdById)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided.", nameof(file));

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext))
                throw new InvalidOperationException("File extension missing.");

            ext = ext.ToLowerInvariant();

            if (Array.IndexOf(AllowedExtensions, ext) < 0)
                throw new InvalidOperationException("File type not allowed.");

            // Decide final base name
            var baseName = string.IsNullOrWhiteSpace(documentName)
                ? Path.GetFileNameWithoutExtension(file.FileName)
                : documentName.Trim();

            var safeBase = baseName.Replace(" ", "_");
            var uniqueName = $"{safeBase}_{Guid.NewGuid():N}{ext}";

            // Folders
            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsRoot = Path.Combine(webRoot, "uploads");
            var originalsDir = Path.Combine(uploadsRoot, "originals");
            var thumbsDir = Path.Combine(uploadsRoot, "thumbs");

            if (!Directory.Exists(originalsDir)) Directory.CreateDirectory(originalsDir);
            if (!Directory.Exists(thumbsDir)) Directory.CreateDirectory(thumbsDir);

            var originalPath = Path.Combine(originalsDir, uniqueName);
            string? thumbPath = null;

            if (IsImageExtension(ext))
            {
                // IMAGE: load once and generate both variants via ImageSharp
                await using var inputStream = file.OpenReadStream();
                using var image = await Image.LoadAsync(inputStream);

                // Original (display) – higher resolution, moderate compression, tries to stay <= 5MB
                await ImageResizeHelper.SaveVariantAsync(
                    image,
                    originalPath,
                    ext,
                    maxWidth: 1920,
                    maxHeight: 1920,
                    initialQuality: 85,
                    maxBytes: 5L * 1024 * 1024 // 5 MB
                );

                // Thumbnail – small, heavily compressed, target ≈ 300KB
                thumbPath = Path.Combine(thumbsDir, uniqueName);

                await ImageResizeHelper.SaveVariantAsync(
                    image,
                    thumbPath,
                    ext,
                    maxWidth: 400,
                    maxHeight: 400,
                    initialQuality: 70,
                    maxBytes: 300L * 1024 // ~300 KB
                );
            }
            else
            {
                // NON-IMAGE: just save original once; no thumbnail
                if (!Directory.Exists(originalsDir))
                    Directory.CreateDirectory(originalsDir);

                await using var fs = System.IO.File.Create(originalPath);
                await file.CopyToAsync(fs);
            }

            // Build public URLs
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            string MakeUrl(string folder, string fileName)
                => $"{baseUrl}/uploads/{folder}/{fileName}";

            var originalUrl = MakeUrl("originals", uniqueName);
            string? thumbUrl = thumbPath != null
                ? MakeUrl("thumbs", uniqueName)
                : null;

            // Persist in DB
            var model = new DocumentFile
            {
                DocumentName = baseName,
                DocumentTypeId = documentTypeId,
                DocumentExtension = ext.TrimStart('.'),
                DocumentUrl = originalUrl,
                DocumentThumbnailUrl = thumbUrl,
                CreatedById = createdById
            };

            var id = await _documentFileApplication.Upload(model);
            return id;
        }
    }
}
