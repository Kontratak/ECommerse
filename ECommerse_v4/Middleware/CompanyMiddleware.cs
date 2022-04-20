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
    public class CompanyMiddleware
    {

        public static void Add(string companyname,AppUser appUser, IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Company>("Companys");
            List<Guid> employees = new List<Guid>();
            employees.Add(appUser.Id);
            collection.InsertOneAsync(new Company { name = companyname,Employees = employees});
        }

        public static Company Get(Guid id, IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Company>("Companys");
            return collection.Find<Company>(company => company.id == id).First();
        }

        public static Company GetByCompanyEmployeeId(Guid id, IConfiguration configuration)
        {
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Company>("Companys");
            return collection.Find<Company>(company => company.Employees.Contains(id)).First();
        }

        public static List<AppUser> GetEmployees(Guid id, IConfiguration configuration)
        {
            List<AppUser> appUsers = new List<AppUser>();
            Company company = GetByCompanyEmployeeId(id,configuration);
            foreach(var emp in company.Employees)
            {
                appUsers.Add(UserMiddleware.Get(emp, configuration));
            }
            return appUsers;
        }



        public static void AddEmployee(AppUser appUserCompany,AppUser toAdd, IConfiguration configuration)
        {
            Company company = GetByCompanyEmployeeId(appUserCompany.Id,configuration);
            company.Employees.Add(toAdd.Id);
            FilterDefinition<Company> filter = new BsonDocument("_id", company.id);
            UpdateDefinition<Company> update = Builders<Company>.Update.Set("Employees", company.Employees );
            var db = DatabaseMiddleware.GetDb(configuration);
            var collection = db.GetCollection<Company>("Companys");
            collection.FindOneAndUpdateAsync(filter, update);
        }
    }
}
