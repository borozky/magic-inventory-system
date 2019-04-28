using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
using WDT_s3485376.Helpers;

namespace WDT_s3485376.Models
{
    class Orders
    {
        // Fields for JSON.NET to populate
        public int? ID;
        public string Location;
        public double DiscountRate = 0;
        public double DiscountPrice = 0;
        public double SubTotal = 0.0;
        public double GrandTotal = 0.0;
        public List<OrderLine> orderLines = new List<OrderLine>();


        // All orders are cached here
        private static List<Orders> AllOrders = Retrieve();


        // Create a new order
        public Orders(List<OrderLine> orderlines, string location, double discount)
        {
            try
            {
                if (orderlines.Count() == 0)
                    throw new ArgumentException("Orders must not be empty");
                if (discount >= 1)
                    throw new ArgumentException("Discount cannot be greater than or equal to 1");
                if (!Menus.STORE_LOCATIONS.Contains(location))
                    throw new ArgumentException("Invalid Store Location");

                orderLines = orderlines;
                Location = location;
                DiscountRate = discount;

                SubTotal =  CalculateSubTotal(orderlines);
                DiscountPrice = CalculateDiscount(SubTotal, discount);
                GrandTotal = CalculateGrandTotal(SubTotal, DiscountPrice);

            }
            // Illegal arguments
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            // other exceptions
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        // Calculate subtotal
        private double CalculateSubTotal(List<OrderLine> lines)
        {
            double s = 0.0;
            foreach (OrderLine line in lines)
                s += line.SubPrice();
            return s;
        }


        // Calculate discount price
        private double CalculateDiscount(double price, double rate)
        {
            return price * rate;
        }


        // Calculate grand total
        private double CalculateGrandTotal(double subtotal, double discount)
        {
            return subtotal - discount;
        }


        // Gets all orders
        public static List<Orders> All()
        {
            return AllOrders.Count() > 0 ? AllOrders: Retrieve();
        }


        // Check if order exists
        public static bool Exists(int ID)
        {
            return Get(ID) != null ? true : false;
        }


        // Find an order by ID
        public static Orders Get(int ID)
        {
            var result = from item in AllOrders
                         where item.ID == ID
                         select item;

            return result.Count() == 0 ? null : result.First();
        }

        // Retrieves all orders from the file
        private static List<Orders> Retrieve()
        {
            try
            {
                string path = Path.Combine(PathHelper.BASEPATH, @"Resources/Orders.txt");
                string contents = File.ReadAllText(path);
                List<Orders> r = JsonConvert.DeserializeObject<List<Orders>>(contents);
                return r;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        
        
        // Save order transaction
        // Note: Placed orders cannot be modified, so we will only use Add()
        public bool Save()
        {
            List<Orders> savedOrders = Retrieve();

            // generate ID
            if (ID == null)
            {
                var latest = from item in savedOrders
                             orderby item.ID descending
                             select item;
                if (latest == null || latest.Count() == 0)
                {
                    ID = 1;
                } else
                {
                    ID = latest.First().ID + 1;
                }
            }

            savedOrders.Add(this);

            // Saving could go wrong. In that case, save has failed and return false
            try
            {
                string path = Path.Combine(PathHelper.BASEPATH, @"Resources/Orders.txt");
                string json = JsonConvert.SerializeObject(savedOrders, Formatting.Indented);
                File.WriteAllText(path, json);


                // reduce stock
                foreach (OrderLine line in orderLines)
                {
                    Product p = Product.Find(line.ProductName, Location);
                    p.StockLevel -= line.Qty;
                    p.Save(Location);
                }

                // empty cart
                AllOrders = new List<Orders>();
                OrderLine.clearAll();

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        

        



    }

    
}
