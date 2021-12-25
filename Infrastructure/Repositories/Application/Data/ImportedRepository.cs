using Application.Interfaces.Repositories;
using Domain.Entities.Data;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Application
{
    public class ImportedRepository : BaseIdentityRepository<Imported, ApplicationDbContext>, IimportedRepository
    {
        public ImportedRepository(
            //IDistributedCache distributedCache,
            IIdentityRepositoryAsync<Imported, ApplicationDbContext> repository
            //BaseCacheKey<Imported> baseCacheKey
            ) :
            base(
                //distributedCache, 
                repository
                //baseCacheKey
                )
        {
        }

        public async Task<Imported> GetByIMEI(string imei)
        {
            return await Model.Where(m => m.IMEI == imei).FirstOrDefaultAsync();
        }
    }
}