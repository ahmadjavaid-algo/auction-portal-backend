using AuctionPortal.ApplicationLayer.Application;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Models;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace DocumentFilePortal.Controllers
{
    public class DocumentFilesController : APIBaseController
    {
        #region Constructor
        /// <summary>
        /// DocumentFilesController initializes class object.
        /// </summary>
        public DocumentFilesController(IDocumentFileApplication DocumentFileApplication, IHeaderValue headerValue, IConfiguration configuration)
            : base(headerValue, configuration)
        {
            this.DocumentFileApplication = DocumentFileApplication;
        }
        #endregion

        #region Properties and Data Members
        public IDocumentFileApplication DocumentFileApplication { get; }
        #endregion


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
            throw new ArgumentException("No file provided.");

        // 1) Ensure upload folder exists
        var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadDir = Path.Combine(webRoot, "uploads");
        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

        // 2) Build a safe unique name
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        // (optional) simple allow-list
        var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx" };
        if (string.IsNullOrWhiteSpace(ext) || Array.IndexOf(allowed, ext) < 0)
            throw new InvalidOperationException("File type not allowed.");

        var baseName = string.IsNullOrWhiteSpace(documentName)
            ? Path.GetFileNameWithoutExtension(file.FileName)
            : documentName.Trim();
        var safeBase = baseName.Replace(" ", "_");
        var uniqueName = $"{safeBase}_{Guid.NewGuid():N}{ext}";

        // 3) Save file to disk
        var fullPath = Path.Combine(uploadDir, uniqueName);
        await using (var fs = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(fs);
        }

        // 4) Build public URL (served by StaticFiles middleware)
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var publicUrl = $"{baseUrl}/uploads/{uniqueName}";

        // 5) Insert metadata via Application (calls SP)
        var model = new DocumentFile
        {
            DocumentName = baseName,
            DocumentTypeId = documentTypeId,
            DocumentExtension = ext.TrimStart('.'),
            DocumentUrl = publicUrl,
            CreatedById = createdById
        };

        var id = await DocumentFileApplication.Upload(model);
        return id;
    }

}
}
