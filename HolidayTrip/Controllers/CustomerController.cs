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
using MongoDB.Bson;
using Newtonsoft.Json;

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

        private IMongoDatabase mongoDatabase;

        //Generic method to get the mongodb database details  
        public IMongoDatabase GetMongoDatabase()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            return mongoClient.GetDatabase("HolidayTrip");
        }

        //[HttpGet, Authorize]
        //public IEnumerable<string> Auth()
        //{
        //    return new string[] { "John Doe", "Jane Doe" };
        //}


        //post:api/Customer/login
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

        //post:api/Customer/customerOtp
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
                return Ok(new { msg = "Valid User", token });
            }
            else
            {
                return Ok(new { msg = "Invalid User", token });
            }
        }

        // GET: api/Customer
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{   
        //    return new string[] { "value1", "value2" };
        //}



        // GET: api/Customer/GetOne/5
        [HttpGet("{id}")]
        public ActionResult GetOne(string id)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.Find<CustomerCollection>(lm => lm.Id == objId).FirstOrDefault();
            return Ok(result);
        }

        // POST: api/Customer
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Customer/5
        [HttpPut("{id}")]
        public ActionResult Update(string id)
        {
            var Jdata = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(Request.Form["data"]);
            Jdata.Property("id").Remove();
            CustomerCollection data = JsonConvert.DeserializeObject<CustomerCollection>(Jdata.ToString());
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.ReplaceOne(lm => lm.Id == objId, data);
            return Ok(new { msg = "Details Updated", data = data });
        }


        [HttpGet("{id}")]
        public ActionResult customerInquiry (string id)
        {
            //var objCustId = new ObjectId(id);
            mongoDatabase = GetMongoDatabase();
            var inq = mongoDatabase.GetCollection<InquiryCollection>("InquiryCollection").AsQueryable().Where(cu => cu.CustomerId==id);
            var pack = mongoDatabase.GetCollection<PackageCollection>("PackageCollection").AsQueryable();
            //var agent = mongoDatabase.GetCollection<AgentCollection>("AgentCollection").AsQueryable();

            //var qu = from p in pack.AsQueryable()
            //         join a in agent.AsQueryable() on p.AgentId equals a.IdAsString into data
            //         select new { package = p, agent = data };


            var query = from i in inq
                        //join a in agent on i.AgentId equals a.IdAsString into AgentData 
                        join p in pack on i.PackageId equals p.IdAsString into PackData
                        //select new { inq = i,agent=AgentData};
                        //select new { inq = i, pack = PackData , agent= AgentData};
                        select new { inq = i, pack = PackData};

            var result = query.ToList().Reverse<Object>();            
            return Ok(result);
        }

        [HttpGet]
        public ActionResult customerFilter()
        {
            //var pa = mongoDatabase.GetCollection<PackageCollection>("PackageCollection").Find(pc => pc.Catego).ToList();
            return Ok();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
