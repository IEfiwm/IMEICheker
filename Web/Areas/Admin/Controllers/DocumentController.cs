using Application.Extensions;
using Common.Enums;
using Infrastructure.Repositories.Application;
using Infrastructure.Repositories.Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Abstractions;
using Web.Areas.Admin.Models;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public class DocumentController : BaseController<UserController>
    {
        private IWebHostEnvironment _hostingEnvironment;
        private readonly IDocumentRepository _documentRepository;
        private readonly IimportedRepository _importedRepository;

        public DocumentController(IimportedRepository importedRepository,
            IDocumentRepository documentRepository,
            IWebHostEnvironment hostingEnvironment)
        {
            _importedRepository = importedRepository;
            _documentRepository = documentRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            var current = await _documentRepository.GetAllDocumentAsync();

            var model = _mapper.Map<IEnumerable<DocumentViewModel>>(current).Where(x => !x.IsConfirm && !x.IsDeclined);

            return PartialView("_ViewAll", model);
        }

        [Route("admin/document/get/{fileName}")]
        public async Task<IActionResult> Get(string filename)
        {
            var file = await System.IO.File.ReadAllBytesAsync(Path.Combine(_hostingEnvironment.WebRootPath, $"UploadedDocs/{filename}"));

            return new FileContentResult(file, MimeTypes.GetMimeType(filename));
        }

        public async Task<bool> DeclineDoc(int id)
        {
            var doc = await _documentRepository.Model.Include(m => m.ImportedData).Where(m => m.Id == id).FirstOrDefaultAsync();

            doc.IsDeclined = true;

            if ((await _documentRepository.SaveChangesAsync()) > 0)
                await SMSProvider.SendTextAsync(doc.ImportedData.PhoneNumber, doc.ImportedData.IMEI, EnumHelper<DocumentType>.GetDisplayValue(doc.DocumentType), "DeclineDocument");

            return true;
        }

        public async Task<bool> ConfrimDoc(int id)
        {
            var doc = await _documentRepository.Model.Include(m => m.ImportedData).Where(m => m.Id == id).FirstOrDefaultAsync();

            doc.IsConfirm = true;

            //await _documentRepository.UpdateAsync(doc);

            if ((await _documentRepository.SaveChangesAsync()) > 0)
            {
                var docs = await _documentRepository.GetAllByImportedRef(doc.ImportedDataRef);

                if (docs.Any(m => m.IsConfirm && m.DocumentType == DocumentType.NationalCard) && docs.Any(m => m.IsConfirm && m.DocumentType == DocumentType.OrderPicture))
                {
                    await SMSProvider.SendAsync(doc.ImportedData.PhoneNumber, doc.ImportedData.IMEI, doc.ImportedData.ActiveCode, "ActiveCode");

                    var model = await _importedRepository.GetByIdAsync((int)doc.ImportedDataRef);

                    model.IsUsed = true;

                    await _importedRepository.SaveChangesAsync();
                }
                else
                {
                    await SMSProvider.SendTextAsync(doc.ImportedData.PhoneNumber, doc.ImportedData.IMEI, EnumHelper<DocumentType>.GetDisplayValue(doc.DocumentType), "AcceptDocument");
                }
            }

            return true;
        }
    }
}