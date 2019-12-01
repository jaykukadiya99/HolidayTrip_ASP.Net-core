using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HolidayTrip.Models
{
    public class AgentCollection
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string IdAsString
        {
            get { return Id.ToString(); }
            set { Id = ObjectId.Parse(value); }
        }

        [BsonElement]
        public string AgentName { get; set; }
        [BsonElement]
        public string AgencyName { get; set; }
        [BsonElement]
        public AddressDetails AgencyAddress { get; set; }
        [BsonElement]
        public IEnumerable<string> ContactMobile { get; set; }
        [BsonElement]
        public string AgencyEmail { get; set; }
        [BsonElement]
        public string Website { get; set; }
        [BsonElement]
        public string Images { get; set; }
        [BsonElement]
        public string Pass { get; set; }
        [BsonElement]
        public int Status { get; set; }
    }

    public class AddressDetails
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
    }
}
