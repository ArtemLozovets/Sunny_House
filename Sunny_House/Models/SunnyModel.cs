namespace Sunny_House.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.Migrations;
    using Microsoft.AspNet.Identity.EntityFramework;


    public sealed class ConfigurationA : DbMigrationsConfiguration<SunnyModel>
    {
        public ConfigurationA()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(SunnyModel db)
        {
            #region Ініціалізація

            //#region Персони
            //Person _pers1 = new Person
            //{
            //    FirstName = "Хоменко",
            //    LastName = "Жанна",
            //    MiddleName = "Василівна",
            //    DateOfBirth = Convert.ToDateTime("20/07/2016"),
            //    Note = ""
            //};

            //Person _pers2 = new Person
            //{
            //    FirstName = "Жмуров",
            //    LastName = "Василь",
            //    MiddleName = "Свиридович",
            //    DateOfBirth = Convert.ToDateTime("16/12/1924"),
            //    Note = ""
            //};

            //Person _pers3 = new Person
            //{
            //    FirstName = "Хоменко",
            //    LastName = "Петрик",
            //    MiddleName = "Сергійович",
            //    DateOfBirth = Convert.ToDateTime("18/03/2008"),
            //    Note = ""
            //};

            //Person _pers4 = new Person
            //{
            //    FirstName = "Хоменко",
            //    LastName = "Сергій",
            //    MiddleName = "Степанович",
            //    DateOfBirth = Convert.ToDateTime("04/08/1970"),
            //    Note = ""
            //};


            //Person _pers5 = new Person
            //{
            //    FirstName = "Смірнова",
            //    LastName = "Анжела",
            //    MiddleName = "Іванівна",
            //    DateOfBirth = Convert.ToDateTime("04/08/1989"),
            //    Note = ""
            //};

            //Person _pers6 = new Person
            //{
            //    FirstName = "Строгов",
            //    LastName = "Дмитро",
            //    MiddleName = "Вільгельмович",
            //    DateOfBirth = Convert.ToDateTime("08/12/1964"),
            //    Note = ""
            //};
            //#endregion

            //#region Родинні зв'язки

            //#endregion

            //#region Платежі
            //Payment _pay1 = new Payment { PayerId = 1, ClientId = 3, Date = Convert.ToDateTime("25.03.2016 08:20:12"), Sum = 456.56M, Note = "Хоменко Петрик, за участь у навчанні ХХХХХХХХ " };
            //Payment _pay2 = new Payment { PayerId = 1, ClientId = 3, Date = Convert.ToDateTime("14.05.2016 14:20:10"), Sum = 254.00M, Note = "За Петрика Хоменка. Участь у навчанні 000000000 " };
            //Payment _pay3 = new Payment { PayerId = 2, ClientId = 3, Date = Convert.ToDateTime("19.06.2016 15:43:12"), Sum = 560.48M, Note = "За проходження курсу ХХХХХХ. Хоменко Петрик" };
            //Payment _pay4 = new Payment { PayerId = 4, ClientId = 3, Date = Convert.ToDateTime("14.08.2016 11:12:14"), Sum = 380.21M, Note = "Оплата послуг із навчання Хоменко Петрика " };
            //Payment _pay5 = new Payment { PayerId = 5, ClientId = 3, Date = Convert.ToDateTime("28.08.2016 11:12:14"), Sum = 600.41M, Note = "Оплата послуг. За Хоменка Петрика від тьоті :)" };
            //#endregion

            //#region Місця персон
            //PersonPlace _place1 = new PersonPlace { PersonId = 1, AddressId = 1, PlaceName = "Місце проживання" };
            //PersonPlace _place2 = new PersonPlace { PersonId = 1, AddressId = 2, PlaceName = "Місце роботи" };
            //PersonPlace _place3 = new PersonPlace { PersonId = 3, AddressId = 1, PlaceName = "Місце проживання" };
            //PersonPlace _place4 = new PersonPlace { PersonId = 4, AddressId = 1, PlaceName = "Місце проживання" };
            //PersonPlace _place5 = new PersonPlace { PersonId = 2, AddressId = 3, PlaceName = "Місце проживання" };
            //PersonPlace _place6 = new PersonPlace { PersonId = 5, AddressId = 4, PlaceName = "Місце проживання" };
            //PersonPlace _place7 = new PersonPlace { PersonId = 2, AddressId = 5, PlaceName = "Місце реєстрації" };
            //#endregion

            //#region Адреси
            //Address _addr1 = new Address { Alias = "Квартира", PostCode = "36010", Country = "Україна", Region = "Полтавська", Area = "Полтавський", City = "Полтава", AddressValue = "Овочева, 8, кв. 16", GeoTag = "49°35'34.0\"N 34°30'59.8\"E", Note = "" };
            //Address _addr2 = new Address { Alias = "База \"Іріта\"", PostCode = "36010", Country = "Україна", Region = "Полтавська", Area = "Полтавський", City = "Полтава", AddressValue = "Квітки Цісик, 26", GeoTag = "49°35'39.3\"N 34°30'52.9\"E", Note = "" };
            //Address _addr3 = new Address { Alias = "Будинок", PostCode = "36010", Country = "Україна", Region = "Полтавська", Area = "Полтавський", City = "Полтава", AddressValue = "Половка, 46", GeoTag = "49°35'32.2\"N 34°30'46.4\"E", Note = "" };
            //Address _addr4 = new Address { Alias = "Квартира", PostCode = "37010", Country = "Україна", Region = "Полтавська", Area = "Кременчуцький", City = "Кременчук", AddressValue = "Квіткова 17, кв. 46", GeoTag = "", Note = "" };
            //Address _addr5 = new Address { Alias = "Будинок", PostCode = "38000", Country = "Україна", Region = "Полтавська", Area = "Лубенський", City = "Лубни", AddressValue = "Бджолина 85", GeoTag = "", Note = "" };
            //#endregion

            //#region Заходи

            //Event _ev1 = new Event { EventName = "Event #1", StartTime = Convert.ToDateTime("01/09/2016"), EndTime = Convert.ToDateTime("20/05/2017"), Description = "Вивчення курсу Event #1", AdministratorId = 6, Note = "" };
            //Event _ev2 = new Event { EventName = "Event #2", StartTime = Convert.ToDateTime("01/09/2016"), EndTime = Convert.ToDateTime("31/12/2016"), Description = "Вивчення курсу Event #2", AdministratorId = 6, Note = "" };
            //Event _ev3 = new Event { EventName = "Event #3", StartTime = Convert.ToDateTime("20/10/2016"), EndTime = Convert.ToDateTime("28/04/2017"), Description = "Вивчення курсу Event #3", AdministratorId = 6, Note = "" };

            //#endregion

            //#region =======================Заповнення таблиць=======================
            //db.Persons.Add(_pers1);
            //db.Persons.Add(_pers2);
            //db.Persons.Add(_pers3);
            //db.Persons.Add(_pers4);
            //db.Persons.Add(_pers5);
            //db.Persons.Add(_pers6);

            //db.SaveChanges();

            //db.Payments.Add(_pay1);
            //db.Payments.Add(_pay2);
            //db.Payments.Add(_pay3);
            //db.Payments.Add(_pay4);
            //db.Payments.Add(_pay5);
            //db.SaveChanges();

            //db.Addresses.Add(_addr1);
            //db.Addresses.Add(_addr2);
            //db.Addresses.Add(_addr3);
            //db.Addresses.Add(_addr4);
            //db.Addresses.Add(_addr5);

            //db.SaveChanges();

            //db.PersonPlaces.Add(_place1);
            //db.PersonPlaces.Add(_place2);
            //db.PersonPlaces.Add(_place3);
            //db.PersonPlaces.Add(_place4);
            //db.PersonPlaces.Add(_place5);
            //db.PersonPlaces.Add(_place6);

            //db.SaveChanges();

            //db.Events.Add(_ev1);
            //db.Events.Add(_ev2);
            //db.Events.Add(_ev3);

            //db.SaveChanges();

            //#endregion
            #endregion
        }
    }


    //class AppUserContextInitializer : DropCreateDatabaseAlways<SunnyModel>
    //class AppUserContextInitializer : DropCreateDatabaseIfModelChanges<SunnyModel>
    class AppUserContextInitializer : MigrateDatabaseToLatestVersion<SunnyModel, ConfigurationA>
    {

    }

    public partial class SunnyModel : DbContext
    {
        static SunnyModel()
        {
            Database.SetInitializer<SunnyModel>(new AppUserContextInitializer());
        }


        public SunnyModel()
            : base("name=SunnyModel")
        {

        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CommentSource> CommentSources { get; set; }
        public virtual DbSet<Communication> Communications { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Exercise> Exercises { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<PersonCommunication> PersonCommunications { get; set; }
        public virtual DbSet<PersonPlace> PersonPlaces { get; set; }
        public virtual DbSet<PersonRelation> PersonRelations { get; set; }
        public virtual DbSet<Reserve> Reserves { get; set; }
        public virtual DbSet<PersonRole> PersonRoles { get; set; }
        public virtual DbSet<TypeOfCommunication> TypeOfCommunications { get; set; }
        public virtual DbSet<Visit> Visits { get; set; }
        public DbSet<RelationTypesCatalog> RelationTypesCatalogs { get; set; }
        public DbSet<AspVisitor> AspVisitors { get; set; }
        public DbSet<PotentialСlient> PotentialСlients { get; set; }
        public DbSet<STask> STask { get; set; }

        #region Fluent API
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.Exercise)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasMany(e => e.PersonPlace)
                .WithRequired(e => e.Address)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CommentSource>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.CommentSource)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Communication>()
                .HasMany(e => e.PersonCommunication)
                .WithRequired(e => e.Communication)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Exercise)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                 .HasMany(e => e.Reserve)
                 .WithRequired(e => e.Event)
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                 .HasMany(e => e.PotentialClient)
                 .WithRequired(e => e.Event)
                 .HasForeignKey(e => e.EventId)
                 .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Payment)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.Exercise)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Exercise>()
                .HasMany(e => e.Visit)
                .WithRequired(e => e.Exercise)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Comment)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.AboutPersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Comment1)
                .WithRequired(e => e.Person1)
                .HasForeignKey(e => e.SignPersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.AdministratorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Payment)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.PayerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Payment1)
                .WithRequired(e => e.Person1)
                .HasForeignKey(e => e.ClientId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonCommunication)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonPlace)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonRelation)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PersonRelation1)
                .WithRequired(e => e.Person1)
                .HasForeignKey(e => e.RelPersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Reserve)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.PotentialClient)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Visit)
                .WithRequired(e => e.Person)
                .HasForeignKey(e => e.VisitorId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonRole>()
                .HasMany(e => e.Reserve)
                .WithRequired(e => e.PersonRole)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonRole>()
                .HasMany(e => e.PotentialСlient)
                .WithRequired(e => e.PersonRole)
                .HasForeignKey(e => e.RoleId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonRole>()
                .HasMany(e => e.Visit)
                .WithRequired(e => e.PersonRole)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TypeOfCommunication>()
                .HasMany(e => e.Communication)
                .WithRequired(e => e.TypeOfCommunication)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Payment>().Property(p => p.Sum)
                .HasPrecision(7, 2);

        }
        #endregion

    }
}
