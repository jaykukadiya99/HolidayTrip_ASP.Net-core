using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using HolidayTrip.Models;
using MongoDB.Bson;

namespace HolidayTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private IMongoCollection<CityCollection> mongoCollection;

        public IMongoCollection<CityCollection> GetMongoCollection()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("HolidayTrip");
            return db.GetCollection<CityCollection>("CityCollection");
        }

        // GET: api/City
        [HttpGet]
        public ActionResult Get()
        {
            mongoCollection = GetMongoCollection();
            var result = mongoCollection.Find(FilterDefinition<CityCollection>.Empty).ToList();
            //var result = mongoCollection.Find(ag => ag.Status == 1).ToList();
            return Ok(result);
        }

        // GET: api/City/5
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.Find<CityCollection>(ag => ag.Id == objId).FirstOrDefault();
            return Ok(result);
        }

        // POST: api/City
        [HttpPost]
        public ActionResult Post(CityCollection value)
        {
            mongoCollection = GetMongoCollection();
            mongoCollection.InsertOne(value);
            return Created("/", value);
        }

        // PUT: api/City/5
        [HttpPut("{id}")]
        public ActionResult Put(string id, CityCollection value)
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
            var result = mongoCollection.DeleteOne(ag => ag.Id == objId);
            return Ok(result);
        }
    }
}
