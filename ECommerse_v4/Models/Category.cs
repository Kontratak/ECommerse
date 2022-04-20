using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Models
{
    public class Category
    {
        [BsonId]
        public Guid id { get; set; }
        public bool isMainCategory { get; set; }
        public List<Guid> subCategorys { get; set; }
        public List<CategoryProps> properties { get; set; }
    }
}
