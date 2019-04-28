using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_s3485376.Models
{
    class Inventory
    {
        public string Location { get; }
        public List<Product> Products { get; }

        public Inventory(string location)
        {
            Location = location;
            Products = Product.All(Location);
        }
    }
}
