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
using CCU_Crawler.Models;

namespace CCU_Crawler.ApiControllers
{
    public class GuestbooksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Guestbooks
        public IQueryable<Guestbook> GetGuestbooks()
        {
            return db.Guestbooks;
        }

        // GET: api/Guestbooks/5
        [ResponseType(typeof(Guestbook))]
        public IHttpActionResult GetGuestbook(int id)
        {
            Guestbook guestbook = db.Guestbooks.Find(id);
            if (guestbook == null)
            {
                return NotFound();
            }

            return Ok(guestbook);
        }

        // PUT: api/Guestbooks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGuestbook(int id, Guestbook guestbook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != guestbook.Id)
            {
                return BadRequest();
            }

            db.Entry(guestbook).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuestbookExists(id))
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

        // POST: api/Guestbooks
        [ResponseType(typeof(Guestbook))]
        public IHttpActionResult PostGuestbook(Guestbook guestbook)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Guestbooks.Add(guestbook);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = guestbook.Id }, guestbook);
        }

        // DELETE: api/Guestbooks/5
        [ResponseType(typeof(Guestbook))]
        public IHttpActionResult DeleteGuestbook(int id)
        {
            Guestbook guestbook = db.Guestbooks.Find(id);
            if (guestbook == null)
            {
                return NotFound();
            }

            db.Guestbooks.Remove(guestbook);
            db.SaveChanges();

            return Ok(guestbook);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GuestbookExists(int id)
        {
            return db.Guestbooks.Count(e => e.Id == id) > 0;
        }
    }
}