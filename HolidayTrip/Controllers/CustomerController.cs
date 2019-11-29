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
//using Microsoft.AspNetCore.Authentication.JwtBearer;
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
                
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:58030",
                    audience: "http://localhost:4200",
                    claims: new List<Claim>() { 
                        new Claim(JwtRegisteredClaimNames.Typ,"Old User"),
                        new Claim(JwtRegisteredClaimNames.NameId,updateObj.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, Newtonsoft.Json.JsonConvert.SerializeObject(updateObj)),                       
                    },
                    //expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            else
            {
                mongoCollection.InsertOne(newCust);

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:58030",
                    audience: "http://localhost:4200",
                    claims: new List<Claim>() {
                        new Claim(JwtRegisteredClaimNames.Typ,"New User"),
                        new Claim(JwtRegisteredClaimNames.NameId,newCust.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, Newtonsoft.Json.JsonConvert.SerializeObject(newCust)),
                    },
                    //expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }        
        }

        [HttpPost]        
        public ActionResult customerOtp(CustomerCollection cust)
        {
            //var handler = new JwtSecurityTokenHandler();
            //string stream = Request.Headers["Authorization"];
            //stream = stream.Replace("Bearer ", "");
            ////var jsonToken = handler.ReadToken(stream);
            //var tokenS = handler.ReadToken(stream) as JwtSecurityToken;
            //var id = tokenS.Claims.First(cl => cl.Type == "nameid").Value;

            var handler = new JwtSecurityTokenHandler();
            string token = Request.Headers["Authorization"];
            Console.WriteLine(token);
            //token = token.Replace("Bearer ", "");
            //var jsonToken = handler.ReadToken(stream);
            //var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            //var tokenId = tokenS.Claims.First(cl => cl.Type == "nameid").Value;

            //return Ok(new { data = id});

            mongoCollection = GetMongoCollection();
            var result = mongoCollection.Find<CustomerCollection>(ag => ag.Mobile == cust.Mobile && ag.OTP == cust.OTP).ToList();
            if (result.Count == 1)
            {
                return Ok(new { msg = "Valid User" ,token});
            }
            else
            {
                return Ok(new { msg = "Invalid User" ,token});
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
