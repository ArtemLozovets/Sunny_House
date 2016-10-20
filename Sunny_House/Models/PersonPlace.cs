namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonPlace")]
    public partial class PersonPlace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PersonId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AddressId { get; set; }

        [Display(Name = "��� �����")]
        [StringLength(50, ErrorMessage = "������ ���� �� ����� 50 ��������")]
        [Required(ErrorMessage="���������� ��������� ���� \"��� �����\"")]
        public string PlaceName { get; set; }

        public virtual Address Address { get; set; }

        public virtual Person Person { get; set; }
    }
}
