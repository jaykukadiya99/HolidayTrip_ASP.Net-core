using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HolidayTrip.Models;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        [HttpGet, Authorize]
        public IEnumerable<string> Auth()
        {
            return new string[] { "John Doe", "Jane Doe" };
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

                //return Ok(data);

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:58030",
                    audience: "http://localhost:4200",
                    claims: new List<Claim>(),
                    //expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            //else
            //{
            //    mongoCollection.InsertOne(newCust);
            //    dynamic data = new JObject();
            //    data.msg = "new user";
            //    data.otp = otp;
            //    data.customer = Newtonsoft.Json.JsonConvert.SerializeObject(newCust);

            //    return Ok(data);
            //}

            return Ok("else");
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
