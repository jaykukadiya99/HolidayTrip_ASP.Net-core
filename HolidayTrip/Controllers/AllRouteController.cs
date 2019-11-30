using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using HolidayTrip.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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


        public ActionResult AdminLogin(AdminCollection data)
        {
            try
            {
                mongoDatabase = GetMongoDatabase();
                var result = mongoDatabase.GetCollection<AdminCollection>("AdminCollection")
                    .Find<AdminCollection>(lm => lm.username==data.username && lm.pass==data.pass).ToList();

                if(result.Count==1)
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var tokeOptions = new JwtSecurityToken(
                        issuer: "http://localhost:58030",
                        audience: "http://localhost:4200",
                        claims: new List<Claim>() {
                        new Claim(JwtRegisteredClaimNames.Typ,"Admin"),
                        new Claim(JwtRegisteredClaimNames.NameId,result.FirstOrDefault().Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, Newtonsoft.Json.JsonConvert.SerializeObject(result.FirstOrDefault())),
                        },
                        //expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: signinCredentials
                    );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    return Ok(new { Token = tokenString });
                }
                else
                {
                    return Ok(new { msg="Invalid User"});
                }
                
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        // GET: api/AllRoute/custPack
        public ActionResult custPack()
        {
            mongoDatabase = GetMongoDatabase();
            var pack = mongoDatabase.GetCollection<PackageCollection>("PackageCollection");
            var agent = mongoDatabase.GetCollection<AgentCollection>("AgentCollection");

            var qu = from p in pack.AsQueryable()
                     join a in agent.AsQueryable() on p.AgentId equals a.IdAsString into data
                     select new { package=p,agent=data};

            var result = qu.ToList();
            return Ok(result);
        }

        // GET: api/AllRoute/AgentLogin
        [HttpPost]
        public ActionResult AgentLogin(AgentCollection data)
        {
            mongoDatabase = GetMongoDatabase();

            var result = mongoDatabase.GetCollection<AgentCollection>("AgentCollection").Find<AgentCollection>(a => a.AgencyEmail == data.AgencyEmail && a.Pass == data.Pass).ToList();

            if (result.Count!=0)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:58030",
                    audience: "http://localhost:4200",
                    claims: new List<Claim>() {
                        new Claim(JwtRegisteredClaimNames.Typ,"Agent"),
                        new Claim(JwtRegisteredClaimNames.NameId,result.FirstOrDefault().Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, Newtonsoft.Json.JsonConvert.SerializeObject(result.FirstOrDefault())),
                    },
                    //expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });                
            }
            else
            {
                //send status 204 nocontent found
                return NoContent();
            }
        }

        [HttpGet("{id}")]
        public ActionResult AgentPackage(string id)
        {
            mongoDatabase = GetMongoDatabase();
            //5ddc059d9b9f555138880aa0
            //var result = mongoDatabase.GetCollection<PackageCollection>("PackageCollection").Find(FilterDefinition<PackageCollection>.Empty).ToList();
            var result = mongoDatabase.GetCollection<PackageCollection>("PackageCollection").Find(a => a.AgentId==id && a.Status==0).ToList();

            return Ok(result);
        }



        // GET: api/AllRoute/5
        //[HttpGet("{id}", Name = "Get")]
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
