using System.ComponentModel.DataAnnotations;

namespace Common.Enums
{
    public enum DocumentType
    {
        [Display(Name = "عکس فاکتور")]
        OrderPicture = 0,

        [Display(Name = "کارت ملی")]
        NationalCard = 1,

        [Display(Name = "عکس کارتون دستگاه")]
        DeviceBoxPicture = 2,
    }
}
