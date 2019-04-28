using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;
using WDT_s3485376.Controllers;
using System.Collections;

namespace WDT_s3485376.Views
{

    // Display list of inventory items (Franchise Owner page)
    class InventoryTable
    {

        public InventoryTable(List<Product> products, int threshold)
        {
            // prepare inventory for printing
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            foreach (Product p in products)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("ID", p.Id.ToString());
                row.Add("Name", p.Name);
                row.Add("Current Stock", p.StockLevel.ToString());
                row.Add("Re-Stock", (p.StockLevel < threshold).ToString());
                table.Add(row);
            }

            Console.WriteLine("INVENTORY");
            TableHelper.PrintTable(table);
            Console.WriteLine();
            Console.WriteLine();
        }

        
    }
}
