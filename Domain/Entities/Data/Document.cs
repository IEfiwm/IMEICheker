using Common.Enums;
using Domain.Base.Entity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Data
{
    public class Document : IdentityBaseEntity
    {
        public string FilePath { get; set; }

        public string FileName { get; set; }

        public DocumentType DocumentType { get; set; }

        public bool IsConfirm { get; set; } = false;

        public long ImportedDataRef { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime? ConfirmDate { get; set; }

        public string Description { get; set; }

        [ForeignKey("ImportedDataRef")]
        public virtual Imported ImportedData { get; set; }
    }
}