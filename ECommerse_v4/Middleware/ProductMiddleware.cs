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
    public class ProductMiddleware
    {
        private IConfiguration Configuration { get; }

        public ProductMiddleware(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static Product Add(Product product,IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Product>("Products");
            collection.InsertOneAsync(product);
            return product;
        }
        
        public static Product Get(Guid id,IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Product>("Products");
            return collection.Find(p => p.id == id).First();
        }

        public static List<Product> GetAll(IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Product>("Products");
            return collection.Find(product => true).ToList();
        }
        public static List<Product> GetCompanyProducts(Company company,IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Product>("Products");
            return collection.Find(product => product.companyId == company.id).ToList();
        }

        public static List<Product> GetAllFiltered(string filter)
        {
            return null;
        }

        public static void Delete(Product toDelete, IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Product>("Products");
            collection.DeleteOneAsync(product => product.id == toDelete.id);
        }

        public static void Update(Product product,List<Image> images, List<ProductProps> items, IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Product>("Products");
            UpdateDefinition<Product> update = Builders<Product>.Update.Set("images", images);
            UpdateDefinition<Product> update2 = Builders<Product>.Update.Set("properties", items);
            UpdateDefinition<Product> update3 = Builders<Product>.Update.Set("reviews", product.reviews);
            collection.UpdateOneAsync(p=> p.id == product.id, update);
            collection.UpdateOneAsync(p=> p.id == product.id, update2);
            collection.UpdateOneAsync(p=> p.id == product.id, update3);
        }

    }
}
