using AuctionPortal.ApplicationLayer.Application;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Controllers;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Models;
using AuctionPortal.Hubs;
using AuctionPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AuctionPortal.Controllers
{
    public class InspectionsController : APIBaseController
    {
        #region Constructor

        public InspectionsController(
            IInspectionApplication inspectionApplication,
            IHeaderValue headerValue,
            IConfiguration configuration,
            IAdminNotificationApplication adminNotificationApplication,
            IHubContext<NotificationHub> hubContext,
            IDocumentFileApplication documentFileApplication)
            : base(headerValue, configuration)
        {
            InspectionApplication = inspectionApplication
                ?? throw new ArgumentNullException(nameof(inspectionApplication));

            _adminNotificationApplication = adminNotificationApplication
                ?? throw new ArgumentNullException(nameof(adminNotificationApplication));

            _hubContext = hubContext
                ?? throw new ArgumentNullException(nameof(hubContext));

            _documentFileApplication = documentFileApplication
                ?? throw new ArgumentNullException(nameof(documentFileApplication));
        }

        #endregion

        #region Properties and Data Members

        public IInspectionApplication InspectionApplication { get; }

        private readonly IAdminNotificationApplication _adminNotificationApplication;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IDocumentFileApplication _documentFileApplication;

        #endregion

        // ---------------- existing JSON endpoints ----------------

        [HttpPost("add")]
        public async Task<int> Add([FromBody] Inspection inspection)
        {
            var inspectionId = await InspectionApplication.Add(inspection);
            return inspectionId;
        }

        [HttpPut("update")]
        public async Task<bool> Update([FromBody] Inspection inspection)
        {
            var response = await InspectionApplication.Update(inspection);
            return response;
        }

        [HttpPut("activate")]
        public async Task<bool> Activate([FromBody] Inspection inspection)
        {
            var response = await InspectionApplication.Activate(inspection);
            return response;
        }

        [HttpGet("get")]
        public async Task<Inspection> Get([FromQuery] Inspection request)
        {
            var response = await InspectionApplication.Get(request);
            return response;
        }

        [HttpGet("getlist")]
        public async Task<List<Inspection>> GetList([FromQuery] Inspection request)
        {
            var response = await InspectionApplication.GetList(request);
            return response;
        }

        [HttpGet("getbyinventory")]
        public async Task<List<Inspection>> GetByInventory([FromQuery] Inspection request)
        {
            var response = await InspectionApplication.GetByInventory(request);
            return response;
        }

        // ---------------- NEW: add inspection WITH IMAGE ----------------

        /// <summary>
        /// For checkpoints where InputType = 'Image'.
        /// 1) Uploads the image to /wwwroot/uploads.
        /// 2) Inserts a row into DocumentFile (for metadata).
        /// 3) Inserts an Inspection row with Result = uploaded image URL.
        /// Returns the newly created InspectionId.
        /// </summary>
        [HttpPost("add-with-image")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(50_000_000)] // 50 MB
        public async Task<int> AddWithImage(
            IFormFile file,
            [FromForm] int inspectionTypeId,
            [FromForm] int inspectionCheckpointId,
            [FromForm] int inventoryId,
            // document type id you use for inspection photos (pass from client)
            [FromForm] int documentTypeId,
            [FromForm] int? createdById,
            [FromForm] string? documentName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided.");

            // ---------- 1) Save physical file ----------
            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadDir = Path.Combine(webRoot, "uploads");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png" }; // image-only for checkpoints

            if (string.IsNullOrWhiteSpace(ext) || Array.IndexOf(allowed, ext) < 0)
                throw new InvalidOperationException("File type not allowed. Only image files are supported.");

            var baseName = string.IsNullOrWhiteSpace(documentName)
                ? Path.GetFileNameWithoutExtension(file.FileName)
                : documentName.Trim();

            var safeBase = baseName.Replace(" ", "_");
            var uniqueName = $"{safeBase}_{Guid.NewGuid():N}{ext}";

            var fullPath = Path.Combine(uploadDir, uniqueName);
            await using (var fs = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(fs);
            }

            // Build a public URL like: https://host/uploads/xxx.png
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var publicUrl = $"{baseUrl}/uploads/{uniqueName}";

            // ---------- 2) Insert DocumentFile metadata ----------
            var docModel = new DocumentFile
            {
                DocumentName = baseName,
                DocumentTypeId = documentTypeId,
                DocumentExtension = ext.TrimStart('.'),
                DocumentUrl = publicUrl,
                CreatedById = createdById
            };

            // We don't need the id for anything right now, but this will
            // also validate and apply your audit fields.
            var documentFileId = await _documentFileApplication.Upload(docModel);

            // ---------- 3) Insert Inspection row with Result = URL ----------
            var inspection = new Inspection
            {
                InspectionTypeId = inspectionTypeId,
                InspectionCheckpointId = inspectionCheckpointId,
                InventoryId = inventoryId,
                Result = publicUrl,          // <--- key part
                CreatedById = createdById
            };

            var inspectionId = await InspectionApplication.Add(inspection);
            return inspectionId;
        }
    }
}
