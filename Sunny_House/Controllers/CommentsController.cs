﻿using System;
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
        public async Task<ActionResult> CommentShow(int? CommentId)
        {
            var comments = db.Comments.Include(c => c.Address)
                    .Include(c => c.CommentSource)
                    .Include(c => c.Event)
                    .Include(c => c.Exercise)
                    .Include(c => c.Person)
                    .Include(c => c.Person1)
                    .Where(c => c.CommentId == CommentId || CommentId == null);
            return View(await comments.ToListAsync());
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
            ViewBag.AddressId = new SelectList(db.Addresses, "AddressId", "Alias");
            ViewBag.SourceId = new SelectList(db.CommentSources, "SourceId", "SourceName");
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName");
            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject");
            ViewBag.AboutPersonId = new SelectList(db.Persons, "PersonId", "FirstName");
            ViewBag.SignPersonId = new SelectList(db.Persons, "PersonId", "FirstName");
            return View();
        }

        // POST: Comments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentCreate([Bind(Include = "CommentId,SourceId,Date,Text,Rating,AboutPersonId,EventId,ExerciseId,AddressId,SignPersonId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                await db.SaveChangesAsync();
                return RedirectToAction("CommentShow");
            }

            ViewBag.AddressId = new SelectList(db.Addresses, "AddressId", "Alias", comment.AddressId);
            ViewBag.SourceId = new SelectList(db.CommentSources, "SourceId", "SourceName", comment.SourceId);
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", comment.EventId);
            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject", comment.ExerciseId);
            ViewBag.AboutPersonId = new SelectList(db.Persons, "PersonId", "FirstName", comment.AboutPersonId);
            ViewBag.SignPersonId = new SelectList(db.Persons, "PersonId", "FirstName", comment.SignPersonId);
            return View(comment);
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
            ViewBag.AddressId = new SelectList(db.Addresses, "AddressId", "Alias", comment.AddressId);
            ViewBag.SourceId = new SelectList(db.CommentSources, "SourceId", "SourceName", comment.SourceId);
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", comment.EventId);
            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject", comment.ExerciseId);
            ViewBag.AboutPersonId = new SelectList(db.Persons, "PersonId", "FirstName", comment.AboutPersonId);
            ViewBag.SignPersonId = new SelectList(db.Persons, "PersonId", "FirstName", comment.SignPersonId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CommentEdit([Bind(Include = "CommentId,SourceId,Date,Text,Rating,AboutPersonId,EventId,ExerciseId,AddressId,SignPersonId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("CommentShow");
            }
            ViewBag.AddressId = new SelectList(db.Addresses, "AddressId", "Alias", comment.AddressId);
            ViewBag.SourceId = new SelectList(db.CommentSources, "SourceId", "SourceName", comment.SourceId);
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", comment.EventId);
            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject", comment.ExerciseId);
            ViewBag.AboutPersonId = new SelectList(db.Persons, "PersonId", "FirstName", comment.AboutPersonId);
            ViewBag.SignPersonId = new SelectList(db.Persons, "PersonId", "FirstName", comment.SignPersonId);
            return View(comment);
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
            Comment comment = await db.Comments.FindAsync(id);
            db.Comments.Remove(comment);
            await db.SaveChangesAsync();
            return RedirectToAction("CommentShow");
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
        public async Task<ActionResult> CSEdit([Bind(Include = "SourceId,SourceName")] CommentSource commentSource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentSource).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("CSShow");
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
            CommentSource commentSource = await db.CommentSources.FindAsync(id);
            db.CommentSources.Remove(commentSource);
            await db.SaveChangesAsync();
            return RedirectToAction("CSShow");
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