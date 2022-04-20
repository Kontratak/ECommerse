using ECommerse_v4.Middleware;
using ECommerse_v4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Controllers
{
    public class CategoryController : Controller
    {


        private IConfiguration Configuration { get; }

        public CategoryController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        public IActionResult CreateCategory()
        {
            return View();
        }



        [HttpPost]
        public void addCategory(string properties,string isMainCategory,string subCategorys)
        {

            List<CategoryProps> items = JsonConvert.DeserializeObject<List<CategoryProps>>(properties);
            List<string> subtoCats = JsonConvert.DeserializeObject<List<string>>(subCategorys);
            List<Guid> subIds = new List<Guid>();
            foreach (var item in subtoCats)
                subIds.Add(new Guid(item));
            Category category = new Category
            {
                properties = items,
                isMainCategory = isMainCategory == "true" ? true : false,
                subCategorys = subIds,
            };
            CategoryMiddleware.Add(category, Configuration);
        }


        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        [HttpGet]
        public string GetCategories()
        {
           List<Category> categories = CategoryMiddleware.GetAll(Configuration);
           return JsonConvert.SerializeObject(categories);
        }

        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        [HttpGet]
        public string GetCategory(string id)
        {
            Guid objectId = new Guid(id);
           Category category = CategoryMiddleware.Get(objectId,Configuration);
           return JsonConvert.SerializeObject(category);
        }

        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        [HttpGet]
        public string GetSubCategorys(string ids)
        {
            List<string> toConvertIds = JsonConvert.DeserializeObject<List<string>>(ids);
            List<Guid> objectids = new List<Guid>();
            foreach (var item in toConvertIds)
                objectids.Add(new Guid(item));
           List<Category> categories = CategoryMiddleware.GetSubCategories(objectids, Configuration);
           return JsonConvert.SerializeObject(categories);
        }

    }
}
