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

        public virtual Exercise Exercise { get; set; }

        public virtual Person Person { get; set; }

        public virtual PersonRole PersonRole { get; set; }
    }
}
