using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HolidayTrip.Models;
using MongoDB.Driver;

namespace HolidayTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private IMongoCollection<PackageCollection> mongoCollection;

        public IMongoCollection<PackageCollection> GetMongoCollection()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("HolidayTrip");            
            return db.GetCollection<PackageCollection>("PackageCollection");
        }

        // GET: api/Package
        [HttpGet]
        public ActionResult Get()
        {

            mongoCollection = GetMongoCollection();

            var result = mongoCollection.Find(FilterDefinition<PackageCollection>.Empty).ToList();
            return Ok(result);
        }

        //[HttpPost]
        //public ActionResult GetOne(int value)
        //{

        //    mongoCollection = GetMongoCollection();
        //    var result = mongoCollection.Find<PackageCollection>(p => p.AgentId == value.ToString()).ToList();
        //    return Ok(result);
        //}

        // GET: api/Package/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Package
        [HttpPost]
        public void Post(PackageCollection value)
        {
            mongoCollection = GetMongoCollection();
            mongoCollection.InsertOne(value);

        }

        // PUT: api/Package/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
