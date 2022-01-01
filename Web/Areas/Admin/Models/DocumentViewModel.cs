using Common.Enums;
using System;

namespace Web.Areas.Admin.Models
{
    public class DocumentViewModel
    {
        public string FilePath { get; set; }

        public string FileName { get; set; }

        public DocumentType DocumentType { get; set; }

        public bool IsConfirm { get; set; }

        public long ImportedDataRef { get; set; }

        public string IMEI { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ConfirmDate { get; set; }

        public string Description { get; set; }
    }
}