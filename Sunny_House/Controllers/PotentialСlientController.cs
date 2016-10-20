﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class PotentialСlientController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: PotentialСlient
        public ActionResult Index()
        {
            var potentialСlients = db.PotentialСlients.Include(p => p.Event).Include(p => p.Person).Include(p => p.PersonRole);
            return View(potentialСlients.ToList());
        }

        // GET: PotentialСlient/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PotentialСlient potentialСlient = db.PotentialСlients.Find(id);
            if (potentialСlient == null)
            {
                return HttpNotFound();
            }
            return View(potentialСlient);
        }

        // GET: PotentialСlient/Create
        public ActionResult Create()
        {
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName");
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "FirstName");
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName");
            return View();
        }

        // POST: PotentialСlient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ClientId,PersonId,EventId,RoleId,Note")] PotentialСlient potentialСlient)
        {
            if (ModelState.IsValid)
            {
                db.PotentialСlients.Add(potentialСlient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", potentialСlient.EventId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "FirstName", potentialСlient.PersonId);
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", potentialСlient.RoleId);
            return View(potentialСlient);
        }

        // GET: PotentialСlient/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PotentialСlient potentialСlient = db.PotentialСlients.Find(id);
            if (potentialСlient == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", potentialСlient.EventId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "FirstName", potentialСlient.PersonId);
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", potentialСlient.RoleId);
            return View(potentialСlient);
        }

        // POST: PotentialСlient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ClientId,PersonId,EventId,RoleId,Note")] PotentialСlient potentialСlient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(potentialСlient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EventId = new SelectList(db.Events, "EventId", "EventName", potentialСlient.EventId);
            ViewBag.PersonId = new SelectList(db.Persons, "PersonId", "FirstName", potentialСlient.PersonId);
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", potentialСlient.RoleId);
            return View(potentialСlient);
        }

        // GET: PotentialСlient/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PotentialСlient potentialСlient = db.PotentialСlients.Find(id);
            if (potentialСlient == null)
            {
                return HttpNotFound();
            }
            return View(potentialСlient);
        }

        // POST: PotentialСlient/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PotentialСlient potentialСlient = db.PotentialСlients.Find(id);
            db.PotentialСlients.Remove(potentialСlient);
            db.SaveChanges();
            return RedirectToAction("Index");
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
