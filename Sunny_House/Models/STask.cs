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

        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Завершение")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Дата создания\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Создание")]
        [DataType(DataType.Date)]
        public DateTime DateOfCreation { get; set; }

        [StringLength(100, ErrorMessage = "Должно быть не более 100 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Тема\"")]
        [Display(Name = "Тема")]
        public string Subject { get; set; }

        [Display(Name = "Вып.")]
        public bool TaskComplete { get; set; }

        [StringLength(1000, ErrorMessage = "Должно быть не более 1000 символов")]
        [Display(Name = "Описание")]
        public string Note { get; set; }

        [Display(Name = "Создатель")]
        public Guid? CreatorId { get; set; }

        public int? ResponsibleId { get; set; }
        public virtual Person Person { get; set; }

        [NotMapped]
        [Display(Name = "Ответственный")]
        public string PersonFIO { get; set; }

    }
}