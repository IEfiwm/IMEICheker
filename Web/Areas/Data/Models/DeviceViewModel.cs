namespace Web.Areas.Data.Models
{
    public class DeviceViewModel
    {
        public long Id { get; set; }

        public string IMEI { get; set; }

        public string ActiveCode { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsUsed { get; set; }
    }
}
