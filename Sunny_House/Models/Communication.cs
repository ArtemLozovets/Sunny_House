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

        [Display(Name = "Адрес/Номер")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Адрес/Номер\"")]
        [StringLength(50, ErrorMessage = "Должно быть не более 50 символов")]
        public string Address_Number { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public int TypeOfCommunicationId { get; set; }

        [Display(Name = "Название")]
        [StringLength(100, ErrorMessage = "Должно быть не более 100 символов")]
        public string CommName { get; set; }

        [StringLength(1000, ErrorMessage = "Должно быть не более 1000 символов")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        public virtual TypeOfCommunication TypeOfCommunication { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonCommunication> PersonCommunication { get; set; }
    }
}
