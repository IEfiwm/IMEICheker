using Domain.Entities.Data;
using Infrastructure.Repositories.Application;
using Infrastructure.Repositories.Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Web.Areas.Device.Models;

namespace Web.Areas.Device.Controllers
{
    [Area("Device")]
    [AllowAnonymous]
    public class ActiveController : Controller
    {
        private readonly IimportedRepository _importedRepository;

        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly IDocumentRepository _documentRepository;

        public ActiveController(IimportedRepository importedRepository,
            IHostingEnvironment hostingEnvironment,
            IDocumentRepository documentRepository)
        {
            _importedRepository = importedRepository;
            _hostingEnvironment = hostingEnvironment;
            _documentRepository = documentRepository;
        }

        public IActionResult Confirm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OnConfirmAsync(OnConfirmViewModel model)
        {
            var data = await _importedRepository.GetByIMEI(model.DeviceIMEI);

            IFormFile file01 = Request.Form.Files[0];

            IFormFile file02 = Request.Form.Files[1];

            string folderName = "UploadedDocs";

            string webRootPath = _hostingEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            if (file01.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + "." + file01.FileName.Split(".")[1];

                string fullPath = Path.Combine(newPath, filename);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file01.CopyTo(stream);
                }

                await _documentRepository.InsertAsync(new Document
                {
                    FileName = filename,
                    FilePath = fullPath,
                    DocumentType = Common.Enums.DocumentType.NationalCard,
                    ImportedDataRef = data.Id
                });
            }

            if (file02.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + "." + file02.FileName.Split(".")[1];

                string fullPath = Path.Combine(newPath, filename);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file02.CopyTo(stream);
                }

                await _documentRepository.InsertAsync(new Document
                {
                    FileName = filename,
                    FilePath = fullPath,
                    DocumentType = Common.Enums.DocumentType.OrderPicture,
                    ImportedDataRef = data.Id
                });
            }

            var res = await _documentRepository.SaveChangesAsync() > 0;

            return Ok(res);
        }

        public async Task<IActionResult> CheckIMEIIsExist(string imei)
        {
            if ((await _importedRepository.GetByIMEI(imei)) == null)
            {
                return Ok(false);
            }

            return Ok(true);
        }
    }
}
