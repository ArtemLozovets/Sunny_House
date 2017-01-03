using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;

namespace Sunny_House.Controllers
{

    [Authorize(Roles = "Administrator, User")]
    public class CommentsController : Controller
    {
        private SunnyModel db = new SunnyModel();


        #region Область методов комментария

        // GET: Comments
        public ActionResult CommentShow(int? CommentId)
        {
            var comments = (from comment in db.Comments
                            where comment.CommentId == CommentId || CommentId == null
                            select new
                            {
                                CommentId = comment.CommentId,
                                Date = comment.Date,
                                Text = comment.Text,
                                Rating = comment.Rating,
                                CommentSource = comment.CommentSource,
                                Event = comment.Event,
                                Exercise = comment.Exercise,
                                Address = comment.Address,
                                Person1 = comment.Person ?? null,
                                Person = comment.Person1 ?? null
                            }).AsEnumerable().Select(x => new Comment
                             {
                                 CommentId = x.CommentId,
                                 Date = x.Date,
                                 Text = x.Text,
                                 Rating = x.Rating,
                                 CommentSource = x.CommentSource,
                                 Event = x.Event,
                                 Exercise = x.Exercise,
                                 Address = x.Address,
                                 SignPersonFIO = x.Person == null ? "" : (x.Person.FirstName + " " + x.Person.LastName).TrimStart(),
                                 AboutPersonFIO = x.Person1 == null ? "" : (x.Person1.FirstName + " " + x.Person1.LastName).TrimStart(),
                                 SignPersonId = x.Person == null ? 0 : x.Person.PersonId,
                                 AboutPersonId = x.Person1 == null ? 0 : x.Person1.PersonId,
                             }).OrderBy(x=>x.Date);


            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public async Task<ActionResult> CommentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult CommentCreate()
        {
            ViewBag.SourceId = new SelectList(db.CommentSources.OrderBy(i => i.SourceName), "SourceId", "SourceName");

            Comment _comment = new Comment();
            _comment.Date = DateTime.Now;
            _comment.Rating = 1;

            return View(_comment);
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentCreate([Bind(Include = "CommentId,SourceId,Date,Text,Rating,AboutPersonId,EventId,ExerciseId,AddressId,SignPersonId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Comments.Add(comment);
                    await db.SaveChangesAsync();
                    TempData["MessageOk"] = "Отзыв успешно добавлен";
                    return RedirectToAction("CommentShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }

            }
            else
            {

                TempData["MessageError"] = "Ошибка валидации модели";
                ViewBag.SourceId = new SelectList(db.CommentSources.OrderBy(i => i.SourceName), "SourceId", "SourceName", comment.SourceId);

                if (comment.SignPersonId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == comment.SignPersonId);
                    ViewData["SignPersonId"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }
                if (comment.AboutPersonId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == comment.AboutPersonId);
                    ViewData["AboutPersonId"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }
                if (comment.EventId > 0)
                {
                    Event _event = db.Events.FirstOrDefault(p => p.EventId == comment.EventId);
                    ViewData["EventName"] = String.Format("{0}", _event.EventName);
                }
                if (comment.ExerciseId > 0)
                {
                    Exercise _ex = db.Exercises.FirstOrDefault(p => p.ExerciseId == comment.ExerciseId);
                    ViewData["ExName"] = _ex.Subject;
                }
                if (comment.AddressId > 0)
                {
                    Address _ad = db.Addresses.FirstOrDefault(p => p.AddressId == comment.AddressId);
                    ViewData["AddressName"] = _ad.Alias;
                }

                return View(comment);
            }
        }

        // GET: Comments/Edit/5
        public async Task<ActionResult> CommentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.SourceId = new SelectList(db.CommentSources.OrderBy(i => i.SourceName), "SourceId", "SourceName", comment.SourceId);

            Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == comment.SignPersonId);
            if (_pers != null)
            {
                ViewData["SignPersonId"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
            }

            Person _abpers = db.Persons.FirstOrDefault(p => p.PersonId == comment.AboutPersonId);
            if (_abpers != null)
            {
                ViewData["AboutPersonId"] = string.Format("{0} {1} {2}", _abpers.FirstName, _abpers.LastName, _abpers.MiddleName);
            }

            Event _event = db.Events.FirstOrDefault(p => p.EventId == comment.EventId);
            if (_event != null)
            {
                ViewData["EventName"] = String.Format("{0}", _event.EventName);
            }

            Exercise _ex = db.Exercises.FirstOrDefault(p => p.ExerciseId == comment.ExerciseId);
            if (_ex != null)
            {
                ViewData["ExName"] = _ex.Subject;
            }

            Address _ad = db.Addresses.FirstOrDefault(p => p.AddressId == comment.AddressId);
            if (_ad != null)
            {
                ViewData["AddressName"] = _ad.Alias;
            }

            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentEdit([Bind(Include = "CommentId,SourceId,Date,Text,Rating,AboutPersonId,EventId,ExerciseId,AddressId,SignPersonId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(comment).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["MessageOk"] = "Данные отзыва успешно изменены";
                    return RedirectToAction("CommentShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
            else
            {

                TempData["MessageError"] = "Ошибка валидации модели";
                ViewBag.SourceId = new SelectList(db.CommentSources.OrderBy(i => i.SourceName), "SourceId", "SourceName", comment.SourceId);

                if (comment.SignPersonId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == comment.SignPersonId);
                    ViewData["SignPersonId"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }
                if (comment.AboutPersonId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == comment.AboutPersonId);
                    ViewData["AboutPersonId"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }
                if (comment.EventId > 0)
                {
                    Event _event = db.Events.FirstOrDefault(p => p.EventId == comment.EventId);
                    ViewData["EventName"] = String.Format("{0}", _event.EventName);
                }
                if (comment.ExerciseId > 0)
                {
                    Exercise _ex = db.Exercises.FirstOrDefault(p => p.ExerciseId == comment.ExerciseId);
                    ViewData["ExName"] = _ex.Subject;
                }
                if (comment.AddressId > 0)
                {
                    Address _ad = db.Addresses.FirstOrDefault(p => p.AddressId == comment.AddressId);
                    ViewData["AddressName"] = _ad.Alias;
                }

                return View(comment);
            }
        }

        // GET: Comments/Delete/5
        public async Task<ActionResult> CommentDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("CommentDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentDeleteConfirmed(int id)
        {
            try
            {
                Comment comment = await db.Comments.FindAsync(id);
                db.Comments.Remove(comment);
                await db.SaveChangesAsync();
                TempData["MessageOk"] = "Отзыв успешно удален";
                return RedirectToAction("CommentShow");
            }

            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }
        }

        #endregion

        #region Область методов работы с источником комментария

        public async Task<ActionResult> CSShow(int? CommentId)
        {
            return View(await db.CommentSources.ToListAsync());
        }

        // GET: CommentSources/Details/5
        public async Task<ActionResult> CSDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentSource commentSource = await db.CommentSources.FindAsync(id);
            if (commentSource == null)
            {
                return HttpNotFound();
            }
            return View(commentSource);
        }

        // GET: CommentSources/Create
        public ActionResult CSCreate()
        {
            return View();
        }

        // POST: CommentSources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CSCreate([Bind(Include = "SourceId,SourceName")] CommentSource commentSource)
        {
            if (ModelState.IsValid)
            {
                db.CommentSources.Add(commentSource);
                await db.SaveChangesAsync();
                TempData["MessageOk"] = "Информация об источнике отзывов успешно добавлена";
                return RedirectToAction("CSShow");
            }

            return View(commentSource);
        }

        // GET: CommentSources/Edit/5
        public async Task<ActionResult> CSEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentSource commentSource = await db.CommentSources.FindAsync(id);
            if (commentSource == null)
            {
                return HttpNotFound();
            }
            return View(commentSource);
        }

        // POST: CommentSources/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CSEdit([Bind(Include = "SourceId, SourceName")] CommentSource commentSource)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

                    //Запрещаем редактирование источника "Бронирование" 
                    string _namestring = (string)"Бронирование".ToUpper();
                    string _sourcestring = (from source in db.CommentSources where source.SourceId == commentSource.SourceId select source.SourceName).Single().ToString().ToUpper();
                    if (_sourcestring == _namestring)
                    {
                        TempData["MessageError"] = "Источник \"Бронирование\" является системным и не может быть изменен или удален!";
                        return RedirectToAction("CSShow");
                    }

                    db.Entry(commentSource).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["MessageOk"] = "Информация об источнике отзывов успешно изменена";
                    return RedirectToAction("CSShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
            return View(commentSource);
        }

        // GET: CommentSources/Delete/5
        public async Task<ActionResult> CSDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentSource commentSource = await db.CommentSources.FindAsync(id);
            if (commentSource == null)
            {
                return HttpNotFound();
            }
            return View(commentSource);
        }

        // POST: CommentSources/Delete/5
        [HttpPost, ActionName("CSDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CSDeleteConfirmed(int id)
        {
            try
            {
                //Запрещаем удаление источника "Бронирование" 
                string _namestring = (string)"Бронирование".ToUpper();
                string _sourcename = db.CommentSources.Find(id).SourceName.ToUpper();
                if (_sourcename == _namestring)
                {
                    TempData["MessageError"] = "Источник \"Бронирование\" является системным и не может быть изменен или удален!";
                    return RedirectToAction("CSShow");
                }

                if (db.CommentSources.First(i => i.SourceId == id).Comment.Any())
                {
                    TempData["MessageError"] = "Данный источник не может быть удален, так как имеются связанные записи в таблице отзывов!";
                    return RedirectToAction("CSShow");
                }

                CommentSource commentSource = await db.CommentSources.FindAsync(id);                
                db.CommentSources.Remove(commentSource);
                await db.SaveChangesAsync();
                TempData["MessageOk"] = "Информация об источнике отзывов успешно удалена";
                return RedirectToAction("CSShow");
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }

        }

        #endregion

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