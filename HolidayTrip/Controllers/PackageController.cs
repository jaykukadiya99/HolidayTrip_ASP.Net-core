using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HolidayTrip.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

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
            //var handler = new JwtSecurityTokenHandler();
            //string token = Request.Headers["Authorization"];
            //Console.WriteLine(token);

            //token = token.Replace("Bearer ", "");
            //var jsonToken = handler.ReadToken(stream);
            //var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            //var tokenId = tokenS.Claims.First(cl => cl.Type == "nameid").Value;

            //Console.WriteLine(tokenId);


            mongoCollection = GetMongoCollection();
            var result = mongoCollection.Find(FilterDefinition<PackageCollection>.Empty).ToList();
            return Ok(result);
        }

        //GET: api/Agent/5
        [HttpGet("{id}")]
        public ActionResult Get(String id)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.Find<PackageCollection>(lm => lm.Id == objId).FirstOrDefault();

            return Ok(result.Id);
        }

        // POST: api/Package
        [HttpPost("{id}"), DisableRequestSizeLimit]
        public ActionResult Post(string id)
        {

            try
            {
                //var handler = new JwtSecurityTokenHandler();
                //string token = Request.Headers["Authorization"];
                //token = token.Replace("Bearer ", "");                
                //var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                //var tokenId = tokenS.Claims.First(cl => cl.Type == "nameid").Value;

                var rawData = Request.Form["data"];
                PackageCollection data = JsonConvert.DeserializeObject<PackageCollection>(Request.Form["data"]);


                var totalImg = Request.Form.Files.Count();
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                for (int i=0; i <= totalImg-3; i++)
                {                   
                    var file = Request.Form.Files[i];

                    var fileName = DateTime.Now.ToFileTime() + "_" + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    //var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                       // file.CopyTo(stream);
                    }
                    data.Itinerary.ElementAt(i).Images = fileName;
                }

                if (Request.Form.Files["MainImage"].Length > 0)
                {
                    var MainImg = Request.Form.Files[totalImg - 2];

                    var MainImgName = DateTime.Now.ToFileTime() + "_" + ContentDispositionHeaderValue.Parse(MainImg.ContentDisposition).FileName.Trim('"');
                    var fullPath1 = Path.Combine(pathToSave, MainImgName);
                    //var dbPath1 = Path.Combine(folderName, MainImgName);

                    using (var stream = new FileStream(fullPath1, FileMode.Create))
                    {
                       MainImg.CopyTo(stream);
                    }
                    Console.WriteLine("main img");
                    data.MainImage = MainImgName;
                }                

                if(Request.Form.Files["Brochure"].Length > 0)
                {
                    var Brocher = Request.Form.Files[totalImg - 1];

                    var BrocherName = DateTime.Now.ToFileTime() + "_" + ContentDispositionHeaderValue.Parse(Brocher.ContentDisposition).FileName.Trim('"');
                    var fullPath2 = Path.Combine(pathToSave, BrocherName);
                    //var dbPath2 = Path.Combine(folderName, BrocherName);

                    using (var stream = new FileStream(fullPath2, FileMode.Create))
                    {
                        Brocher.CopyTo(stream);
                    }
                    Console.WriteLine("Brocher");
                    data.Brochure = BrocherName;
                }

                data.AgentId = id;
                mongoCollection = GetMongoCollection();
                mongoCollection.InsertOne(data);
                
                return Ok(new { msg="Inserted Sucessful"});
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        // PUT: api/Package/5
        [HttpPut("{id}")]
        public ActionResult Put(string id,PackageCollection value)
        {
            mongoCollection = GetMongoCollection();
            var objId = new ObjectId(id);
            var result = mongoCollection.ReplaceOne(lm => lm.Id == objId, value);
            return Ok(result);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var objId = new ObjectId(id);
            mongoCollection = GetMongoCollection();
            var update = Builders<PackageCollection>.Update.Set("Status", 0);
            var result = mongoCollection.UpdateOne<PackageCollection>(lm => lm.Id == objId, update);

            return Ok(result);
        }
    }
}
