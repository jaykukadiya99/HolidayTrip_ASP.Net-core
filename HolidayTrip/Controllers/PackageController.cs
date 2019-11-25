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
        [HttpPost, DisableRequestSizeLimit]
        public ActionResult Post()
        {

            try
            {
                var file = Request.Form.Files[0];
                var title = Request.Form["Title"];
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    //value.MainImage = dbPath;
                    //mongoCollection = GetMongoCollection();
                    //mongoCollection.InsertOne(value);
                    return Ok(new { dbPath ,title});
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
            //return Ok(new { status = true, message = "Student Posted Successfully" });
            //mongoCollection = GetMongoCollection();
            //mongoCollection.InsertOne(value);
            //return Created("/", value);
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
