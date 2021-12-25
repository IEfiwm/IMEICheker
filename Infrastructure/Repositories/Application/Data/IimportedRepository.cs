using Application.Interfaces.Repositories.Base;
using Domain.Entities.Data;
using Infrastructure.DbContexts;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Application
{
    public interface IimportedRepository : IBaseIdentityRepository<Imported, ApplicationDbContext>
    {
        Task<Imported> GetByIMEI(string imei);
    }
}