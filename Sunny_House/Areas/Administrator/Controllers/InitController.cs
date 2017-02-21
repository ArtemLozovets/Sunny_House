using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Sunny_House.Models;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace Sunny_House.Areas.Administrator.Controllers
{
    public class InitController : Controller
    {
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Init

        public ActionResult CreateAdmin()
        {
            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

            db.Database.ExecuteSqlCommand("DROP INDEX UserNameIndex ON AspNetUsers");
            db.Database.ExecuteSqlCommand("ALTER TABLE AspNetUsers ALTER COLUMN UserName NVARCHAR(64) COLLATE Latin1_General_CS_AS NOT NULL");
            db.Database.ExecuteSqlCommand("CREATE UNIQUE NONCLUSTERED INDEX UserNameIndex ON AspNetUsers (UserName ASC) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
            db.SaveChanges();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (roleManager.Roles.Count() > 0)
            {
                TempData["MessageError"] = "Выполнение операции прекращено. В таблице ролей имеются записи.";
                return RedirectToAction("ShowUsers", new { area = "Administrator", controller = "UsersManagement" });
            }

            //Создаем роли администратора и пользователя
            var role1 = new IdentityRole { Name = "Administrator" };
            var role2 = new IdentityRole { Name = "User" };
            var role3 = new IdentityRole { Name = "Blockeduser" };

            // добавляем роль в бд
            roleManager.Create(role1);
            roleManager.Create(role2);
            roleManager.Create(role3);
            db.SaveChanges();

            //Создаем пользователя и администратора
            var admin = new ApplicationUser
            {
                UserName = "techadmin",
                Email = "admin@tech.com",
                FirstName = "Администратор",
                LastName = "Технический",
                MiddleName = "Technical Administrator",
                Post = "Администратор",
                UserRole = "Administrator"
            };

            var user = new ApplicationUser
            {
                UserName = "techuser",
                Email = "user@tech.com",
                FirstName = "Пользователь",
                LastName = "Технический",
                MiddleName = "Technical User",
                Post = "Пользователь",
                UserRole = "User"
            };

            //Добавляем пользователя и администратора в базу данных
            UserManager.Create(admin, "Qwe!123");
            UserManager.AddToRole(admin.Id, admin.UserRole);
            
            
            UserManager.Create(user, "Qwe!123");
            UserManager.AddToRole(user.Id, user.UserRole);

            //Добавляем пользователя в роль

            TempData["MessageOk"] = "Учетная запись создана";
            return RedirectToAction("ShowUsers", new { area = "Administrator", controller = "UsersManagement" });

        }

        public string InitRole()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var role = new ApplicationRole { Name = "Presenter", Description = "Ведущий" };
            // добавляем роль в бд
            roleManager.Create(role);

            db.SaveChanges();
            return "Complete";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();

                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

            }
            base.Dispose(disposing);
        }

    }
}