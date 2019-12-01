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
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;

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
            var result = mongoCollection.Find(FilterDefinition<AgentCollection>.Empty).ToList();
            //var result = mongoCollection.Find(ag => ag.Status == 1).ToList();
            return Ok(result);
        }

        // POST: api/Agent(insert)
        [HttpPost]
        public ActionResult Post(AgentCollection value)
        {

            mongoCollection = GetMongoCollection();
            mongoCollection.InsertOne(value);            
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:58030",
                audience: "http://localhost:4200",
                claims: new List<Claim>() {
                        new Claim(JwtRegisteredClaimNames.Typ,"Agent"),
                        new Claim(JwtRegisteredClaimNames.NameId,value.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Sub, Newtonsoft.Json.JsonConvert.SerializeObject(value))
                },
                //expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );
            Console.WriteLine(value.Id.ToString());

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(new { Token = tokenString });
        }


        //GET: api/Agent/5
        //[HttpGet("{id}", Name = "Get")]
        [HttpGet("{id}")]
        public ActionResult Get(String id)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.Find<AgentCollection>(lm => lm.Id == objId).FirstOrDefault();
            return Ok(result);
        }

        // PUT: api/Agent/5
        [HttpPut("{id}")]
        public ActionResult Put(string id)
        {
            try
            {

                var Jdata = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(Request.Form["data"]);
                Jdata.Property("id").Remove();
                AgentCollection data = JsonConvert.DeserializeObject<AgentCollection>(Jdata.ToString());

            var AgencyImage = Request.Form.Files["agencyImage"];

            if (AgencyImage!=null)
            {               
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                var filename = DateTime.Now.ToFileTime() + "_" + ContentDispositionHeaderValue.Parse(AgencyImage.ContentDisposition).FileName.Trim('"');
                var fullPath2 = Path.Combine(pathToSave, filename);

                using (var stream = new FileStream(fullPath2, FileMode.Create))
                {
                    string path = pathToSave.ToString() + "\\" + data.Images;
                    System.IO.File.Delete(path);
                    AgencyImage.CopyTo(stream);
                }

                data.Images = filename;
            }

            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.ReplaceOne<AgentCollection>(lm => lm.Id == objId, data);
            return Ok(new { msg="Profile Updated"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(String id)
        {
            var objId = new ObjectId(id);
            mongoCollection = GetMongoCollection();
            var update = Builders<AgentCollection>.Update.Set("Status", 0);
            var result = mongoCollection.UpdateOne<AgentCollection>(ag => ag.Id == objId,update);
            
            return Ok(result);
        }
    }
}
