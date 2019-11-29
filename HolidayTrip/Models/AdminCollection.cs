using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTrip.Models
{
    public class AdminCollection
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string username { get; set; }
        [BsonElement]
        public string pass { get; set; }
    }
}
