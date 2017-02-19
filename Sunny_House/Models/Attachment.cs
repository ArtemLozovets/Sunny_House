using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Sunny_House.Models
{
    [Table("Attacment")]
    public partial class Attachment
    {
        public int Id { get; set; }

        [Display(Name = "Имя файла")]
        public string FileName { get; set; }

        [Display(Name = "Идентификатор файла")]
        public Guid ServerFileName { get; set; }

        [Display(Name = "Идентификатор сущности")]
        public Guid RelGuid { get; set; }

        public DateTime CreateDT { get; set; }
       
    }

}
