using ECommerse_v4.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Middleware
{
    public class CategoryMiddleware
    {

        public static Category Get(Guid id,IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Category>("Categories");
            return collection.Find(category => category.id == id).FirstOrDefault();
        }
        public static List<Category> GetSubCategories(List<Guid> ids,IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Category>("Categories");
            return collection.Find(category => ids.Contains(category.id)).ToList();
        }

        public static void Add(Category category, IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Category>("Categories");
            collection.InsertOneAsync(category);
        }

        public static List<Category> GetAll(IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Category>("Categories");
            return collection.Find(category => true).ToList();
        }

    }
}
