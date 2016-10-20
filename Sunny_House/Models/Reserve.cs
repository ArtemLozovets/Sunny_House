namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reserve")]
    public partial class Reserve
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReserveId { get; set; }

        [Required(ErrorMessage = "���������� ������� �������")]
        [Display(Name = "�������")]
        public int PersonId { get; set; }

        [Required(ErrorMessage = "���������� ������� �����������")]
        [Display(Name = "�����������")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "���������� ������� ���� �������")]
        [Display(Name = "����")]
        public int RoleId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "���������� ������� ���� ������������")]
        public DateTime? ReserveDate { get; set; }

        [StringLength(1000, ErrorMessage="������ ���� �� ����� 1000 ��������")]
        [Display(Name = "����������")]
        public string Note { get; set; }

        public virtual Event Event { get; set; }
        public virtual Person Person { get; set; }
        public virtual PersonRole PersonRole { get; set; }
    }
}
