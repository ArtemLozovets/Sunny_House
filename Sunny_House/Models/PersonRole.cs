namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PersonRole")]
    public partial class PersonRole
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PersonRole()
        {
            Reserve = new HashSet<Reserve>();
            Visit = new HashSet<Visit>();
            PotentialСlient = new HashSet<PotentialСlient>();
        }

        [Key]
        public int RoleId { get; set; }

        [Display(Name = "Роль")]
        [Required(ErrorMessage="Необходимо заполнить поле \"Роль\"")]
        [StringLength(100, ErrorMessage="Значение не должно превышать 100 символов")]
        public string RoleName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reserve> Reserve { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Visit> Visit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PotentialСlient> PotentialСlient { get; set; }
    }
}
