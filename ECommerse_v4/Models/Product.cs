using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Models
{
    public class Product
    {

        [BsonId]
        public Guid id { get; set; }
        public List<Image> images { get; set; }
        public Guid categoryId { get; set; }
        public Guid subCategoryId { get; set; }

        public List<ProductProps> properties;

        public List<Review> reviews;

        public Guid companyId;

        public Guid EmployeeId;

    }
}
