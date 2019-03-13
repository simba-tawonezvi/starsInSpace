using System.Data.Entity;
using MVCStars.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace MVCStars.Controllers
{

    public class HomeController : Controller
    {
        //allow code to work with databse
        private StarsInSpaceEntities2 db = new StarsInSpaceEntities2();


        public ActionResult Index(string starType, string searchString)
        {
            var TypeList = new List<string>();

            var TypeQuery = from a in db.Stars
                            orderby a.Type
                            select a.Type;

            TypeList.AddRange(TypeQuery.Distinct());
            ViewBag.StarType = new SelectList(TypeList);

            //LINQ querry to get records from database
            var stars = from s in db.Stars
                        select s;

            //search for star by name type
            if (!String.IsNullOrEmpty(searchString))
            {
                stars = stars.Where(s => s.Name.Contains(searchString));
            }

            //search for star by type
            if (!string.IsNullOrEmpty(starType))
            {
                stars = stars.Where(x => x.Type == starType);
            }


            //Passing the data to the view
            return View(stars);
        }


        // add a record
        public ActionResult Create()
        {
            return View();
        }

        // details of new record that are then saved to the db
        [HttpPost]
        public ActionResult Create(Star star)
        {
            if (star.ImageUrl == null)
            {
                star.ImageUrl = "https://tinyurl.com/yaur249h";
            }

            if (ModelState.IsValid)
            {
                db.Stars.Add(star);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(star);

        }

        // edits record, find details using id
        public ActionResult Edit(int id)
        {
            Star star = db.Stars.Find(id);

            return View(star);
        }

        // save edits to the db
        [HttpPost]
        public ActionResult Edit(Star star)
        {
            if (star.ImageUrl == null)
            {
                star.ImageUrl = "https://tinyurl.com/yaur249h";
            }

            if (ModelState.IsValid)
            {
                db.Entry(star).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(star);


        }

        //shows details of record using id
        public ActionResult Details(int id)
        {
            Star star = db.Stars.Find(id);
            return View(star);
        }

        //shows details of the star to be deleted
        public ActionResult Delete(int id)
        {
            Star star = db.Stars.Find(id);
            return View(star);
        }

        // confirms the star to be deleted and saves changes
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            Star star = db.Stars.Find(id);
            db.Stars.Remove(star);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        //used by Index view to refresh rows with AJAX when likes or dislikes updated
        public ActionResult _StarRowData(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Star star = db.Stars.Find(id);
            if (star == null)
            {
                return HttpNotFound();
            }
            return PartialView(star);
        }

        // increments like count when button is clicked
        [HttpPost]
        public ActionResult Like(int? id)
        {
            Star star = db.Stars.Find(id);

            int likes = star.LikeCount;
            star.LikeCount = likes + 1;

            if (ModelState.IsValid)
            {
                db.Entry(star).State = EntityState.Modified;
                db.SaveChanges();
            }

            star = db.Stars.Find(id);

            return PartialView("_StarRowData",star);
        }

        //increments dislike count when button is clicked
        [HttpPost]
        public ActionResult Dislike(int id)
        {
            Star star = db.Stars.Find(id);

            int dislikes = star.DislikeCount;
            star.DislikeCount = dislikes + 1;

            if (ModelState.IsValid)
            {
                db.Entry(star).State = EntityState.Modified;
                db.SaveChanges();
            }

            return PartialView("_StarRowData", star);
        }



        // from template
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}