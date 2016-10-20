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

        
        [Required(ErrorMessage = "���������� ������� ����� �������")]
        [RegularExpression("(^\\d{1,5}(\\,\\d{1,2})?$)", ErrorMessage = "��������� ������������ ����� �����")]
        [Display(Name = "�����")]
        public decimal Sum { get; set; }

        [Required(ErrorMessage = "���������� ��������� ����  \"����\"")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "����")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(1000, ErrorMessage = "���������� ������ ���� �� ������ 1000 �������� ")]
        [Display(Name = "����������")]
        public string Note { get; set; }

        [Required(ErrorMessage = "���������� ������� �����������")]
        [Display(Name = "����������")]
        public int PayerId { get; set; }

        [Required(ErrorMessage = "���������� ������� �������")]
        [Display(Name = "������")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "���������� ������� �����������")]
        [Display(Name = "�����������")]
        public int EventId { get; set; }

        public virtual Person Person { get; set; }

        public virtual Person Person1 { get; set; }

        public virtual Event Event { get; set; }
    }
}
