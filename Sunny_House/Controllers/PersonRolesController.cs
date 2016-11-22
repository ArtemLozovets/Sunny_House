using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;
using System.Data.Entity.Core.Objects;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class PersonRolesController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: PersonRoles
        public ActionResult RolesShow()
        {
            return View(db.PersonRoles.ToList());
        }

        // GET: PersonRoles/Create
        public ActionResult RoleCreate()
        {
            return View();
        }

        // POST: PersonRoles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleCreate([Bind(Include = "RoleId,RoleName")] PersonRole personRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.PersonRoles.Where(r => r.RoleName == personRole.RoleName).Count() > 0)
                    {
                        TempData["MessageError"] = "Указанная роль уже существует";
                        return RedirectToAction("RolesShow");
                    }

                    db.PersonRoles.Add(personRole);
                    db.SaveChanges();
                    TempData["MessageOk"] = "Роль успешно добавлена";
                    return RedirectToAction("RolesShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
            return View(personRole);
        }

        // GET: PersonRoles/Edit/5
        public ActionResult RoleEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonRole personRole = db.PersonRoles.Find(id);
            if (personRole == null)
            {
                return HttpNotFound();
            }
            return View(personRole);
        }

        // POST: PersonRoles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleEdit([Bind(Include = "RoleId,RoleName")] PersonRole personRole)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Запрещаем редактирование роли "Клиент" 
                    string _namestring = (string)"Клиент".ToUpper();
                    string _rolestring = (from role in db.PersonRoles where role.RoleId == personRole.RoleId select role.RoleName).Single().ToString().ToUpper();
                    if (_rolestring == _namestring)
                    {
                        TempData["MessageError"] = "Роль \"Клиент\" является системной и не может быть изменена или удалена!";
                        return RedirectToAction("RolesShow");
                    }

                    db.Entry(personRole).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["MessageOk"] = "Роль успешно изменена";
                    return RedirectToAction("RolesShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
            return View(personRole);
        }

        // GET: PersonRoles/Delete/5
        public ActionResult RoleDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonRole personRole = db.PersonRoles.Find(id);
            if (personRole == null)
            {
                return HttpNotFound();
            }
            return View(personRole);
        }

        // POST: PersonRoles/Delete/5
        [HttpPost, ActionName("RoleDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //Запрещаем удаление роли "Клиент" 
                string _namestring = (string)"Клиент".ToUpper();
                string _rolename = db.PersonRoles.Find(id).RoleName.ToUpper();
                if (_rolename == _namestring)
                {
                    TempData["MessageError"] = "Роль \"Клиент\" является системной и не может быть изменена или удалена!";
                    return RedirectToAction("RolesShow");
                }

                if (db.PersonRoles.First(i => i.RoleId == id).Visit.Any())
                {
                    string _message = "Роль не может быть удалена, так как имеются связанные данные в таблице посещений";
                    TempData["MessageError"] = _message;
                    return RedirectToAction("RolesShow");
                }

                if (db.PersonRoles.First(i => i.RoleId == id).Reserve.Any())
                {
                    string _message = "Роль не может быть удалена, так как имеются связанные данные в таблице бронирований";
                    TempData["MessageError"] = _message;
                    return RedirectToAction("RolesShow");
                }

                if (db.PersonRoles.First(i => i.RoleId == id).PotentialСlient.Any())
                {
                    string _message = "Роль не может быть удалена, так как имеются связанные данные в таблице потенциальных клиентов";
                    TempData["MessageError"] = _message;
                    return RedirectToAction("RolesShow");
                }

                PersonRole personRole = db.PersonRoles.Find(id);
                db.PersonRoles.Remove(personRole);
                TempData["MessageOk"] = "Роль успешно удалена";
                db.SaveChanges();
                return RedirectToAction("RolesShow");
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
