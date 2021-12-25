using Application.Interfaces.Repositories.Base;
using Domain.Entities.Data;
using Infrastructure.DbContexts;

namespace Infrastructure.Repositories.Application.Data
{
    public interface IDocumentRepository : IBaseIdentityRepository<Document, ApplicationDbContext>
    {
    }
}