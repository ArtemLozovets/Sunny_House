namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Persons")]
    public partial class Person
    {
        public enum SexTypes
        {
            �������,
            �������
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            Comment = new HashSet<Comment>();
            Comment1 = new HashSet<Comment>();
            Event = new HashSet<Event>();
            Payment = new HashSet<Payment>();
            Payment1 = new HashSet<Payment>();
            PersonCommunication = new HashSet<PersonCommunication>();
            PersonPlace = new HashSet<PersonPlace>();
            PersonRelation = new HashSet<PersonRelation>();
            PersonRelation1 = new HashSet<PersonRelation>();
            Reserve = new HashSet<Reserve>();
            Visit = new HashSet<Visit>();
            PotentialClient = new HashSet<Potential�lient>();
        }

        public int PersonId { get; set; }

        //[Required(ErrorMessage = "���������� ��������� ���� \"�������\"")]
        [StringLength(50, ErrorMessage = "������ ���� �� ����� 50 ��������")]
        [Display(Name = "�������")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "���������� ��������� ���� \"���\"")]
        [StringLength(30, ErrorMessage = "������ ���� �� ����� 30 ��������")]
        [Display(Name = "���")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "���������� ��������� ���� \"��������\"")]
        [StringLength(100, ErrorMessage = "������ ���� �� ����� 100 ��������")]
        [Display(Name = "��������")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "���������� ������� ���")]
        [Range(0, 1, ErrorMessage = "�������� ���")]
        [EnumDataType(typeof(SexTypes))]
        [Display(Name = "���")]
        public SexTypes? Sex { get; set; }

        //[Required(ErrorMessage = "���������� ��������� ���� \"���� ��������\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "���� ��������")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(1000, ErrorMessage = "���������� ������ ���� �� ����� 1000 �������� ")]
        [Display(Name = "����������")]
        public string Note { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage="���������� ������� ���� ���������� �������")]
        public DateTime? CreateDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Event> Event { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payment1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonCommunication> PersonCommunication { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonPlace> PersonPlace { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonRelation> PersonRelation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonRelation> PersonRelation1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reserve> Reserve { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Visit> Visit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Potential�lient> PotentialClient { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STask> STask { get; set; }

    }
}
