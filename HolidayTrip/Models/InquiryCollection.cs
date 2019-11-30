using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTrip.Models
{
    public class InquiryCollection
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string CustomerId { get; set; }
        [BsonElement]
        public string AgentId { get; set; }
        [BsonElement]
        public string PackageId { get; set; }
        [BsonElement]
        public int Person { get; set; }
        [BsonElement]
        public string InquiryAbout { get; set; }
        [BsonElement]
        public string InquiryDate { get; set; }
        [BsonElement]
        public int InquiryStatus { get; set; }

    }
}
