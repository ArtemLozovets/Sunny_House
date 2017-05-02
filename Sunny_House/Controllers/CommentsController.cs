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
using PagedList;
using PagedList.Mvc;
using System.Web.Configuration;
using System.Configuration;


namespace Sunny_House.Controllers
{

    [Authorize(Roles = "Administrator, User, Presenter")]
    public class CommentsController : Controller
    {
        private SunnyModel db = new SunnyModel();
        private ApplicationDbContext aspdb = new ApplicationDbContext();


        #region Область методов комментария

        // GET: Comments
        public ActionResult CommentShow()
        {
            ViewBag.SourceId = new SelectList(db.CommentSources.OrderBy(i => i.SourceName), "SourceId", "SourceName");

            return View();
        }

        // GET: Comments/Details/5
        public ActionResult CommentDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Guid _RelGuid = db.Comments.FirstOrDefault(x => x.CommentId == id).RelGuid;

            var _comment = (from comment in db.Comments
                            where comment.CommentId == id
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
                                Person = comment.Person1 ?? null,
                                RelGuid = comment.RelGuid
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
                                AttList = db.Attachments.Where(z => z.RelGuid == x.RelGuid).ToList()
                            }).FirstOrDefault();
            if (_comment == null)
            {
                return HttpNotFound();
            }
            return View(_comment);
        }

        // GET: Comments/Create
        public ActionResult CommentCreate()
        {
            ViewBag.SourceId = new SelectList(db.CommentSources.OrderBy(i => i.SourceName), "SourceId", "SourceName");

            ViewBag.MaxAttSize = GetMaxAttSize();

            Comment _comment = new Comment();
            _comment.Date = DateTime.Now;
            _comment.Rating = 1;
            _comment.RelGuid = Guid.NewGuid();

            return View(_comment);
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentCreate([Bind(Include = "CommentId,SourceId,Date,Text,Rating,AboutPersonId,EventId,ExerciseId,AddressId,SignPersonId,RelGuid,CreatorId")] Comment comment, string CreatorName)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Получаем идентификатор текущего пользователя
                    if (!String.IsNullOrEmpty(CreatorName))
                    {
                        comment.CreatorId = Guid.Parse(aspdb.Users.FirstOrDefault(x => x.UserName == CreatorName).Id.ToString());
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

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

                ViewBag.MaxAttSize = GetMaxAttSize();

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

            ViewBag.MaxAttSize = GetMaxAttSize();

            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentEdit([Bind(Include = "CommentId,SourceId,Date,Text,Rating,AboutPersonId,EventId,ExerciseId,AddressId,SignPersonId,RelGuid,CreatorId")] Comment comment, string CurrentUser)
        {
            if (ModelState.IsValid)
            {
                Guid? _currentUserId = null;
                if (!String.IsNullOrEmpty(CurrentUser))
                {
                    _currentUserId = Guid.Parse(aspdb.Users.FirstOrDefault(x => x.UserName == CurrentUser).Id.ToString());
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (comment.CreatorId == _currentUserId || User.IsInRole("Administrator") || User.IsInRole("User"))
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
                    TempData["MessageError"] = "Вы не являетесь создателем данного отзыва. Редактирование запрещено.";
                    return RedirectToAction("CommentShow");
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

                ViewBag.MaxAttSize = GetMaxAttSize();

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
        public async Task<ActionResult> CommentDeleteConfirmed(int id, string CurrentUser)
        {
            if (db.Comments.FirstOrDefault(e => e.CommentId == id) != null)
            {
                Guid? _currentUserId = null;
                if (!String.IsNullOrEmpty(CurrentUser))
                {
                    _currentUserId = Guid.Parse(aspdb.Users.FirstOrDefault(x => x.UserName == CurrentUser).Id.ToString());
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Comment comment = await db.Comments.FindAsync(id);
                if (comment.CreatorId == _currentUserId || User.IsInRole("Administrator") || User.IsInRole("User"))
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            string AttachmentPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"];

                            var _attList = db.Attachments.Where(x => x.RelGuid == comment.RelGuid).ToList();

                            db.Attachments.RemoveRange(db.Attachments.Where(x => x.RelGuid == comment.RelGuid));
                            db.SaveChanges();

                            db.Comments.Remove(comment);
                            db.SaveChanges();

                            foreach (var item in _attList)
                            {
                                string fullPath = AttachmentPath + item.ServerFileName.ToString();
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }

                            dbContextTransaction.Commit();
                            TempData["MessageOk"] = "Отзыв успешно удален";
                        }

                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            ViewBag.ErMes = ex.Message;
                            ViewBag.ErStack = ex.StackTrace;
                            return View("Error");
                        }
                    }
                }
                else
                {
                    TempData["MessageError"] = "Вы не являетесь создателем данного отзыва. Удаление запрещено.";
                    return RedirectToAction("CommentShow");
                }
            }
            else
            {
                string _message = string.Format("Отзыв не может быть удален. Указанный объект отсутствует в базе данных.");
                TempData["MessageError"] = _message;
            }

            return RedirectToAction("CommentShow");
        }
        private int GetMaxAttSize() // Функция чтения значения MaxRequestLength из файла web.config
        {
            int MaxAttSize = 10240000;
            HttpRuntimeSection section = ConfigurationManager.GetSection("system.web/httpRuntime") as HttpRuntimeSection;
            if (section != null)
            {
                MaxAttSize = (section.MaxRequestLength * 1024);
            }
            return MaxAttSize;
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
                if (db.Comments.FirstOrDefault(e => e.CommentId == id) != null)
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
                   
                }
                else
                {
                    string _message = string.Format("Удаление невозможно. Указанный объект отсутствует в базе данных.");
                    TempData["MessageError"] = _message;
                }
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

        #region Метод отображения списка отзывов в частичном представлении

        public ActionResult CommentShowPartial(int? CommentId, int? page,
                                string SortBy, string minRating, string maxRating,
                                string SearchStartDate, string SearchEndDate,
                                int? SourceId, string SearchText,
                                int? SignPersonId, int? AboutPersonId, int? EventId, int? ExerciseId, int? AddressId,
                                bool? chbAboutPerson, bool? chbEvent, bool? chbEx, bool? chbAddress)
        {
            ViewBag.SortSource = SortBy == "Source" ? "Source desc" : "Source";
            ViewBag.SortDate = SortBy == "Date" ? "Date desc" : "Date";
            ViewBag.SortRating = SortBy == "Rating" ? "Rating desc" : "Rating";
            ViewBag.SortSignPerson = SortBy == "SignPerson" ? "SignPerson desc" : "SignPerson";

            // Блок кода для преобразования минимального и максимального значения рейтинга для фильтра 
            int _minRating, _maxRating;
            if (string.IsNullOrEmpty(minRating)) { minRating = "1"; }
            if (string.IsNullOrEmpty(maxRating)) { maxRating = "32767"; }
            Int32.TryParse(minRating, out _minRating);
            Int32.TryParse(maxRating, out _maxRating);

            // Блок кода для преобразования значений дат периода фильтрации
            DateTime? _startDate = (String.IsNullOrEmpty(SearchStartDate)) ? Convert.ToDateTime("1900-01-01") : Convert.ToDateTime(SearchStartDate);
            DateTime? _endDate = (String.IsNullOrEmpty(SearchEndDate)) ? Convert.ToDateTime("2900-01-01") : Convert.ToDateTime(SearchEndDate);



            var comments = (from comment in db.Comments
                            where (comment.CommentId == CommentId || CommentId == null) &&
                            (comment.Rating >= _minRating && comment.Rating <= _maxRating) &&
                            (comment.Date >= _startDate && comment.Date <= _endDate) &&
                            (comment.SourceId == SourceId || SourceId == null) &&
                            (comment.Text.Contains(SearchText) || string.IsNullOrEmpty(SearchText)) &&
                            (comment.SignPersonId == SignPersonId || SignPersonId == null) &&
                            (comment.AboutPersonId == AboutPersonId || AboutPersonId == null) &&
                            (comment.EventId == EventId || EventId == null) &&
                            (comment.ExerciseId == ExerciseId || ExerciseId == null) &&
                            (comment.AddressId == AddressId || AddressId == null) &&
                            (comment.AboutPersonId.HasValue || chbAboutPerson == false || chbAboutPerson == null) &&
                            (comment.EventId.HasValue || chbEvent == false || chbEvent == null) &&
                            (comment.ExerciseId.HasValue || chbEx == false || chbEx == null) &&
                            (comment.AddressId.HasValue || chbAddress == false || chbAddress == null)
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
                                Person = comment.Person1 ?? null,
                                RelGuid = comment.RelGuid
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
                                AttCount = db.Attachments.Where(z => z.RelGuid == x.RelGuid).Count()
                            }).OrderByDescending(x => x.Date);

            switch (SortBy)
            {
                case "Source desc":
                    comments = comments.OrderByDescending(x => x.CommentSource.SourceName);
                    ViewData["SortColumn"] = "Source";
                    break;
                case "Source":
                    comments = comments.OrderBy(x => x.CommentSource.SourceName);
                    ViewData["SortColumn"] = "Source";
                    break;

                case "Date desc":
                    comments = comments.OrderByDescending(x => x.Date);
                    ViewData["SortColumn"] = "Date";
                    break;
                case "Date":
                    comments = comments.OrderBy(x => x.Date);
                    ViewData["SortColumn"] = "Date";
                    break;

                case "Rating desc":
                    comments = comments.OrderByDescending(x => x.Rating);
                    ViewData["SortColumn"] = "Rating";
                    break;
                case "Rating":
                    comments = comments.OrderBy(x => x.Rating);
                    ViewData["SortColumn"] = "Rating";
                    break;

                case "SignPerson desc":
                    comments = comments.OrderByDescending(x => x.SignPersonFIO);
                    ViewData["SortColumn"] = "SignPerson";
                    break;
                case "SignPerson":
                    comments = comments.OrderBy(x => x.SignPersonFIO);
                    ViewData["SortColumn"] = "SignPerson";
                    break;

                default:
                    comments = comments.OrderByDescending(x => x.Date);
                    break;
            }

            ViewData["chbAboutPerson"] = chbAboutPerson;
            ViewData["chbEvent"] = chbEvent;
            ViewData["chbEx"] = chbEx;
            ViewData["chbAddress"] = chbAddress;

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(comments.ToList().ToPagedList(pageNumber, pageSize));
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