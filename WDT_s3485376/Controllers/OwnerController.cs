using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;
using WDT_s3485376.Views;

namespace WDT_s3485376.Controllers
{
    class OwnerController : Controller, IMenuController
    {

        // entry point
        public OwnerController()
        {
            DisplayMenus();
        }


        // display menus
        public void DisplayMenus()
        {
            Console.Clear();
            new OwnersMenuPage(Menus.OWNER_MENU);
            int choice = SelectOption(Menus.OWNER_MENU);

            GoTo(choice);
        }


        // Route to one of the actions
        // [1] - Display Stock Requests
        // [2] - Display Stock Requests (True/False)
        // [3] - Display all product lines
        // [4] - Return to main menu
        // [5] - Exit
        public void GoTo(int choice)
        {
            switch (choice)
            {
                case 1:
                    DisplayAllStockRequests(null);
                    GoBack();
                    break;
                case 2:
                    DisplayAllStockRequests(SelectBoolean("Please select true or false: "));
                    GoBack();
                    break;
                case 3:
                    DisplayAllProductLines();
                    GoBack();
                    break;
                case 4:
                    new MainMenuController().DisplayMenus();
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
            }
        }

        public void DisplayAllStockRequests(bool? trueOrFalse = null)
        {

            // Fetch data from file
            Console.Clear();
            Console.WriteLine("Getting all stock requests. Please wait...");
            List<StockRequest> stockRequests = StockRequest.All();
            Console.Clear();
            
            List<Product> ownerInventory = new Inventory("owners").Products;

            if (ownerInventory.Count() == 0)
                return;

            // if user select a true or false option, filter all requests
            if (trueOrFalse != null)
            {
                if (trueOrFalse == true)
                {
                    var set = from req in stockRequests
                              where req.Quantity <= (from i in ownerInventory
                                                     where i.Name.ToLower() == req.ProductName.ToLower()
                                                     select i).First().StockLevel
                              select req;

                    stockRequests = set != null ? set.ToList() : new List<StockRequest>();
                } else
                {
                    var set = from req in stockRequests
                              where req.Quantity > (from i in ownerInventory
                                                     where i.Name.ToLower() == req.ProductName.ToLower()
                                                     select i).First().StockLevel
                              select req;

                    stockRequests = set != null ? set.ToList() : new List<StockRequest>();
                }
            }

            // no stock request
            if (stockRequests.Count() == 0)
            {
                Console.WriteLine();
                Console.WriteLine("=========================================");
                Console.WriteLine("    There are no request to process      ");
                Console.WriteLine("=========================================");
                Console.WriteLine();

                return;
            }

            // print the stock requests table
            new StockRequestsTable(stockRequests, ownerInventory);



            // choose a request to process
            int choice = SelectItem(stockRequests, "Please select a request to process: ");

            StockRequest selected = (from req in stockRequests
                                     where req.Id == choice
                                     select req).First();


            // check if request can be re-stocked. If not exit immediately
            Product productToCheck = (from p in ownerInventory
                                      where p.Name == selected.ProductName
                                      select p).First();

            if (selected.Quantity > productToCheck.StockLevel)
            {
                Console.WriteLine("You don't have the stock to fulfill this request");
                return;
            }
            
            // re-stock the product
            List < Product > products = new Inventory(selected.StoreName).Products;

            var resultset = from prod in products
                            where prod.Name.ToLower() == selected.ProductName.ToLower()
                            select prod;

            Product found = (from prod in products
                             where prod.Name.ToLower() == selected.ProductName.ToLower()
                             select prod).First();

            // update product stock level
            found.StockLevel += selected.Quantity;

            // save product info and delete request
            if (found.Save(selected.StoreName))
                if (selected.Delete())
                    Console.WriteLine("Request has been fulfulled");

        }

        public bool SelectBoolean(string message)
        {

            string[] allowed = {
                "t", "T", "True", "true", "TRUE",   // 0-4
                "f", "F", "False", "false", "FALSE" // 5-9
            };

            bool result = true;

            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                bool validBoolean = allowed.Contains(input.ToString().Trim());
                if (!validBoolean)
                {
                    Console.WriteLine("Please insert true or false only");
                    continue;
                }

                // 5,6,7,8,9 is false
                if (Array.IndexOf(allowed, input.ToString().Trim()) > 4)
                    result = false;

                break;
            }

            return result;
        
        }


        public void DisplayAllProductLines()
        {
            List<Product> inventory = new Inventory("owners").Products;
            new ProductLinesTable(inventory);
        }

        private int SelectItem(List<StockRequest> stockrequests, string message)
        {
            int choice;
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                bool validInt = int.TryParse(input, out choice);
                if (!validInt)
                {
                    Console.WriteLine("Please insert a number");
                    continue;
                }

                var res = from inv in stockrequests
                          where inv.Id == choice
                          select inv;


                if (res.Count() == 0)
                {
                    Console.WriteLine("Item with id {0} doesn't exist", input);
                    continue;
                }

                choice = res.First().Id;
                break;
            }

            return choice;
        }

        private void GoBack()
        {
            Console.Write("Press any key to go back");
            Console.ReadKey();
            DisplayMenus();
        }


    }
}
