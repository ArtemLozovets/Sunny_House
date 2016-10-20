namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TypeOfCommunication")]
    public partial class TypeOfCommunication
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TypeOfCommunication()
        {
            Communication = new HashSet<Communication>();
        }

        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Вид связи")]
        [Required(ErrorMessage="Обязательное поле")]
        [StringLength(50, ErrorMessage = "Должно быть не более 50 символов")]
        public string CommType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Communication> Communication { get; set; }
    }
}
