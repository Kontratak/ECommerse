using ECommerse_v4.Settings;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Middleware
{
    public class DatabaseMiddleware
    {

        public static IMongoDatabase GetDb(IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
            var Client = new MongoClient(mongoDbSettings.ConnectionString);
            return Client.GetDatabase("ECommerse");
        }

    }
}
