namespace Sunny_House.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Data.Entity;
    using System.Linq;

    [Table("Comment")]
    public partial class Comment
    {
        public int CommentId { get; set; }

        [Required(ErrorMessage = "���������� ������� �������� ������")]
        [Display(Name = "��������")]
        public int SourceId { get; set; }

        [Display(Name = "���� ������")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy HH':'mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "���������� ��������� ���� \"���� ������\"")]
        public DateTime Date { get; set; }

        [Display(Name = "�����")]
        [Required(ErrorMessage = "���������� ��������� ���� \"�����\"")]
        [StringLength(4000, ErrorMessage = "���������� ������ ���� �� ����� 4000 �������� ")]
        public string Text { get; set; }

        [Display(Name = "������")]
        [Required(ErrorMessage = "���������� ������� ������")]
        [Range(1, Int16.MaxValue, ErrorMessage = "�������� ���� \"������\" ������ ��������� � ��������� �� 1 �� 32767")]
        public int? Rating { get; set; }

        [Display(Name = "� �������")]
        public int? AboutPersonId { get; set; }

        [Display(Name = "� �����������")]
        public int? EventId { get; set; }

        [Display(Name = "� �������")]
        public int? ExerciseId { get; set; }

        [Display(Name = "� �����")]
        public int? AddressId { get; set; }

        [Display(Name = "�������")]
        public int? SignPersonId { get; set; }

        [Display(Name = "������������� ��������")]
        public Guid RelGuid { get; set; }

        [Display(Name = "���������")]
        public Guid? CreatorId { get; set; }
        
        [NotMapped]
        public List<Attachment> AttListGetter
        {
            get
            {
                List<Attachment> _attList = new List<Attachment>();
                using (var db = new SunnyModel())
                {
                    _attList = db.Attachments.Where(x => x.RelGuid == RelGuid).ToList();
                }
                return _attList;
            }
            set { }
        }

        [NotMapped]
        public List<Attachment> AttList { get; set; }

        [NotMapped]
        public int AttCount { get; set; }


        public virtual Address Address { get; set; }

        public virtual Event Event { get; set; }

        public virtual Exercise Exercise { get; set; }

        public virtual Person Person { get; set; }

        public virtual Person Person1 { get; set; }

        [Display(Name = "��������")]
        public virtual CommentSource CommentSource { get; set; }

        [NotMapped]
        public string SignPersonFIO { get; set; }

        [NotMapped]
        public string AboutPersonFIO { get; set; }

    }
}
