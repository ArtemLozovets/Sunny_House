namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Exercise")]
    public partial class Exercise
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Exercise()
        {
            Comment = new HashSet<Comment>();
            Visit = new HashSet<Visit>();
        }

        [ScaffoldColumn(false)]
        public int ExerciseId { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Тема\"")]
        [StringLength(500, ErrorMessage = "Примечание должно быть не более 500 символов")]
        [Display(Name = "Тема")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Начало\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy hh:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Начало")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле \"Окончание\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy hh:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Окончание")]
        public DateTime EndTime { get; set; }

        [StringLength(1000, ErrorMessage = "Примечание должно быть не более 1000 символов")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [Required]
        public int EventId { get; set; }
        
        [Required]
        public int AddressId { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }

        public virtual Event Event { get; set; }

        public virtual Address Address { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Visit> Visit { get; set; }
    }
}
