using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HolidayTrip.Models
{
    public class PackageCollection
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        [BsonElement]
        public string Title { get; set; }
        [BsonElement]
        public string MainImage { get; set; } 
        [BsonElement]
        public string CategoryId { get; set; }
        [BsonElement]
        public string AgentId { get; set; }
        [BsonElement]
        public string FixedDepatureDate { get; set; }
        [BsonElement]
        public string Description { get; set; }
        [BsonElement]
        public IEnumerable<ItineraryClass> Itinerary { get; set; }
        [BsonElement]
        public string Inclusion { get; set; }
        [BsonElement]
        public string Exclusion { get; set; }
        [BsonElement]
        public string OtherInfo { get; set; }
        [BsonElement]
        public string TandC { get; set; }
        [BsonElement]
        public IEnumerable<string> CityIncluded { get; set; }
        [BsonElement]
        public double Price { get; set; }
        [BsonElement]
        public string PriceDesc { get; set; }
        [BsonElement]
        public string Brochure { get; set; }
        [BsonElement]
        public int TrendingRank { get; set; }
        [BsonElement]
        public string InsertedDate { get; set; }
        [BsonElement]
        public int Status { get; set; }

    }

    public class ItineraryClass
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Images { get; set; }
    }
}
