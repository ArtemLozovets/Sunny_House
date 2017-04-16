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
            Мужской,
            Женский
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
            PotentialClient = new HashSet<PotentialСlient>();
        }

        public int PersonId { get; set; }

        //[Required(ErrorMessage = "Необходимо заполнить поле \"Фамилия\"")]
        [StringLength(50, ErrorMessage = "Должно быть не более 50 символов")]
        [Display(Name = "Фамилия")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Имя\"")]
        [StringLength(30, ErrorMessage = "Должно быть не более 30 символов")]
        [Display(Name = "Имя")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Необходимо заполнить поле \"Отчество\"")]
        [StringLength(100, ErrorMessage = "Должно быть не более 100 символов")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Необходимо указать пол")]
        [Range(0, 1, ErrorMessage = "Выберите пол")]
        [EnumDataType(typeof(SexTypes))]
        [Display(Name = "Пол")]
        public SexTypes? Sex { get; set; }

        //[Required(ErrorMessage = "Необходимо заполнить поле \"Дата рождения\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(1000, ErrorMessage = "Примечание должно быть не более 1000 символов ")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage="Необходимо указать дату добавления персоны")]
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
        public virtual ICollection<PotentialСlient> PotentialClient { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<STask> STask { get; set; }

    }
}
