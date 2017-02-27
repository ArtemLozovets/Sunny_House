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

        [Required(ErrorMessage = "���������� ������� �������")]
        public int VisitorId { get; set; }

        [Required(ErrorMessage = "���������� ������� �������")]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "���������� ������� ����")]
        public int RoleId { get; set; }

        [StringLength(1000, ErrorMessage = "���������� ������ ���� �� ����� 1000 �������� ")]
        [Display(Name = "����������")]
        public string Note { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual Person Person { get; set; }

        public virtual PersonRole PersonRole { get; set; }

        [NotMapped]
        [Display(Name = "��� �������")]
        public string PersonFIO { get; set; }

        [NotMapped]
        [Display(Name = "�������")]
        public string ExName { get; set; }

        [NotMapped]
        [Display(Name = "����")]
        public string RoleName { get; set; }

        [NotMapped]
        [Display(Name = "����")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        [NotMapped]
        public int EventId { get; set; }

        [NotMapped]
        [Display(Name = "�����������")]
        public string EventName { get; set; }
    }
}
