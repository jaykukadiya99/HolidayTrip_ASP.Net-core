using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HolidayTrip.Models;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace HolidayTrip.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private IMongoCollection<CustomerCollection> mongoCollection;

        public IMongoCollection<CustomerCollection> GetMongoCollection()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("HolidayTrip");
            return db.GetCollection<CustomerCollection>("CustomerCollection");
        }

        [HttpPost]
        public ActionResult login(CustomerCollection newCust)
        {
            
            Random random = new Random();           
            int otp = random.Next(100000, 999999);

            newCust.OTP = otp;

            //return Ok(mobile);
            mongoCollection = GetMongoCollection();
            var result = mongoCollection.Find<CustomerCollection>(ag => ag.Mobile == newCust.Mobile).ToList();

            if (result.Count != 0)
            {
                var update = Builders<CustomerCollection>.Update.Set("OTP", otp);
                var updateOtp = mongoCollection.UpdateOne<CustomerCollection>(ag => ag.Mobile == newCust.Mobile, update);

                var updateObj = mongoCollection.Find<CustomerCollection>(ag => ag.Mobile == newCust.Mobile).FirstOrDefault();

                dynamic data = new JObject();
                data.msg = "old user";
                data.otp = otp;
                data.customer = Newtonsoft.Json.JsonConvert.SerializeObject(updateObj);

                return Ok(data);
            }
            else
            {
                mongoCollection.InsertOne(newCust);
                dynamic data = new JObject();
                data.msg = "new user";
                data.otp = otp;
                data.customer = Newtonsoft.Json.JsonConvert.SerializeObject(newCust);

                return Ok(data);
            }
        }

        // GET: api/Customer
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Customer
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Customer/5
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
