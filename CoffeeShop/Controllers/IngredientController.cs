using System;
using System.Linq;
using System.Net;
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
                var ingredients = _repository.GetIngredients();
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
        public ActionResult Create([Bind(Include = "IngredientId,Name,Price,ImgUrl,Description")] IngredientModel newIngredient)
        {
            if (ModelState.IsValid)
            {
                _repository.CreateIngredient(newIngredient);
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
                return View(db.Ingredients.Find(id));
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Guid id)
        {
            _repository.DeleteIngredient(id);
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
