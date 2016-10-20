using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace Sunny_House.Models
{
    [Table("RelationTypesCatalog")]
   
    public class RelationTypesCatalog
    {

        public enum RelSexTypes
        {
            Мужской,
            Женский,
            No_Gender
        }
        
        [Key]
        public int RelTypesId { get; set; }
        
        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(50, ErrorMessage = "Должно быть не более 50 символов")]
        [Display(Name = "Первичное отношение")]
        public string RelType { get; set; }

        [Required(ErrorMessage = "Выберите пол")]
        [Range(0, 2, ErrorMessage = "Выберите пол")]
        [Display(Name = "Пол (для первичного отношения)")]
        public RelSexTypes? TypeSex { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [StringLength(50, ErrorMessage = "Должно быть не более 50 символов")]
        [Display(Name = "Обратное отношение")]
        public string ReverseRelType { get; set; }

        [Required(ErrorMessage = "Выберите пол")]
        [Range(0, 2, ErrorMessage = "Выберите пол")]
        [Display(Name = "Пол (для обратного отношения)")]
        public RelSexTypes? RevTypeSex { get; set; }
    }
}