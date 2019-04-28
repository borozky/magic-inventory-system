using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;

namespace WDT_s3485376.Views
{
    // View component that displays list of shopping cart items
    // Used from Customers page
    class ShoppingCartTable
    {
        public ShoppingCartTable(List<OrderLine> OrderLines, Workshop workshop = null)
        {
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            
            Console.WriteLine("YOUR ORDER BASKET");
            if (OrderLines == null || OrderLines.Count() == 0)
            {
                Console.WriteLine("Basket is empty");
            } else
            {
                double GrandTotal = 0.0;
                foreach (OrderLine line in OrderLines)
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    row.Add("Product Name", line.ProductName);
                    row.Add("Quantity", line.Qty.ToString());
                    row.Add("Unit Price", String.Format("{0:C2}", line.UnitPrice));
                    row.Add("Total Price", String.Format("{0:C2}", line.UnitPrice * line.Qty));

                    GrandTotal += line.UnitPrice * line.Qty;

                    table.Add(row);
                }


                TableHelper.PrintTable(table);
                Console.WriteLine();
                Console.WriteLine(string.Format("Grand Total: {0:C2}", GrandTotal));
                if (workshop != null)
                    Console.WriteLine("You have choose to bookinto {0} {1} workshop", workshop.Location, workshop.Type);
                Console.WriteLine();
                Console.WriteLine();
                

            }

        }

    }
}
