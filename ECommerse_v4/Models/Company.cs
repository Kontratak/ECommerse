using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Models
{
    public class Company
    {
        [BsonId]
        public Guid id { get; set; }
        public string name { get; set; }
        public List<Guid> Employees { get; set; }
    }
}
