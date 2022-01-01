using AutoMapper;
using Domain.Entities.Data;
using Web.Areas.Admin.Models;

namespace Web.Areas.Admin.Mappings
{
    public class DeviceProfile : Profile
    {
        public DeviceProfile()
        {
            CreateMap<Document, DocumentViewModel>()
                .ForMember(m => m.IMEI, x => x.MapFrom(s => s.ImportedData.IMEI))
                .ReverseMap();
        }
    }
}