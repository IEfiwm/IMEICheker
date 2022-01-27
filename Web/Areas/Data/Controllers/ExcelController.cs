using Application.Enums;
using Application.Extensions;
using Common.Enums;
using Domain.Entities.Base.Identity;
using Domain.Entities.Basic;
using Domain.Entities.Data;
using Infrastructure.Repositories.Application;
using Infrastructure.Repositories.Application.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Areas.Data.Models;
using Web.Controllers;

namespace Web.Areas.Attendance.Controllers
{
    [Area("Data")]
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    public class ExcelController : BaseController<Imported>
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly IimportedRepository _repository;

        public ExcelController(IHostingEnvironment hostingEnvironment,
            IimportedRepository repository,
            UserManager<ApplicationUser> userManager,
            IBankAccountRepository bankAccountRepository,
            IProjectRepository projectRepository)
        {
            _hostingEnvironment = hostingEnvironment;
            _repository = repository;
        }

        [HttpGet]
        public IActionResult ImportDevice()
        {
            return View("Devices");
        }

        [HttpPost]
        public async Task<bool> ImportDevices()
        {
            try
            {
                IFormFile file = Request.Form.Files[0];

                string folderName = "UploadExcel";

                string webRootPath = _hostingEnvironment.WebRootPath;

                string newPath = Path.Combine(webRootPath, folderName);

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                if (file.Length > 0)
                {
                    string sFileExtension = Path.GetExtension(file.FileName).ToLower();

                    ISheet sheet;

                    string fullPath = Path.Combine(newPath, file.FileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);

                        stream.Position = 0;

                        if (sFileExtension == ".xls")
                        {
                            HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  

                            sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
                        }

                        else
                        {
                            XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  

                            sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
                        }

                        for (int j = 1; j < sheet.LastRowNum - 1; j++)
                        {
                            var row = sheet.GetRow(j);

                            var model = new Imported
                            {
                                IMEI = row?.GetCell(0)?.ToString(),
                                ActiveCode = row?.GetCell(1)?.ToString(),
                                IsDeleted = false
                            };

                            await _repository.InsertAsync(model);
                        }
                        if (_repository.SaveChanges() > 0)
                            _notify.Success("فایل با موفقیت در سیستم اضافه شد.");
                        else
                            _notify.Error("اضافه کردن فایل با خطا مواجعه شد.");
                    }
                }
                return true;
            }
            catch (System.Exception e)
            {
                _notify.Error("اضافه کردن فایل با خطا مواجعه شد.");

                return false;
            }
        }

        [HttpGet]
        public IActionResult Devices()
        {
            return View("Index");
        }

        public async Task<IActionResult> LoadAll()
        {
            var list = await _repository.GetListAsync();

            var model = _mapper.Map<IEnumerable<DeviceViewModel>>(list);

            return PartialView("_ViewAll", model);
        }

        [HttpGet]
        public async Task<bool> DeleteDevice(long id)
        {
            var model = await _repository.GetByIdAsync(id);

            model.IsDeleted = true;

            return (await _repository.SaveChangesAsync()) > 0;
        }
    }
}
