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
            PotentialClient = new HashSet<PotentialСlient>();
        }

        public int EventId { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Название\"")]
        [StringLength(100, ErrorMessage = "Должно быть не более 100 символов")]
        [Display(Name = "Название")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Начало\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Начало")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Окончание\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Окончание")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Описание\"")]
        [StringLength(1000, ErrorMessage = "Должно быть не более 1000 символов")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать ответственного")]
        [Display(Name = "Ответственный")]
        public int AdministratorId { get; set; }

        [Required(ErrorMessage = "Необходимо указать количество мест")]
        [Range(1, 300, ErrorMessage = "Значение должно находиться в диапазоне от 1 до 300")]
        [Display(Name = "Количество мест")]
        public int PlacesCount { get; set; }

        [StringLength(1000, ErrorMessage = "Примечание должно быть не более 1000 символов")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [NotMapped]
        public int ResCount { get; set; }
        
        [NotMapped]
        public int Percent { get; set; }

        [NotMapped]
        [Display(Name = "Фамилия")]
        public string FirstName { get; set; }

        [NotMapped]
        [Display(Name = "Имя")]
        public string LastName { get; set; }

        [NotMapped]
        [Display(Name = "Отчество")]
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
        public virtual ICollection<PotentialСlient> PotentialClient { get; set; }
    }
}
