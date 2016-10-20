namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("STask")]
    public partial class STask
    {
        public int STaskId { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Дата\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(100, ErrorMessage = "Должно быть не более 100 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Тема\"")]
        [Display(Name = "Тема")]
        public string Subject { get; set; }

        public bool TaskComplete { get; set; }

        [StringLength(1000, ErrorMessage = "Должно быть не более 1000 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Описание\"")]
        [Display(Name = "Описание")]
        public string Note { get; set; }

    }
}