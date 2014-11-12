using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PlayServer.Models;

namespace PlayServer.Controllers
{
    public class PlayerDBsController : Controller
    {
        private PlayServerContext db = new PlayServerContext();

        // GET: PlayerDBs
        public ActionResult Index()
        {
            return View(db.PlayerDBs.ToList());
        }

        // GET: PlayerDBs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDB playerDB = db.PlayerDBs.Find(id);
            if (playerDB == null)
            {
                return HttpNotFound();
            }
            return View(playerDB);
        }

        // GET: PlayerDBs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlayerDBs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,URL")] PlayerDB playerDB)
        {
            if (ModelState.IsValid)
            {
                db.PlayerDBs.Add(playerDB);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(playerDB);
        }

        // GET: PlayerDBs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDB playerDB = db.PlayerDBs.Find(id);
            if (playerDB == null)
            {
                return HttpNotFound();
            }
            return View(playerDB);
        }

        // POST: PlayerDBs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,URL")] PlayerDB playerDB)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playerDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(playerDB);
        }

        // GET: PlayerDBs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerDB playerDB = db.PlayerDBs.Find(id);
            if (playerDB == null)
            {
                return HttpNotFound();
            }
            return View(playerDB);
        }

        // POST: PlayerDBs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlayerDB playerDB = db.PlayerDBs.Find(id);
            db.PlayerDBs.Remove(playerDB);
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
