namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать источник отзыва")]
        [Display(Name = "Источник")]
        public int SourceId { get; set; }

        [Display(Name = "Дата отзыва")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy HH':'mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Дата отзыва\"")]
        public DateTime Date { get; set; }

        [Display(Name = "Отзыв")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Отзыв\"")]
        [StringLength(4000, ErrorMessage = "Примечание должно быть не более 4000 символов ")]
        public string Text { get; set; }

        [Display(Name = "Оценка")]
        [Required(ErrorMessage = "Необходимо указать оценку")]
        [Range(1, Int16.MaxValue, ErrorMessage="Значение поля \"Оценка\" должно находится в диапазоне от 1 до 32767")]
        public int? Rating { get; set; }

        [Display(Name = "О персоне")]
        public int? AboutPersonId { get; set; }

        [Display(Name = "О мероприятии")]
        public int? EventId { get; set; }

        [Display(Name = "О занятии")]
        public int? ExerciseId { get; set; }

        [Display(Name = "О месте")]
        public int? AddressId { get; set; }

        [Display(Name = "Подпись")]
        public int? SignPersonId { get; set; }

        public virtual Address Address { get; set; }

        public virtual Event Event { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual Person Person { get; set; }

        public virtual Person Person1 { get; set; }

        [Display(Name = "Источник")]
        public virtual CommentSource CommentSource { get; set; }
    }
}
