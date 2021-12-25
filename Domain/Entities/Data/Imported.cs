using Domain.Base.Entity;

namespace Domain.Entities.Data
{
    public class Imported : IdentityBaseEntity
    {
        public string IMEI { get; set; }

        public string ActiveCode { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsUsed { get; set; }
    }
}
