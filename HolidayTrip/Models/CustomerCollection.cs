using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTrip.Models
{
    public class CustomerCollection
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        [BsonElement]
        public string FirstName { get; set; }
        [BsonElement]
        public string LastName { get; set; }
        [BsonElement]
        public string Mobile { get; set; }
        [BsonElement]
        public string Email { get; set; }
        [BsonElement]
        public int OTP { get; set; }
    }
}
