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
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class PersonRolesController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: PersonRoles
        public ActionResult RolesShow()
        {
            return View(db.PersonRoles.ToList());
        }

        // GET: PersonRoles/Create
        [Authorize(Roles = "Administrator, User")]
        public ActionResult RoleCreate()
        {
            return View();
        }

        // POST: PersonRoles/Create
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
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
        [Authorize(Roles = "Administrator, User")]
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
        [Authorize(Roles = "Administrator, User")]
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
        [Authorize(Roles = "Administrator, User")]
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
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                PersonRole _role = db.PersonRoles.FirstOrDefault(x => x.RoleId == id);
                if (_role != null)
                {
                    //Запрещаем удаление роли "Клиент" 
                    string _namestring = (string)"Клиент".ToUpper();
                    string _rolename = _role.RoleName.ToUpper();
                    if (_rolename == _namestring)
                    {
                        TempData["MessageError"] = "Роль \"Клиент\" является системной и не может быть изменена или удалена!";
                        return RedirectToAction("RolesShow");
                    }

                    if (_role.Visit.Any())
                    {
                        string _message = "Роль не может быть удалена, так как имеются связанные данные в таблице посещений";
                        TempData["MessageError"] = _message;
                        return RedirectToAction("RolesShow");
                    }

                    if (_role.Reserve.Any())
                    {
                        string _message = "Роль не может быть удалена, так как имеются связанные данные в таблице бронирований";
                        TempData["MessageError"] = _message;
                        return RedirectToAction("RolesShow");
                    }

                    if (_role.PotentialСlient.Any())
                    {
                        string _message = "Роль не может быть удалена, так как имеются связанные данные в таблице потенциальных клиентов";
                        TempData["MessageError"] = _message;
                        return RedirectToAction("RolesShow");
                    }

                    db.PersonRoles.Remove(_role);
                    TempData["MessageOk"] = "Роль успешно удалена";
                    db.SaveChanges();
                    return RedirectToAction("RolesShow");
                }

                else
                {
                    string _message = string.Format("Удаление невозможно. Указанный объект отсутствует в базе данных.");
                    TempData["MessageError"] = _message;
                    return RedirectToAction("RolesShow");
                }


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
