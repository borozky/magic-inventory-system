using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;

namespace WDT_s3485376.Views
{
    // View component for displaying owner's inventory
    class ProductLinesTable
    {
        public ProductLinesTable(List<Product> inventory)
        {
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            foreach (Product item in inventory)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("ID", item.Id.ToString());
                row.Add("Name", item.Name);
                row.Add("Stock Level", item.StockLevel.ToString());
                row.Add("Unit Price", String.Format("{0:C2}", item.Price));
                table.Add(row);
            }
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("INVENTORY");
            TableHelper.PrintTable(table);
            Console.WriteLine();
        }
    }
}
