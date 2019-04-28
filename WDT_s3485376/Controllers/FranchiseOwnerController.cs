using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;
using WDT_s3485376.Views;

namespace WDT_s3485376.Controllers
{
    // Controller that handles capabilities that a Franchise Owner can do
    class FranchiseOwnerController : Controller, IMenuController
    {
        // use static fields to save info throughout this controller
        public static string CurrentLocation = "";
        public static int Threshold = 0;
        public static readonly int DEFAULT_STOCK_FOR_NEW_INVENTORY = 10;


        // entry-point
        public FranchiseOwnerController()
        {
            if ( CurrentLocation == "")
                SelectStore();

            DisplayMenus();
        }
        

        // select stores given by the Menus model
        public void SelectStore()
        {
            Console.Clear();
            new StoreLocationMenuPage(Menus.STORE_LOCATIONS, CurrentLocation);

            int choice = SelectOption(Menus.STORE_LOCATIONS, "Please select a store: ");
            CurrentLocation = Menus.STORE_LOCATIONS[choice - 1];
        }


        // display options
        public void DisplayMenus()
        {
            Console.Clear();
            new FranchiseOwnerMenuPage(Menus.FRANCHISE_OWNER_MENU, CurrentLocation);
            int choice = SelectOption(Menus.FRANCHISE_OWNER_MENU);
            GoTo(choice);
        }


        // route to one of the functions
        // [1] Display Inventory
        // [2] Display Inventory (True/False)
        // [3] Add new inventory
        // [4] Return to main menu
        // [5] Exit
        public void GoTo(int choice)
        {
            switch (choice)
            {
                case 1:
                    Console.Clear();
                    DisplayAllInventory(RequestThreshold(), 2); // 2 means display everything
                    GoBack();
                    break;
                case 2:
                    Console.Clear();
                    int th = RequestThreshold();
                    int selected = SelectBoolean() ? 1 : 0; // 1 means display 'True', 0 means display 'false'
                    DisplayAllInventory(th, selected);
                    GoBack();
                    break;
                case 3:
                    AddNewInventoryItem();
                    GoBack();
                    break;
                case 4:
                    new MainMenuController();
                    break;
                case 5:
                    Environment.Exit(0);
                    return; // exit immediately
            }


        }


        // Function that 
        // [1] displays inventory
        // [2] displays inventory with True/False option
        public void DisplayAllInventory(int threshold, int boolOption)
        {

            // get store products, filter them
            Inventory inv = new Inventory(CurrentLocation);
            List<Product> inventory = inv.Products;
            switch (boolOption)
            {
                // false is selected, select those w/ stock lvl same or above threshold
                case 0:
                    inventory = (from i in inventory
                                 where i.StockLevel >= threshold
                                 select i).ToList<Product>();
                    break;
                // true is selected, select items that requires re-stocking
                case 1:
                    inventory = (from i in inventory
                          where i.StockLevel < threshold
                          select i).ToList<Product>();
                    break;
                
                // other values, don't filter
            }
            
            // if nothing to process, just exit
            if (inventory.Count() == 0)
            {
                Console.WriteLine("==============================================");
                Console.WriteLine("      There are no products to process        ");
                Console.WriteLine("==============================================");
                Console.WriteLine();
                return;
            }

            // View Component
            new InventoryTable(inventory, threshold);


            // select choice, if q is selected (which is 0), just exit
            int choice = SelectItem(inventory, "Enter request to process (press q to quit): ");
            if (choice == 0) return;


            Product product = (from i in inventory
                              where i.Id == choice
                              select i).First();
            
            // create a stock request
            StockRequest request = new StockRequest();
            request.ProductName = product.Name;
            request.Quantity = threshold - product.StockLevel;
            request.StoreName = CurrentLocation;


            // When user selects an item that doesn't have to be re-stocked
            // if user insist, request for 0 more stock, otherwise just exit
            if (product.StockLevel >= threshold)
            {
                Console.WriteLine("The product with ID of {0} does not have to be re-stocked", product.Id);
                Console.Write("Do you wish to continue anyway? ");

                if (SelectBoolean() == false)
                    return; // exit immediately
                
                // amount needed for restock is 0
                request.Quantity = 0;
            }

            // save
            if (request.Save())
                Console.WriteLine("Request has been dispatched");
            else
                Console.WriteLine("Request failed. Something has gone wrong");
            
        }

        
        // [3] Adds new inventory item
        public void AddNewInventoryItem()
        {
            Console.Clear();

            List<Product> ownerProducts = new Inventory("owners").Products;
            List<Product> products = new Inventory(CurrentLocation).Products;
            string[] productNames = (from p in products
                                     select p.Name.ToLower()).ToArray();
            List<Product> excluded = ( from item in ownerProducts
                                 where !productNames.Contains(item.Name.ToLower())
                                 select item ).ToList<Product>();

            // Display table of new items
            new NewInventoryItemTable(excluded);

            Console.WriteLine();

            // select item, if user presses 'q', choice will return 0 which cancels the selection
            int choice = SelectItem(excluded, "Please select ID to add (press q to cancel): ");
            if (choice == 0) { Console.Write("Selection cancelled. "); return; };
            
            Product selected = (from item in excluded
                         where item.Id == choice
                         select item).First();

            // default stock to be added is 10
            // if there are less than 10 remaining, get all the remaining stock (even if there's 0 stock remaining)
            int stockToAdd = selected.StockLevel < DEFAULT_STOCK_FOR_NEW_INVENTORY ? 
                selected.StockLevel : 
                DEFAULT_STOCK_FOR_NEW_INVENTORY;
            
            // reduce owner's product stock first
            selected.StockLevel -= stockToAdd;
            if (selected.Save("owners"))
            {
                // Create new inventory item for current store
                // product ID is automatically added
                Product newitem = new Product();
                newitem.Name = selected.Name;
                newitem.StockLevel = stockToAdd;
                newitem.Price = selected.Price;
                newitem.Save(CurrentLocation);
                
                Console.WriteLine("New product with name {0} ({1} item{2}) has been added to the inventory.", 
                    selected.Name, stockToAdd, stockToAdd != 1 ? "s" : "");
            }
            // revert stock back
            else
            {
                selected.StockLevel += stockToAdd;
                selected.Save("owners");
            }
            


        }


        // Request threshold
        private int RequestThreshold()
        {
            if (Threshold == 0)
            {
                int choice;
                while (true)
                {
                    Console.Write("Please enter number to threshold for re-stocking: ");
                    string input = Console.ReadLine();
                    bool validInt = int.TryParse(input, out choice);
                    if (!validInt)
                    {
                        Console.WriteLine("Please insert a number");
                        continue;
                    }

                    if (choice < 1)
                    {
                        Console.WriteLine("Please enter number 1 or more");
                        continue;
                    }

                    break;
                }
                Threshold = choice;
            }

            return Threshold;
        }


        // Allows use to select "true" or "false", yes or no
        private bool SelectBoolean()
        {
            string[] allowed = {
                "t", "T", "True", "true", "TRUE", "y", "Y", "Yes", "yes", "YES", // 0 - 9
                "f", "F", "False", "false", "FALSE", "n", "N", "No", "no", "NO"  // 10 - 19
            };

            bool result = true;
            
            while (true)
            {
                Console.Write("Please select true or false: ");
                string input = Console.ReadLine();
                bool validBoolean = allowed.Contains(input.ToString().Trim());
                if (!validBoolean)
                {
                    Console.WriteLine("Please insert true or false only");
                    continue;
                }

                // 9, 10, 11... is false
                if (Array.IndexOf(allowed, input.ToString().Trim()) > 9)
                    result = false;

                break;
            }

            return result;
        }


        // Selects an inventory. Offers the user to press 'q' to quit selection
        // This method also allows user to select an inventory even if that has 0 stock
        private int SelectItem( List<Product> inventory, 
            string message = "Enter request to process (press q to quit): ")
        {
            int choice;
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                // quit if user selects 'q'
                if (input == "q") return 0;

                bool validInt = int.TryParse(input, out choice);
                if (!validInt)
                {
                    Console.WriteLine("Please insert a number");
                    continue;
                }

                var res = from inv in inventory
                          where inv.Id == choice
                          select inv;
                
                if(res.Count() == 0)
                {
                    Console.WriteLine("Item with id {0} doesn't exist", input);
                    continue;
                }

                choice = res.First().Id;
                break;
            }

            return choice;
        }


        // Go back to Franchise Owner menu
        private void GoBack()
        {
            Console.Write("Press any key to go back");
            Console.ReadKey();
            DisplayMenus();
        }



    }
}
