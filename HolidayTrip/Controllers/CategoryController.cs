using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HolidayTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private IMongoCollection<CategoryCollection> mongoCollection;

        public IMongoCollection<CategoryCollection> GetMongoCollection()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("HolidayTrip");
            return db.GetCollection<CategoryCollection>("CategoryCollection");
        }
        // GET: api/Category
        [HttpGet]
        public ActionResult Get()
        {
            mongoCollection = GetMongoCollection();
            var result = mongoCollection.Find(FilterDefinition<CategoryCollection>.Empty).ToList();
            //var result = mongoCollection.Find(ag => ag.Status == 1).ToList();
            return Ok(result);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.Find<CategoryCollection>(ag => ag.Id == objId).FirstOrDefault();
            return Ok(result);
        }

        // POST: api/Category
        [HttpPost]
        public ActionResult Post([FromBody] CategoryCollection value)
        {
            mongoCollection = GetMongoCollection();
            mongoCollection.InsertOne(value);
            return Created("/", value);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] CategoryCollection value)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.ReplaceOne(ag => ag.Id == objId, value);
            return Ok(result);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var objId = new ObjectId(id);
            mongoCollection = GetMongoCollection();
            var result = mongoCollection.DeleteOne(lm => lm.Id == objId);
            return Ok(new { msg="Deleted"});
        }
    }
}
