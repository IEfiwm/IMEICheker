using AutoMapper;
using Domain.Entities.Data;
using Web.Areas.Data.Models;

namespace Web.Areas.Data.Mappings
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<DeviceViewModel, Imported>().ReverseMap();
        }
    }
}
