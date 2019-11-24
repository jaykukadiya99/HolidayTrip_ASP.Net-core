using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HolidayTrip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HolidayTrip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private IMongoCollection<AgentCollection> mongoCollection;

        public IMongoCollection<AgentCollection> GetMongoCollection()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("HolidayTrip");
            return db.GetCollection<AgentCollection>("AgentCollection");
        }

        // GET: api/Agent (get all data)
        [HttpGet]
        public ActionResult Get()
        {
            mongoCollection = GetMongoCollection();
            //fetch the details from CustomerDB and pass into view  
            var result = mongoCollection.Find(FilterDefinition<AgentCollection>.Empty).ToList();
            return Ok(result);
        }

        //post:api/Agent/login (login)
        //[HttpPost]
        //public ActionResult login(AgentCollection data)
        //{
        //    mongoCollection = GetMongoCollection();
        //    var result = mongoCollection.Find<AgentCollection>(a =>a.AgencyEmail == data.AgencyEmail && a.Pass == data.Pass).FirstOrDefault();

        //    if(result !=null)
        //    {
        //        return Ok(result);
        //    }            
        //    else
        //    {
        //        //send status 204 nocontent found
        //        return NoContent();
        //    }
        //}


        // POST: api/Agent(insert)
        [HttpPost]
        public ActionResult Post(AgentCollection value)
        {
            mongoCollection = GetMongoCollection();
            mongoCollection.InsertOne(value);
            return Created("/", value);
        }


        //GET: api/Agent/5
        [HttpGet("{id}", Name = "Get")]
        public ActionResult Get(String id)
        {
            mongoCollection = GetMongoCollection();
            var objectId = new ObjectId(id);

            var result = mongoCollection.Find<AgentCollection>(ag => ag.Id == objectId).FirstOrDefault();
            return Ok(result);
        }

        // PUT: api/Agent/5
        [HttpPut("{id}")]
        public void Put(String id, AgentCollection value)
        {
            mongoCollection = GetMongoCollection();
            var objectId = new ObjectId(id);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
