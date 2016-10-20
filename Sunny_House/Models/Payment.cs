namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Globalization;

    [Table("Payment")]
    public partial class Payment
    {
        public int PaymentId { get; set; }

        
        [Required(ErrorMessage = "Необходимо указать сумму платежа")]
        [RegularExpression("(^\\d{1,5}(\\,\\d{1,2})?$)", ErrorMessage = "Проверьте правильность ввода суммы")]
        [Display(Name = "Сумма")]
        public decimal Sum { get; set; }

        [Required(ErrorMessage = "Необходимо заполнить поле  \"Дата\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(1000, ErrorMessage = "Примечание должго быть не больше 1000 символов ")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Необходимо указать плательщика")]
        [Display(Name = "Плательщик")]
        public int PayerId { get; set; }

        [Required(ErrorMessage = "Необходимо указать клиента")]
        [Display(Name = "Клиент")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Необходимо указать мероприятие")]
        [Display(Name = "Мероприятие")]
        public int EventId { get; set; }

        public virtual Person Person { get; set; }

        public virtual Person Person1 { get; set; }

        public virtual Event Event { get; set; }
    }
}
