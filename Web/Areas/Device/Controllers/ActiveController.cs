using Application.Extensions;
using Domain.Entities.Data;
using ImageMagick;
using Infrastructure.Repositories.Application;
using Infrastructure.Repositories.Application.Data;
using Infrastructure.Repositories.Application.Idenitity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
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

        private readonly IAuthenticationCodeRepository _authenticationCodeRepository;

        public ActiveController(IimportedRepository importedRepository,
            IHostingEnvironment hostingEnvironment,
            IDocumentRepository documentRepository,
            IAuthenticationCodeRepository authenticationCodeRepository)
        {
            _importedRepository = importedRepository;
            _hostingEnvironment = hostingEnvironment;
            _documentRepository = documentRepository;
            _authenticationCodeRepository = authenticationCodeRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OnConfirmAsync(OnConfirmViewModel model)
        {
            var data = await _importedRepository.GetByIMEI(model.DeviceIMEI);

            if (data.IsUsed)
            {
                return Ok(false);
            }

            if (await _authenticationCodeRepository.IsDeviceVerified(model.PhoneNumber))
            {
                return Ok(false);
            }

            IFormFile file01 = Request.Form.Files[0];

            IFormFile file02 = Request.Form.Files[1];

            IFormFile file03 = Request.Form.Files[2];

            if (model is null || file03 is null || file02 is null || file01 is null || data is null)
                return Ok(false);

            await SaveFile(file01, Common.Enums.DocumentType.NationalCard, data.Id);

            await SaveFile(file02, Common.Enums.DocumentType.OrderPicture, data.Id);

            await SaveFile(file03, Common.Enums.DocumentType.DeviceBoxPicture, data.Id);

            var res = await _documentRepository.SaveChangesAsync() > 0;

            var d = await _importedRepository.GetByIdAsync(data.Id);

            d.PhoneNumber = model.PhoneNumber;

            await _importedRepository.SaveChangesAsync();

            //if ((await _importedRepository.SaveChangesAsync()) > 0)
            await SMSProvider.SendAsync(model.PhoneNumber, model.DeviceIMEI, "DocumentsUploaded");

            return Ok(res);
        }

        public async Task<IActionResult> IsPhoneVerified(string phone)
        {
            if (await _authenticationCodeRepository.IsDeviceVerified(phone))
                return Ok(true);

            var code = await _authenticationCodeRepository.GenerateNewCode(phone);

            await SMSProvider.SendAsync($@"{phone}", code, "kalamato");

            return Ok(false);
        }

        public async Task<IActionResult> VerifyPhone(string phone, string code)
        {
            return Ok(await _authenticationCodeRepository.VerifyCode(phone, code));
        }

        public async Task<IActionResult> CheckIMEIIsExist(string imei)
        {
            //await SMSProvider.SendAsync("09127215825", "5464", "45456", "ActiveCode");

            var model = await _importedRepository.GetByIMEI(imei);

            if (model == null || model.IsUsed)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        private async Task SaveFile(IFormFile file, Common.Enums.DocumentType type, long importDataRef)
        {
            (await _documentRepository.Model.Where(m => !m.IsDeleted
            && !m.IsDeclined
            && !m.IsConfirm
            && m.ImportedDataRef == importDataRef
            && m.DocumentType == type)
                .ToListAsync())
                .ForEach(m =>
            {
                System.IO.File.Delete(m.FilePath);
                m.IsDeleted = true;
            });

            await _documentRepository.SaveChangesAsync();

            string folderName = "UploadedDocs";

            string webRootPath = _hostingEnvironment.WebRootPath;

            string newPath = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            var filename = Guid.NewGuid().ToString() + Guid.NewGuid().ToString() + "." + file.FileName.Split(".")[1];

            string fullPath = Path.Combine(newPath, filename);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            using (MagickImage image = new MagickImage(fullPath))
            {
                image.Format = image.Format; // Get or Set the format of the image.
                image.Resize(512, 512); // fit the image into the requested width and height. 
                image.Quality = 10; // This is the Compression level.
                image.Write(fullPath);
            }

            await _documentRepository.InsertAsync(new Document
            {
                FileName = filename,
                FilePath = fullPath,
                DocumentType = type,
                ImportedDataRef = importDataRef
            });
        }
    }
}