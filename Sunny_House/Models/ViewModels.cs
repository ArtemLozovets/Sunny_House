using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Sunny_House.Models;

namespace Sunny_House.Models
{
    public enum SexTypes
    {
        Мужской,
        Женский
    }

    [NotMapped]
    public class PersonsViewModel
    {
        [ScaffoldColumn(false)]
        public int PersonId { get; set; }

        [Display(Name = "Фамилия")]
        public string FirstName { get; set; }

        [Display(Name = "Имя")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [EnumDataType(typeof(SexTypes))]
        [Display(Name = "Пол")]
        public SexTypes? Sex { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        
        [Display(Name = "Номер/Адрес")]
        public string Num_Address { get; set; }

        [Display(Name = "ФИО персоны")]
        public string PersonFIO { get; set; }

        [Display(Name = "Роль")]
        public string PersonRole { get; set; }

        [Display(Name = "Возраст")]
        public int? PersonAge { get; set; }

        [Display(Name = "Месяцев")]
        public int? PersonMonth { get; set; }

        [NotMapped]
        public int? RoleId { get; set; }

    }

    [NotMapped]
    public class RelationViewModel
    {
        [Display(Name = "Фамилия")]
        public string FirstName { get; set; }
        [Display(Name = "Имя")]
        public string LastName { get; set; }
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }
        [Display(Name = "Тип взаимоотношений")]
        public string RelationName { get; set; }
        [Display(Name = "ФИО персоны")]
        public string PersonFIO { get; set; }
        public int RelationId { get; set; }
        public int PersonId { get; set; }
    }

    [NotMapped]
    public class PaymentViewModel
    {
        [Key]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int PayerId { get; set; }
        
        [ScaffoldColumn(false)]
        public int ClientId { get; set; }

        [Display(Name = "Плательщик")]
        public string PayerPIB { get; set; }

        [Display(Name = "ФИО клиента")]
        public string PIB { get; set; }

        [Display(Name = "Мероприятие")]
        public string EventName { get; set; }

        [DisplayFormat(DataFormatString = "{0:F}", ApplyFormatInEditMode = true)]
        [Display(Name = "Сумма")]
        public double Sum { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Дата платежа")]
        public DateTime PayDate { get; set; }

        [Display(Name = "Примечание")]
        public string Note { get; set; }
    }

    [NotMapped]
    public class PlacesViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Тип места")]
        public string PlaceName { get; set; }
        [Display(Name = "Псевдоним")]
        public string Alias { get; set; }

        [Display(Name = "Индекс")]
        public string PostCode { get; set; }

        [Display(Name = "Страна")]
        public string Country { get; set; }

        [Display(Name = "Область")]
        public string Region { get; set; }

        [Display(Name = "Район")]
        public string Area { get; set; }

        [Display(Name = "Город")]
        public string City { get; set; }

        [Display(Name = "Адрес")]
        public string AddressValue { get; set; }

        [Display(Name = "Координаты")]
        public string GeoTag { get; set; }

        [Display(Name = "Примечание")]
        public string Note { get; set; }

    }

    [NotMapped]
    public class CommViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Display(Name = "Адрес/Номер")]
        public string Address_Number { get; set; }

        [Display(Name = "Вид связи")]
        public string TypeOfCommunication { get; set; }

        [Display(Name = "Название")]
        public string CommName { get; set; }

        [Display(Name = "Примечание")]
        public string Note { get; set; }
    }

    [NotMapped]
    public class AdPlaceEditViewModel
    {
        public PersonPlace PersonPlace { get; set; }
        public Address Address { get; set; }
    }

    [NotMapped]
    public class ReservesViewModel
    {
        [ScaffoldColumn(false)]
        public int ReserveId { get; set; }
        
        [Display(Name = "Персона")]
        public string PersonFIO { get; set; }

        [Display(Name = "Мероприятие")]
        public string EventName { get; set; }

        [Display(Name = "Мест всего")]
        public int FreePlaces { get; set; }

        [Display(Name = "Мест свободно")]
        public int AllPlaces { get; set; }

        [Display(Name = "Роль")]
        public string RoleName { get; set; }

        [Display(Name = "Примечание")]
        public string Note { get; set; }
    }

    [NotMapped]
    public class Events2ResViewModel
    {
        [ScaffoldColumn(false)]
        public int EventId { get; set; }

        [Display(Name = "Название")]
        public string EventName { get; set; }

        [Display(Name = "Начало")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime StartTime { get; set; }
        
        [Display(Name = "Окончание")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}")]
        public DateTime EndTime{ get; set; }
        
        [Display(Name = "Мест всего")]
        public int FreePlaces { get; set; }

        [Display(Name = "Мест свободно")]
        public int AllPlaces { get; set; }

    }

    [NotMapped]
    public class PotentialClientsViewModel
    {
        public int ClientId { get; set; }

        [Display(Name = "ФИО персоны")]
        public string PersonFIO { get; set; }

        [Display(Name = "Мероприятие")]
        public string EventName { get; set; }
      
        [Display(Name = "Роль")]
        public string RoleName { get; set; }

        [Display(Name = "Информация")]
        public string Infoes { get; set; }
        public int PersonId { get; set; }
    }

    [NotMapped]
    public class CommentViewModel
    {
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Необходимо выбрать источник отзыва")]
        [Display(Name = "Источник")]
        public int SourceId { get; set; }

        [Display(Name = "Дата отзыва")]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Дата отзыва\"")]
        public DateTime Date { get; set; }

        [Display(Name = "Отзыв")]
        [Required(ErrorMessage = "Необходимо заполнить поле \"Отзыв\"")]
        [StringLength(4000, ErrorMessage = "Примечание должно быть не более 4000 символов ")]
        public string Text { get; set; }

        [Display(Name = "Оценка")]
        public string Rating { get; set; }

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

        [Display(Name = "Мероприятие")]
        public string EventName { get; set; }

        [Display(Name = "ФИО персоны")]
        public string PersonFIO { get; set; }

        [Display(Name = "Источник")]
        public string SourceName { get; set; }


        
    }
}