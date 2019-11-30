using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HolidayTrip.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HolidayTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InquiryController : ControllerBase
    {
        IMongoCollection<InquiryCollection> mongoCollection;
        
        public IMongoCollection<InquiryCollection> GetMongoCollection()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("HolidayTrip");
            return db.GetCollection<InquiryCollection>("InquiryCollection");
        }

        // GET: api/Inquiry
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Inquiry/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Inquiry
        [HttpPost]
        public ActionResult Post(InquiryCollection value)
        {
            mongoCollection = GetMongoCollection();
            mongoCollection.InsertOne(value);

            return Ok(new { msg = "Inquiry Generated", data = value });
        }

        // DELETE: api/ApiWithActions/5
        [HttpPut("{id}")]
        public ActionResult Put(string id)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var update = Builders<InquiryCollection>.Update.Set("Status", 1);
            var result = mongoCollection.UpdateOne<InquiryCollection>(lm => lm.Id == objId, update);
            return Ok(new { msg = "", data = id });
        }
    }
}
