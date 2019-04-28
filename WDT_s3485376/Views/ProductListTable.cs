using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;

namespace WDT_s3485376.Views
{
    // View component that displays list of available products for purchase
    // Used by CustomersController
    class ProductListTable
    {
        // fields for pagination
        public static int CurrentPage = 0;
        public static int NumPages = 0;
        public static int ItemsPerPage = 5;

        public List<Dictionary<string, string>> table;

        public ProductListTable(List<Product> products)
        {
            // set this up for printing tables of data
            table = new List<Dictionary<string, string>>();
            
            // pagination setup, start with page 0
            int start = 0 + (CurrentPage * ItemsPerPage);
            int end = start + ItemsPerPage - 1;
            NumPages = (int) Math.Ceiling( (decimal) products.Count() / ItemsPerPage);

            for (int i = start; i <= end; i++)
            {
                Product product;

                // this will certainly throw IndexOutOfRangeException when there are less than 5 items 
                // if it happens, just simply break the loop
                try { product = products.ElementAt(i); }
                catch (Exception e) { break; }

                // add cells to rows, then add them to table
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("ID", product.Id.ToString());
                row.Add("Product", product.Name);
                row.Add("Current Stock", product.StockLevel.ToString());
                row.Add("Unit Price", String.Format("{0:C2}", product.Price));
                table.Add(row);
            }



            Console.WriteLine("INVENTORY ({0} of {1})", CurrentPage + 1, NumPages);
            TableHelper.PrintTable(table);
            if (products.Count() - start < ItemsPerPage)
                for (int i = 0; i < ItemsPerPage - (products.Count() - start); i++)
                    Console.WriteLine();

            Console.WriteLine();
        }
    }
}
