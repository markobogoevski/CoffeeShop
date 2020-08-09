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
                    Server.MapPath("~/Content/Images/Ingredient"), pic);
                    file.SaveAs(path);

                    // save the image path path to the database
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                    newIngredient.ImgUrl = "/Content/Images/Ingredient/" + pic;
                }
                else
                {
                   newIngredient.ImgUrl = "/Content/Images/Ingredient/default_ingredient.jpg";
                }
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
        public ActionResult Edit(IngredientModel _ingredient, HttpPostedFileBase file)
        {
            if (file != null)
            {
                string pic = Path.GetFileName(file.FileName);
                string path = Path.Combine(
                Server.MapPath("~/Content/Images/Ingredient"), pic);
                file.SaveAs(path);

                // save the image path path to the database
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

                _ingredient.ImgUrl = "/Content/Images/Ingredient/" + pic;
            }
            _repository.UpdateIngredient(_ingredient);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            _repository.DeleteIngredient(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult IngredientStatistics()
        {
            var ingredients = _repository.GetIngredients();
            return View(ingredients);
        }

        // Post: Coffee/UpdateIngredientQuantity
        public ActionResult UpdateIngredientQuantity(string id, string quantity)
        {
            _repository.UpdateIngredientStock(id, quantity);
            var ingredients = _repository.GetIngredients();
            return View("CoffeeStatistics", ingredients);
        }

        public ActionResult MostSold()
        {
            IngredientModel mostUsed= _repository.GetMostUsedIngredient();
            ViewBag.Statistics = true;
            return View("Details", mostUsed);
        }

        public ActionResult LeastSold()
        {
            IngredientModel leastUsed = _repository.GetLeastUsedIngredient();
            ViewBag.Statistics = true;
            return View("Details", leastUsed);
        }

        public ActionResult MostSoldWeek()
        {
            IngredientModel mostUsed = _repository.GetMostUsedIngredientWeek();
            ViewBag.Statistics = true;
            return View("Details", mostUsed);
        }

        public ActionResult LeastSoldWeek()
        {
            IngredientModel leastUsed = _repository.GetLeastUsedIngredientWeek();
            ViewBag.Statistics = true;
            return View("Details", leastUsed);
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
