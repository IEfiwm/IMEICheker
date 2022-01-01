using Infrastructure.Repositories.Application;
using Infrastructure.Repositories.Application.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        private readonly IDocumentRepository _documentRepository;
        private readonly IimportedRepository _importedRepository;

        public DocumentController(IimportedRepository importedRepository,
            IDocumentRepository documentRepository)
        {
            _importedRepository = importedRepository;
            _documentRepository = documentRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            var current = await _documentRepository.GetAllDocumentAsync();

            var model = _mapper.Map<IEnumerable<DocumentViewModel>>(current);

            return PartialView("_ViewAll", model);
        }
    }
}
