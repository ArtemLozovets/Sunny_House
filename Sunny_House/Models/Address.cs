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

        [Display(Name = "Псевдоним")]
        [StringLength(100, ErrorMessage = "Должно быть не более 100 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Псевдоним\"")]
        public string Alias { get; set; }

        [Display(Name = "Индекс")]
        [StringLength(5, ErrorMessage = "Должно быть не более 5 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Индекс\"")]
        public string PostCode { get; set; }

        [Display(Name = "Страна")]
        [StringLength(60, ErrorMessage = "Должно быть не более 60 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Страна\"")]
        public string Country { get; set; }

        [Display(Name = "Область")]
        [StringLength(60, ErrorMessage = "Должно быть не более 60 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Область\"")]
        public string Region { get; set; }

        [Display(Name = "Район")]
        [StringLength(60, ErrorMessage = "Должно быть не более 60 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Район\"")]
        public string Area { get; set; }

        [Display(Name = "Город")]
        [StringLength(60, ErrorMessage = "Должно быть не более 60 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Город\"")]
        public string City { get; set; }

        [Display(Name = "Адрес")]
        [StringLength(100, ErrorMessage = "Должно быть не более 100 символов")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Адрес\"")]
        public string AddressValue { get; set; }

        [StringLength(46, ErrorMessage = "Повинно бути не більше 46 символів")]
        [Display(Name = "Координаты")]
        public string GeoTag { get; set; }

        [StringLength(1000, ErrorMessage = "Должно быть не более 1000 символов")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonPlace> PersonPlace { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exercise> Exercise { get; set; }
    }
}
