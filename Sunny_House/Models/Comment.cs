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

        public int SourceId { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Display(Name = "Комментарий")]
        [Required]
        public string Text { get; set; }

        [Display(Name = "Оценка")]
        [Required]
        public string Rating { get; set; }

        [Display(Name = "Подпись")]
        [Required]
        public string Signature { get; set; }

        public int AboutPersonId { get; set; }

        public int EventId { get; set; }

        public int ExerciseId { get; set; }

        public int AddressId { get; set; }

        public int SignPersonId { get; set; }

        public virtual Address Address { get; set; }

        public virtual Event Event { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual Person Person { get; set; }

        public virtual Person Person1 { get; set; }

        public virtual CommentSource CommentSource { get; set; }
    }
}
