namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonRelation")]
    public partial class PersonRelation
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "������������ ����")]
        [Display(Name = "�������")]
        public int PersonId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "������������ ����")]
        [Display(Name = "����������� �������")]
        public int RelPersonId { get; set; }

        [Required(ErrorMessage = "������������ ����")]
        [StringLength(50, ErrorMessage = "������ ���� �� ����� 50 ��������")]
        [Display(Name = "��������� ���������")]
        public string Relation12 { get; set; }

        [Required(ErrorMessage = "������������ ����")]
        [StringLength(50, ErrorMessage = "������ ���� �� ����� 50 ��������")]
        [Display(Name = "�������� ���������")]
        public string Relation21 { get; set; }

        public virtual Person Person { get; set; }

        public virtual Person Person1 { get; set; }
    }
}
