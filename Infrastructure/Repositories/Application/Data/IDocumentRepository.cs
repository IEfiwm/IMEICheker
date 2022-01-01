using Application.Interfaces.Repositories.Base;
using Domain.Entities.Data;
using Infrastructure.DbContexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Application.Data
{
    public interface IDocumentRepository : IBaseIdentityRepository<Document, ApplicationDbContext>
    {
        public Task<List<Document>> GetAllDocumentAsync();
    }
}