namespace CoffeeShop.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using CoffeeShop.Enumerations;
    using CoffeeShop.Models;
    using CoffeeShop.Services;

    [Authorize]
    public class IngredientController : Controller
    {
        private Repository _repository;

        public IngredientController()
        {
            _repository = Repository.GetInstance();
        }

        // GET: Ingredient
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                return View(_repository.GetIngredients().ToList());
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }
       
        // GET: Ingredient/Details/5
        [AllowAnonymous]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                var ingredient = _repository.FindIngredient(id);
                return View(ingredient);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Ingredient/Create
        [Authorize(Roles=UserRoles.Admin + "," +UserRoles.Owner)]
        public ActionResult Create()
        {
            var ingredientModel = new IngredientModel();
            return View(ingredientModel);
        }

        // POST: Ingredient/Create
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
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
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                return View(_repository.FindIngredient(id));
            }
            catch(Exception)
            {
                return HttpNotFound();
            }
            
        }

        // POST: Ingredient/Edit/5
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
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
            try
            {
                _repository.UpdateIngredient(_ingredient);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        // POST: Ingredient/Delete/5
        public ActionResult Delete(Guid id)
        {
            try
            {
                _repository.DeleteIngredient(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Ingredient/IngredientStatistics
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult IngredientStatistics()
        {
            try
            {
                var ingredients = _repository.GetIngredients().ToList();
                return View(ingredients);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // POST: Ingredient/UpdateIngredientQuantity
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult UpdateIngredientQuantity(string id, string quantity)
        {
            try
            {
                _repository.UpdateIngredientStock(Guid.Parse(id), quantity);
                var ingredients = _repository.GetIngredients().ToList();
                return View("CoffeeStatistics", ingredients);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Ingredient/MostUsed
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult MostUsed()
        {
            try
            {
                IngredientModel mostUsed = _repository.GetMostUsedIngredient();
                ViewBag.Statistics = true;
                return View("Details", mostUsed);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Ingredient/LeastUsed
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult LeastUsed()
        {
            try
            {
                IngredientModel leastUsed = _repository.GetLeastUsedIngredient();
                ViewBag.Statistics = true;
                return View("Details", leastUsed);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Ingredient/MostUsedWeek
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult MostUsedWeek()
        {
            try
            {
                IngredientModel mostUsed = _repository.GetMostUsedIngredientWeek();
                ViewBag.Statistics = true;
                return View("Details", mostUsed);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        // GET: Ingredient/LeastUsedWeek
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.Owner)]
        public ActionResult LeastUsedWeek()
        {
            try
            {
                IngredientModel leastUsed = _repository.GetLeastUsedIngredientWeek();
                ViewBag.Statistics = true;
                return View("Details", leastUsed);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
