namespace Sunny_House.Models
{

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PotentialСlient")]
    public partial class PotentialСlient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Необходимо указать участника")]
        [Display(Name = "Участник")]
        public int PersonId { get; set; }

        [Required(ErrorMessage = "Необходимо указать мероприятие")]
        [Display(Name = "Мероприятие")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Необходимо указать роль")]
        [Display(Name = "Роль")]
        public int RoleId { get; set; }

        [StringLength(1000, ErrorMessage = "Должно быть не более 1000 символов")]
        [Display(Name = "Информация")]
        public string Infoes { get; set; }
        public virtual Event Event { get; set; }
        public virtual Person Person { get; set; }
        public virtual PersonRole PersonRole { get; set; }
    }
}