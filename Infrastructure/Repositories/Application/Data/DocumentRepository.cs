using Application.Interfaces.Repositories;
using Domain.Entities.Data;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Application.Data
{
    public class DocumentRepository : BaseIdentityRepository<Document, ApplicationDbContext>, IDocumentRepository
    {
        public DocumentRepository(IIdentityRepositoryAsync<Document, ApplicationDbContext> repository) : base(repository)
        {
        }
    }
}
