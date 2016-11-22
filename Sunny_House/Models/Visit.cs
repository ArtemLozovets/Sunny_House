namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Visit")]
    public partial class Visit
    {
        public int VisitId { get; set; }
        
        public int VisitorId { get; set; }

        public int ExerciseId { get; set; }

        public int RoleId { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual Person Person { get; set; }

        public virtual PersonRole PersonRole { get; set; }
    }
}
