namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Visit")]
    public partial class Visit
    {
        public int VisitId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать персону")]
        public int VisitorId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать занятие")]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать роль")]
        public int RoleId { get; set; }

        [StringLength(1000, ErrorMessage = "Примечание должно быть не более 1000 символов ")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual Person Person { get; set; }

        public virtual PersonRole PersonRole { get; set; }

        [NotMapped]
        [Display(Name = "ФИО персоны")]
        public string PersonFIO { get; set; }

        [NotMapped]
        [Display(Name = "Занятие")]
        public string ExName { get; set; }

        [NotMapped]
        [Display(Name = "Роль")]
        public string RoleName { get; set; }

        [NotMapped]
        [Display(Name = "Дата")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        [NotMapped]
        public int EventId { get; set; }

        [NotMapped]
        [Display(Name = "Мероприятие")]
        public string EventName { get; set; }
    }
}
