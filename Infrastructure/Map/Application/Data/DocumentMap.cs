using Domain.Entities.Data;
using Infrastructure.Attribute;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Map.Application.Data
{
    [Application]
    internal class DocumentMap : IdentityBaseEntityMap<Document>
    {
        public DocumentMap() : base()
        {
        }

        public override void Map(EntityTypeBuilder<Document> builder)
        {
        }
    }
}