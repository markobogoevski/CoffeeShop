using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoffeeShop.Models;
using CoffeeShop.Services;
namespace CoffeeShop.Controllers
{
    public class IngredientController : Controller
    {
        public static bool isAscending = true;

        private Repository _repository;
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ingredient
        public ActionResult Index()
        {
            return View(db.Ingredients.ToList());
        }
        public IngredientController()
        {
            _repository = Repository.GetInstance();
        }
        // GET: Ingredient/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var ingredients = _repository.GetIngredient(id);
                return View(ingredients);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Ingredient/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ingredient/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IngredientModel newIngredient, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string pic = Path.GetFileName(file.FileName);
                    string path = Path.Combine(
                    Server.MapPath("~/Content/Images"), pic);
                    file.SaveAs(path);

                    // save the image path path to the database
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                    newIngredient.ImgUrl = "/Content/Images/" + pic;
                }
                else
                {
                   newIngredient.ImgUrl = "/Content/Images/default_coffee.JPG";
                }
                _repository.CreateIngredient(newIngredient);
                ViewBag.Title = "Coffee Shop blabla";
                return RedirectToAction("Index");
            }
            return View(newIngredient);

        }

        // GET: Ingredient/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                return View(_repository.GetIngredient(id));
            }
            catch(Exception)
            {
                return HttpNotFound();
            }
            
        }

        // POST: Ingredient/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IngredientId,Name,Price,ImgUrl,Description")] IngredientModel _ingredient)
        {
            if (ModelState.IsValid)
            {
                _repository.UpdateIngredient(_ingredient);
                return RedirectToAction("Index");
            }
            return View(_ingredient);
        }

        /*// GET: Ingredient/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IngredientModel ingredientModel = db.Ingredients.Find(id);
            if (ingredientModel == null)
            {
                return HttpNotFound();
            }
            return View(ingredientModel);
        }*/

        // POST: Ingredient/Delete/5
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            _repository.DeleteIngredient(id);
            Console.WriteLine("Deleeting " + id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public ActionResult OrderBy(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            switch (sortOrder)
            {
                case "price_desc":
                    ViewBag.Title = "The ingredients are displayed in descending order";
                    return View("Index", _repository.GetSortedIngredients(!isAscending));

                default:
                    ViewBag.Title = "The ingredients are displayed in ascending order";
                    return View("Index", _repository.GetSortedIngredients(isAscending));
            }
        }
    }
}
