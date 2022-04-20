using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerse_v4.Models
{
    public class ProductDetails
    {

        public Product product { get; set; }
        public Company company { get; set; }
        public Category category { get; set; }
        public Category subCategory { get; set; }

    }
}
