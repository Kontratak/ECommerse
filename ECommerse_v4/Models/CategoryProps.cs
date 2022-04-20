using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Models
{
    public class CategoryProps
    {
        [BsonId]
        public Guid id { get; set; }
        public string name { get; set; }
        public List<string> value { get; set; }

    }
}
