namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Address()
        {
            Comment = new HashSet<Comment>();
            PersonPlace = new HashSet<PersonPlace>();
            Exercise = new HashSet<Exercise>();
        }

        public int AddressId { get; set; }

        [Display(Name = "���������")]
        [StringLength(100, ErrorMessage = "������ ���� �� ����� 100 ��������")]
        [Required(ErrorMessage = "���������� ��������� ���� \"���������\"")]
        public string Alias { get; set; }

        [Display(Name = "������")]
        [StringLength(5, ErrorMessage = "������ ���� �� ����� 5 ��������")]
        [Required(ErrorMessage = "���������� ��������� ���� \"������\"")]
        public string PostCode { get; set; }

        [Display(Name = "������")]
        [StringLength(60, ErrorMessage = "������ ���� �� ����� 60 ��������")]
        [Required(ErrorMessage = "���������� ��������� ���� \"������\"")]
        public string Country { get; set; }

        [Display(Name = "�������")]
        [StringLength(60, ErrorMessage = "������ ���� �� ����� 60 ��������")]
        [Required(ErrorMessage = "���������� ��������� ���� \"�������\"")]
        public string Region { get; set; }

        [Display(Name = "�����")]
        [StringLength(60, ErrorMessage = "������ ���� �� ����� 60 ��������")]
        [Required(ErrorMessage = "���������� ��������� ���� \"�����\"")]
        public string Area { get; set; }

        [Display(Name = "�����")]
        [StringLength(60, ErrorMessage = "������ ���� �� ����� 60 ��������")]
        [Required(ErrorMessage = "���������� ��������� ���� \"�����\"")]
        public string City { get; set; }

        [Display(Name = "�����")]
        [StringLength(100, ErrorMessage = "������ ���� �� ����� 100 ��������")]
        [Required(ErrorMessage = "���������� ��������� ���� \"�����\"")]
        public string AddressValue { get; set; }

        [StringLength(46, ErrorMessage = "������� ���� �� ����� 46 �������")]
        [Display(Name = "����������")]
        public string GeoTag { get; set; }

        [StringLength(1000, ErrorMessage = "������ ���� �� ����� 1000 ��������")]
        [Display(Name = "����������")]
        public string Note { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonPlace> PersonPlace { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exercise> Exercise { get; set; }
    }
}
