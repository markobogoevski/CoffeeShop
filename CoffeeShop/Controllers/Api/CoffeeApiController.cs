using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using CoffeeShop.Models;

namespace CoffeeShop.Controllers.Api
{
    public class CoffeeApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/CoffeeModels
        public IEnumerable<CoffeeModel> GetCoffee()
        {
            return db.Coffee.AsNoTracking().ToList();
        }

        // GET: api/CoffeeModels/5
        [ResponseType(typeof(CoffeeModel))]
        public IHttpActionResult GetCoffeeModel(Guid id)
        {
            CoffeeModel coffeeModel = db.Coffee.Find(id);
            if (coffeeModel == null)
            {
                return NotFound();
            }

            return Ok(coffeeModel);
        }

        // PUT: api/CoffeeModels/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCoffeeModel(Guid id, CoffeeModel coffeeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != coffeeModel.CoffeeId)
            {
                return BadRequest();
            }

            db.Entry(coffeeModel).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoffeeModelExists(id))
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

        // POST: api/CoffeeModels
        [ResponseType(typeof(CoffeeModel))]
        public IHttpActionResult PostCoffeeModel(CoffeeModel coffeeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Coffee.Add(coffeeModel);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CoffeeModelExists(coffeeModel.CoffeeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = coffeeModel.CoffeeId }, coffeeModel);
        }

        // DELETE: api/CoffeeModels/5
        [ResponseType(typeof(CoffeeModel))]
        public IHttpActionResult DeleteCoffeeModel(Guid id)
        {
            CoffeeModel coffeeModel = db.Coffee.Find(id);
            if (coffeeModel == null)
            {
                return NotFound();
            }

            db.Coffee.Remove(coffeeModel);
            db.SaveChanges();

            return Ok(coffeeModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CoffeeModelExists(Guid id)
        {
            return db.Coffee.Count(e => e.CoffeeId == id) > 0;
        }
    }
}