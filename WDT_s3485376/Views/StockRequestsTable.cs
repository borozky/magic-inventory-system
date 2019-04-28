using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;

namespace WDT_s3485376.Views
{
    // View component that displays stock requests
    class StockRequestsTable
    {
        public StockRequestsTable(List<StockRequest> requests, List<Product> ownerInventory)
        {

            // create list of key-value pair, we'll pass it to a helper function 'PrintTable()'
            // see the last lines of code
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            foreach (StockRequest sr in requests)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();



                // use ownerInventory items decide 
                // whether or not we'll refill the stores inventory
                int currentstock = (from inv in ownerInventory
                                    where inv.Name.ToLower() == sr.ProductName.ToLower()
                                    select inv).First().StockLevel;

                row.Add("ID", sr.Id.ToString());
                row.Add("Store", sr.StoreName);
                row.Add("Product", sr.ProductName);
                row.Add("Quantity", sr.Quantity.ToString());
                row.Add("Stock Level", currentstock.ToString());
                row.Add("Stock Availability", (sr.Quantity <= currentstock).ToString());

                table.Add(row);
            }

            Console.WriteLine("STOCK REQUESTS");
            Console.WriteLine();

            // we'll use a helper called 'TableHelper' to help me print nice tables
            // TableHelper is located inside the Helpers folder
            TableHelper.PrintTable(table);

            Console.WriteLine();
            Console.WriteLine();


        }
    }
}
