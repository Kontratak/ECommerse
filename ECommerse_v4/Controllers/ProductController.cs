using ECommerse_v4.Middleware;
using ECommerse_v4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ECommerse_v4.Controllers
{
    public class ProductController : Controller
    {

        private IConfiguration Configuration { get; }
        private UserManager<AppUser> _userManager;
        private IHostingEnvironment hostingEnv;

        public ProductController(UserManager<AppUser> userManager, IConfiguration configuration, IHostingEnvironment env)
        {
            this._userManager = userManager;
            this.Configuration = configuration;
            this.hostingEnv = env;
        }

        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        public async Task<IActionResult> ListProducts()
        {
            List<ProductDetails> productDetails = new List<ProductDetails>();
            AppUser appUser = await _userManager.GetUserAsync(User);
            Company company = CompanyMiddleware.GetByCompanyEmployeeId(appUser.Id,Configuration);
            List<Product> products = ProductMiddleware.GetCompanyProducts(company, Configuration);
            foreach(var product in products)
            {
                Category category = CategoryMiddleware.Get(product.categoryId, Configuration);
                Category subCategory = CategoryMiddleware.Get(category.subCategorys.Where(sub => sub == product.subCategoryId).FirstOrDefault(), Configuration);
                ProductDetails details = new ProductDetails
                {
                    product = product,
                    company = company,
                    category = category,
                    subCategory = subCategory,
                };
                productDetails.Add(details);
            }
            return View(productDetails);
        }



        [Authorize(Roles = "Admin,Company,CompanyEmployee")]
        public IActionResult EditProduct(Guid id)
        {
            Product product = ProductMiddleware.Get(id, Configuration);
            return View(product);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            Product product = ProductMiddleware.Get(id,Configuration);
            Category category = CategoryMiddleware.Get(product.categoryId, Configuration);
            Company company = CompanyMiddleware.Get(product.companyId, Configuration);
            ProductDetails details = new ProductDetails
            {
                product = product,
                company = company,
                category = category,

            };
            AppUser user = await _userManager.GetUserAsync(User);
            if (user != null)
                ViewBag.isLoggedIn = true;
            else
                ViewBag.isLoggedIn = false;
            return View(details);
        }

        


        [HttpPost]
        public async Task addProduct(string properties, string categoryId, string subCategoryId,IList<IFormFile> files)
        {
            List<ProductProps> items = JsonConvert.DeserializeObject<List<ProductProps>>(properties);
            AppUser user = await _userManager.GetUserAsync(User);
            Company company = CompanyMiddleware.GetByCompanyEmployeeId(user.Id, Configuration);
            Guid catId = new Guid(categoryId);
            Guid subCatId = new Guid(subCategoryId);
            Product product = new Product
            {
                properties = items,
                companyId = company.id,
                EmployeeId = user.Id,
                categoryId = catId,
                subCategoryId = subCatId,
                reviews = new List<Review>()
            };
            Product insertedProduct = ProductMiddleware.Add(product, Configuration);
            List<Image> imagelist = new List<Image>();
            try
            {
                string mainImage = items.Where(i => i.Name == "MainImage").First().Value[0];
                foreach (IFormFile source in files)
                {
                    
                    if(source.FileName == mainImage) items.Where(i => i.Name == "MainImage").First().Value[0] = GetFilePath(insertedProduct.id + "_" + files.IndexOf(source) + this.GetExt(source.FileName));
                    string filename = insertedProduct.id + "_" +files.IndexOf(source) + this.GetExt(source.FileName);

                    filename = this.CyrptImage(filename);
                    using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename)))
                        await source.CopyToAsync(output);
                    imagelist.Add(new Image
                    {   path = this.GetFilePath(filename),
                        name = filename
                    });
                }
                ProductMiddleware.Update(insertedProduct, imagelist,items, Configuration);
            }
            catch (Exception ex)
            {
                ProductMiddleware.Delete(insertedProduct,Configuration);
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(string Description,Guid Id)
        {
            
            try
            {
                Product product = ProductMiddleware.Get(Id, Configuration);
                AppUser appUser = await _userManager.GetUserAsync(User);
                if(appUser != null)
                {
                    product.reviews.Add(new Review
                    {
                        Description = Description,
                        Time = DateTime.Now,
                        User = new User
                        {
                            Email = appUser.Email,
                            Name = appUser.UserName,
                        }
                    });
                    ProductMiddleware.Update(product, product.images, product.properties, Configuration);
                }
            }
            catch (Exception ex)
            {
                
            }
            return RedirectToAction("Details","Product",new { id = Id});
        }

        private string CyrptImage(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);
            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            return this.hostingEnv.WebRootPath + "\\Uploads\\Products\\" + filename;
        }
        private string GetFilePath(string filename)
        {
            return "\\Uploads\\Products\\" + filename;
        }
        
        private string GetExt(string filename)
        {
            return filename.Substring(filename.LastIndexOf("."),filename.Length - filename.LastIndexOf("."));
        }




    }
}
