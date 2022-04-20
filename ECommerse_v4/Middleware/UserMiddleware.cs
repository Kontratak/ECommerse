using ECommerse_v4.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Middleware
{
    public class UserMiddleware
    {

        public static AppUser Get(Guid id,IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<AppUser>("Users");
            return collection.Find(user => user.Id == id).First();
        }

    }
}
