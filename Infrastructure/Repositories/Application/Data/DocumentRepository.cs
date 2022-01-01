using Application.Interfaces.Repositories;
using Domain.Entities.Data;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Application.Data
{
    public class DocumentRepository : BaseIdentityRepository<Document, ApplicationDbContext>, IDocumentRepository
    {
        public DocumentRepository(IIdentityRepositoryAsync<Document, ApplicationDbContext> repository) : base(repository)
        {
        }

        public Task<List<Document>> GetAllDocumentAsync()
        {
            return Model
                .Include(x => x.ImportedData)
                .ToListAsync();
        }
    }
}
