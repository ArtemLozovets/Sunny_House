namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            Comment = new HashSet<Comment>();
            Exercise = new HashSet<Exercise>();
            Reserve = new HashSet<Reserve>();
            Payment = new HashSet<Payment>();
            PotentialClient = new HashSet<Potential�lient>();
        }

        public int EventId { get; set; }

        [Required(ErrorMessage = "���������� ��������� ���� \"��������\"")]
        [StringLength(100, ErrorMessage = "������ ���� �� ����� 100 ��������")]
        [Display(Name = "��������")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "���������� ��������� ���� \"������\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "������")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "���������� ��������� ���� \"���������\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "���������")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "���������� ��������� ���� \"��������\"")]
        [StringLength(1000, ErrorMessage = "������ ���� �� ����� 1000 ��������")]
        [Display(Name = "��������")]
        public string Description { get; set; }

        [Required(ErrorMessage = "���������� ������� ��������������")]
        [Display(Name = "�������������")]
        public int AdministratorId { get; set; }

        [Required(ErrorMessage = "���������� ������� ���������� ����")]
        [Range(1, 300, ErrorMessage = "�������� ������ ���������� � ��������� �� 1 �� 300")]
        [Display(Name = "���������� ����")]
        public int PlacesCount { get; set; }

        [StringLength(1000, ErrorMessage = "���������� ������ ���� �� ����� 1000 ��������")]
        [Display(Name = "����������")]
        public string Note { get; set; }

        [NotMapped]
        public int ResCount { get; set; }
        
        [NotMapped]
        public int Percent { get; set; }

        [NotMapped]
        [Display(Name = "�������")]
        public string FirstName { get; set; }

        [NotMapped]
        [Display(Name = "���")]
        public string LastName { get; set; }

        [NotMapped]
        [Display(Name = "��������")]
        public string MiddleName { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        public virtual Person Person { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exercise> Exercise { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reserve> Reserve { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Potential�lient> PotentialClient { get; set; }
    }
}
