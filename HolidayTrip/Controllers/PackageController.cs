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
                var totalImg = Request.Form.Files.Count-1;

                for(int i=0;i<totalImg-2;i++)
                {

                }

                var file = Request.Form.Files[0].FileName;
                var file1 = Request.Form.Files[1].FileName;
                var file2 = Request.Form.Files[2].FileName;
                var file3 = Request.Form.Files[3].FileName;
                var data = Request.Form["data"];                


                //PackageCollection pc = new PackageCollection();                

                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);


                return Ok(new {file,file1,file2,file3,data,totalImg });
                //return Ok(new { dbPath, dbPath1, data });
                //if (file.Length > 0)
                //{
                //    var fileName = DateTime.Now.ToFileTime()+ "_" +ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                //    var fullPath = Path.Combine(pathToSave, fileName);
                //    var dbPath = Path.Combine(folderName, fileName);

                    //    using (var stream = new FileStream(fullPath, FileMode.Create))
                    //    {
                    //        file.CopyTo(stream);
                    //    }

                    //    var fileName1 = DateTime.Now.ToFileTime() + "_" + ContentDispositionHeaderValue.Parse(file1.ContentDisposition).FileName.Trim('"');
                    //    var fullPath1 = Path.Combine(pathToSave, fileName1);
                    //    var dbPath1 = Path.Combine(folderName, fileName1);

                    //    using (var stream = new FileStream(fullPath1, FileMode.Create))
                    //    {
                    //        file1.CopyTo(stream);
                    //    }

                    //    return Ok(new { dbPath ,dbPath1,data});

                    //}
                    //value.MainImage = dbPath;
                    //mongoCollection = GetMongoCollection();
                    //mongoCollection.InsertOne(value);

                    //pc.Title = Request.Form["Title"];
                    //pc.MainImage = fileName;
                    //pc.CategoryId = Request.Form["CategoryId"];
                    //pc.AgentId = Request.Form["AgentId"];
                    //pc.FixedDepatureDate = Request.Form["FixedDepatureDate"];
                    //pc.Description = Request.Form["Description"];
                    ////pc.Itinerary = Request.Form["Itinerary"];
                    //pc.Inclusion = Request.Form["Inclusion"];
                    //pc.Exclusion = Request.Form["Exclusion"];
                    //pc.OtherInfo = Request.Form["OtherInfo"];
                    //pc.TandC = Request.Form["TandC"];
                    //pc.CityIncluded = Request.Form["CityIncluded"];
                    //pc.Price = Convert.ToDouble(Request.Form["Price"]);
                    //pc.PriceDesc = Request.Form["PriceDesc"];
                    //pc.Brochure = Request.Form["Brochure"];
                    //pc.TrendingRank = Convert.ToInt32(Request.Form["TrendingRank"]);
                    //pc.InsertedDate = Request.Form["InsertedDate"];
                    //pc.Status = Convert.ToInt32(Request.Form["Status"]);

                    //mongoCollection = GetMongoCollection();
                    //mongoCollection.InsertOne(pc);


                //else
                //{
                //    return BadRequest();
                //}
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
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
