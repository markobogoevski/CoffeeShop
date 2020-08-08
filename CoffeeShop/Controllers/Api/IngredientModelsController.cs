using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CoffeeShop.Models;

namespace CoffeeShop.Controllers.Api
{
    public class IngredientModelsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/IngredientModels
        public IQueryable<IngredientModel> GetIngredients()
        {
            return db.Ingredients;
        }

        // GET: api/IngredientModels/5
        [ResponseType(typeof(IngredientModel))]
        public IHttpActionResult GetIngredientModel(Guid id)
        {
            IngredientModel ingredientModel = db.Ingredients.Find(id);
            if (ingredientModel == null)
            {
                return NotFound();
            }

            return Ok(ingredientModel);
        }

        // PUT: api/IngredientModels/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutIngredientModel(Guid id, IngredientModel ingredientModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ingredientModel.IngredientId)
            {
                return BadRequest();
            }

            db.Entry(ingredientModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IngredientModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/IngredientModels
        [ResponseType(typeof(IngredientModel))]
        public IHttpActionResult PostIngredientModel(IngredientModel ingredientModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Ingredients.Add(ingredientModel);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (IngredientModelExists(ingredientModel.IngredientId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = ingredientModel.IngredientId }, ingredientModel);
        }

        // DELETE: api/IngredientModels/5
        [ResponseType(typeof(IngredientModel))]
        public IHttpActionResult DeleteIngredientModel(Guid id)
        {
            IngredientModel ingredientModel = db.Ingredients.Find(id);
            if (ingredientModel == null)
            {
                return NotFound();
            }

            db.Ingredients.Remove(ingredientModel);
            db.SaveChanges();

            return Ok(ingredientModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IngredientModelExists(Guid id)
        {
            return db.Ingredients.Count(e => e.IngredientId == id) > 0;
        }
    }
}