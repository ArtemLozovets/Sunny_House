namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Communication")]
    public partial class Communication
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Communication()
        {
            PersonCommunication = new HashSet<PersonCommunication>();
        }

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "�����/�����")]
        [Required(ErrorMessage = "���������� ��������� ���� \"�����/�����\"")]
        [StringLength(50, ErrorMessage = "������ ���� �� ����� 50 ��������")]
        public string Address_Number { get; set; }

        [Required(ErrorMessage = "������������ ����")]
        public int TypeOfCommunicationId { get; set; }

        [Display(Name = "��������")]
        [StringLength(100, ErrorMessage = "������ ���� �� ����� 100 ��������")]
        public string CommName { get; set; }

        [StringLength(1000, ErrorMessage = "������ ���� �� ����� 1000 ��������")]
        [Display(Name = "����������")]
        public string Note { get; set; }

        public virtual TypeOfCommunication TypeOfCommunication { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonCommunication> PersonCommunication { get; set; }
    }
}
