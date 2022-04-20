using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Models
{
    public class Review
    {

        [BsonId]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public User User { get; set; }
        public DateTime Time { get; set; }

    }
}
