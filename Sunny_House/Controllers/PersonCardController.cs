﻿using Sunny_House.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Methods;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class PersonCardController : Controller
    {
        private SunnyModel db = new SunnyModel();

        public enum SexTypes
        {
            Мужской,
            Женский
        }
        // GET: PersonCard
        public ActionResult ShowPersonCard(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Person _pers = db.Persons.FirstOrDefault(x => x.PersonId == PersonId);

            List<RelationViewModel> _rellist = new List<RelationViewModel>();

            var _result = (from person in db.Persons
                           where person.PersonId == PersonId
                           select new RelationViewModel
                           {
                               PersonId = person.PersonId,
                               PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                               RelationName = "Персона",
                               HaveComm = db.PersonCommunications.Any(x=>x.PersonId == PersonId)
                           }).ToList();

            _rellist.AddRange(_result);

            _result = (from relation in db.PersonRelations
                       join person in db.Persons on relation.PersonId equals person.PersonId
                       where relation.RelPersonId == PersonId
                       select new RelationViewModel
                       {
                           PersonId = person.PersonId,
                           PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                           RelationName = relation.Relation12,
                           HaveComm = db.PersonCommunications.Any(x => x.PersonId == relation.PersonId)
                       }).ToList();

            _rellist.AddRange(_result);

            var _resultRev = (from relation in db.PersonRelations
                              join person in db.Persons on relation.RelPersonId equals person.PersonId
                              where relation.PersonId == PersonId
                              select new RelationViewModel
                              {
                                  PersonId = person.PersonId,
                                  PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                  RelationName = relation.Relation21,
                                  HaveComm = db.PersonCommunications.Any(x => x.PersonId == relation.RelPersonId)
                              }).ToList();

            _rellist.AddRange(_resultRev);

            List<Visit> _visitList = (from visit in db.Visits
                                      join ex in db.Exercises on visit.ExerciseId equals ex.ExerciseId
                                      join person in db.Persons on visit.VisitorId equals person.PersonId
                                      join role in db.PersonRoles on visit.RoleId equals role.RoleId
                                      where (visit.VisitorId == PersonId)
                                      select new
                                      {
                                          VisitId = visit.VisitId,
                                          VisitorId = visit.VisitorId,
                                          ExerciseId = ex.ExerciseId,
                                          RoleId = role.RoleId,
                                          ExName = ex.Subject,
                                          Note = visit.Note,
                                          StartTime = ex.StartTime,
                                          EventId = ex.EventId,
                                          EventName = ex.Event.EventName,
                                          FactVisit = visit.FactVisit
                                      }).Distinct().Take(30).AsEnumerable().Select(x => new Visit
                          {
                              VisitId = x.VisitId,
                              VisitorId = x.VisitorId,
                              ExerciseId = x.ExerciseId,
                              RoleId = x.RoleId,
                              ExName = x.ExName,
                              Note = x.Note,
                              StartTime = x.StartTime,
                              EventId = x.EventId,
                              EventName = x.EventName,
                              FactVisit = x.FactVisit
                          }).OrderByDescending(x => x.StartTime).ToList();


            List<Comment> _commentlist = (from relation in db.PersonRelations.Where(x => x.RelPersonId == PersonId || x.PersonId == PersonId)
                                          from comment in db.Comments.Where(x => x.SignPersonId == relation.RelPersonId || x.SignPersonId == relation.PersonId)
                                          select new
                                          {
                                              Date = comment.Date,
                                              SignPersonFIO = comment.Person1.FirstName+" "+comment.Person1.LastName,
                                              Text = comment.Text
                                          }).Distinct().OrderByDescending(x => x.Date).Take(10).AsEnumerable().Select(x => new Comment
                                          {
                                              Date = x.Date,
                                              SignPersonFIO = x.SignPersonFIO,
                                              Text = x.Text
                                          }).ToList();

            MoreInfoesViewModel _moremodel = new MoreInfoesViewModel();

            _moremodel.PersonId = _pers.PersonId;
            _moremodel.PersonFIO = _pers.FirstName + " " + _pers.LastName + " " + _pers.MiddleName;
            _moremodel.PersonNote = _pers.Note;
            _moremodel.Sex = _pers.Sex;
            _moremodel.PersonAge = AgeMethods.GetAge(_pers.DateOfBirth);
            _moremodel.PersonMonth = AgeMethods.GetTotalMonth(_pers.DateOfBirth);
            _moremodel.DateOfBirth = _pers.DateOfBirth;

            _moremodel.RelPerson = _rellist;
            _moremodel.VisitsList = _visitList;
            _moremodel.CommentList = _commentlist;

            return PartialView(_moremodel);
        }

        public ActionResult AjaxPersonComm(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var _result = (from personcomm in db.PersonCommunications
                           join comm in db.Communications on personcomm.CommunicationId equals comm.Id
                           join commtype in db.TypeOfCommunications on comm.TypeOfCommunicationId equals commtype.Id
                           where personcomm.PersonId == PersonId
                           select new CommViewModel
                           {
                               Address_Number = comm.Address_Number,
                               TypeOfCommunication = commtype.CommType,
                               CommName = comm.CommName,
                           }).ToList();

            return PartialView(_result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxUpdateInfoes(int? PersonId, string Infoes)
        {
            if (PersonId == null)
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                Person _pers = db.Persons.Find(PersonId);
                if (_pers == null)
                {
                    return Json(new { Result = false, Message = "Персона не найдена" }, JsonRequestBehavior.AllowGet);
                }

                _pers.Note = Infoes;

                db.Entry(_pers).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Во время выполнения запроса возникла критическая ошибка: " + ex.Message }, JsonRequestBehavior.AllowGet);
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