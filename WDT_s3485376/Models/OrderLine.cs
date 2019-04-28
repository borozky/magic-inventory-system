using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_s3485376.Models
{
    // Blueprint for items in your shopping cart.
    class OrderLine
    {
        // Fields for JSON.NET to populate
        public string ProductName;
        public double UnitPrice;
        public int Qty;


        // The shopping cart.
        static List<OrderLine> All = new List<OrderLine>();


        // New orderline
        public OrderLine(string productName, double unitprice, int qty)
        {
            ProductName = productName;
            UnitPrice = unitprice;
            Qty = qty;
        }


        // gets all items in the shopping cart
        public static List<OrderLine> all()
        {
            return All;
        }


        // find an item in your shopping cart
        public static OrderLine find(string productName)
        {
            var result = from i in All
                         where i.ProductName == productName
                         select i;
            if (result.Count() > 0)
                return result.First();

            return null;
        }


        // Check if an item in the shopping cart exists
        public static bool Exists(string productName)
        {
            return find(productName) != null;
        }


        // clears the shopping cart
        public static void clearAll()
        {
            All = new List<OrderLine>();
        }


        // adds an item in the shopping cart
        public static void add(string name, double price, int qty)
        {
            if (!Exists(name))
            {
                All.Add(new OrderLine(name, price, qty));
            }
            else
            {
                OrderLine found = (from item in All where item.ProductName == name select item).First();
                found.Qty += qty;
            }
        }


        // Calculates the subprice
        public double SubPrice()
        {
            return Qty * UnitPrice;
        }


    }

}
