using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using HolidayTrip.Models;

namespace HolidayTrip.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class AllRouteController : ControllerBase
    {
        private IMongoDatabase mongoDatabase;

        //Generic method to get the mongodb database details  
        public IMongoDatabase GetMongoDatabase()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            return mongoClient.GetDatabase("HolidayTrip");
        }

        // GET: api/AllRoute/AgentLogin
        [HttpPost]
        public ActionResult AgentLogin(AgentCollection data)
        {
            mongoDatabase = GetMongoDatabase();

            var result = mongoDatabase.GetCollection<AgentCollection>("AgentCollection").Find<AgentCollection>(a => a.AgencyEmail == data.AgencyEmail && a.Pass == data.Pass).ToList();

            if (result.Count!=0)
            {
                return Ok(result);
            }
            else
            {
                //send status 204 nocontent found
                return NoContent();
            }
        }

        // GET: api/AllRoute/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/AllRoute
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/AllRoute/5
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
