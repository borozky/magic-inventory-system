using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;

namespace WDT_s3485376.Views
{
    // View component that displays inventory items not present 
    //at current store but present in owners inventory (Franchise Owner page)
    class NewInventoryItemTable
    {
        public NewInventoryItemTable(List<Product> excluded)
        {
            // print new items
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            foreach (Product p in excluded)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("ID", p.Id.ToString());
                row.Add("Product name", p.Name);
                row.Add("Stock Level", p.StockLevel.ToString());
                row.Add("Price", String.Format("{0:C2}", p.Price));
                table.Add(row);
            }

            Console.WriteLine("NEW ITEMS");
            TableHelper.PrintTable(table);
            Console.WriteLine();
        }
    }
}
