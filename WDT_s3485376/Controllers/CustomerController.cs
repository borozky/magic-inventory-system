using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Views;
using WDT_s3485376.Models;

namespace WDT_s3485376.Controllers
{
    // Controller that handles functionalities that a 'Customer' can do
    // NOTE: this controller is fairly complex
    class CustomerController : Controller, IMenuController
    {

        // save current location here. 
        // This will always be cleared everytime user goes back to main menu
        public static string CurrentLocation = "";


        // Fields to keep track selected item and qty for other methods to use
        int SelectedItem = -1;
        int SelectedQty = -1;


        // Fields to handle functionalities like Next Page, Previous Page, Return to Menu, etc...
        // NOTE: The program will show only functionalities that are needed and make sense
        //       For example on pagination, if customer is on the first page, the "Previous Page" is not available
        //       These fields are manipulated from AssignAvailableFunctions()
        string SelectedFunction;
        List<string> AvailableFunctions = new List<string>();
        List<string> AvailableFunctionChars = new List<string>();


        // View components are saved here
        ProductListTable ProductListPage;
        ShoppingCartTable OrderBasketPage;

        
        // This is for those "available functionalities" fields above
        // In "Workshop" page, there's no "Next Page" or "Previous Page" functionality
        bool CurrentlyAtWorkshop = false;
        

        // save Workshop information here
        static Workshop Workshop;


        // entry point
        public CustomerController()
        {
            if (CurrentLocation == "")
                SelectStore();

            DisplayMenus();
        }


        // Displays menus stored from Menus model (see Models/Menus.cs)
        public void DisplayMenus()
        {
            Console.Clear();
            new CustomerMenuPage(Menus.CUSTOMER_MENU, CurrentLocation);

            int choice = SelectOption(Menus.CUSTOMER_MENU);

            GoTo(choice);
        }


        // Router that send you to one of the 'Customer' actions
        // [1] Display Products    - Place where you can place your order
        // [2] Display Workshops   - Place where you can book into one of the 
        //                             10 workshops (5 stores, morning and afternoon) 
        // [3] Return to Main Menu - All of the pending orders are deleted once you go back to main menu
        // [4] Exit
        public void GoTo(int choice)
        {
            switch (choice)
            {
                case 1:
                    DisplayProducts();
                    GoBack();
                    break;
                case 2:
                    CurrentlyAtWorkshop = true;
                    DisplayWorkshops();
                    GoBack();
                    CurrentlyAtWorkshop = false;
                    break;
                case 3:
                    OrderLine.clearAll();
                    CurrentLocation = "";
                    SelectedFunction = null;
                    SelectedItem = -1;
                    SelectedQty = -1;
                    CurrentlyAtWorkshop = false;
                    new MainMenuController();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }
        }

        
        // Selects a store. Names of the stores are stored in Menus model (Model/Menus.cs)
        public void SelectStore()
        {
            Console.Clear();
            new StoreLocationMenuPage(Menus.STORE_LOCATIONS, CurrentLocation);

            int choice = SelectOption(Menus.STORE_LOCATIONS, "Please select a store: ");
            Console.WriteLine("Chosen Menu: {0}", choice.ToString());

            CurrentLocation = Menus.STORE_LOCATIONS[choice - 1];
        }


        // [1] Display Products
        // This method will also ask you select an item for purchase
        public void DisplayProducts()
        {
            CurrentlyAtWorkshop = false;
            Inventory inventory = new Inventory(CurrentLocation);

            List<Product> products = new List<Product>();
            foreach (Product p in inventory.Products)
                if (p.StockLevel > 0)
                    products.Add(p);


            Console.Clear();

            // if no products in the inventory, print this message, then exit
            if (products.Count() == 0)
            {
                Console.WriteLine("=============================================================");
                Console.WriteLine("   There are no items to buy or all products are out stock   ");
                Console.WriteLine("=============================================================");
                Console.WriteLine();

                return;
            }
            // display product list
            // instance vars from this view will be used by other methods
            ProductListPage = new ProductListTable(products);
            

            DisplayOrders();

            AssignAvailableFunctions();
            DisplayAvailableFunctions();


            SelectItem(products);

            ProcessSelectedFunction();

        }


        // Displays your shopping cart
        private void DisplayOrders()
        {
            OrderBasketPage = new ShoppingCartTable(OrderLine.all(), Workshop);
        }


        // Displays available functions you can use
        // Functionaties are generated from AssignAvailableFunctions()
        private void DisplayAvailableFunctions()
        {
            string legend = "[ Legend: " + String.Join(" | ", AvailableFunctions) + " ]";
            Console.WriteLine();
            Console.WriteLine(legend);
            Console.WriteLine();
        }


        // Allow certain functions to execute depending on there situations:
        // Next Page            - display this if current page is not the last page
        // Previous Page        - Display this if current page is not the first page
        // Return to Menu       -     
        // Complete transaction - Display this if there are items in your shopping cart
        private void ProcessSelectedFunction()
        {
            string[] availFuncArray = AvailableFunctionChars.ToArray();
            if (availFuncArray.Contains(SelectedFunction))
            {
                switch (SelectedFunction)
                {
                    // next page
                    case "N":
                        if (availFuncArray.Contains(SelectedFunction) == false)
                            break;
                        ProductListTable.CurrentPage++;
                        DisplayProducts();
                        break;


                    // previous page
                    case "P":
                        if (availFuncArray.Contains(SelectedFunction) == false)
                            break;
                        if (ProductListTable.CurrentPage > 0)
                            ProductListTable.CurrentPage--;
                        DisplayProducts();
                        break;

                    // Return to customer menu page. 
                    // Basket and current location is preserved until user goes back to the main menu
                    case "R":
                        new CustomerController();
                        DisplayProducts();
                        break;
                    
                    // Completes the transaction
                    case "C":
                        CompleteTransaction();
                        break;

                }
            }

            // when valid item number and quantity is selected instead...
            else if (SelectedItem > 0 && SelectedQty > 0)
            {
                DisplayProducts();

            }
        }


        // Checkout and completes transaction
        private void CompleteTransaction()
        {

            Console.Clear();
            Console.WriteLine("Processing order...");
            try
            {
                List<OrderLine> OrderLines = OrderLine.all(); // may take a while
                Orders order = new Orders(OrderLines, CurrentLocation, Workshop != null ? 0.1 : 0);
                Booking booking = null;

                long ReferenceNumber = 0;

                // save an order
                if ( ! order.Save())
                    throw new Exception("Order has failed to dispatch");

                // if a workshop is selected, book into it
                if (Workshop != null)
                {
                    booking = new Booking((int)order.ID,
                        Workshop.Location,
                        Workshop.Type == "MORNING" ? Booking.BookingType.MORNING : Booking.BookingType.AFTERNOON);

                    if ( ! booking.Save() )
                        throw new Exception("Failed to book");

                    ReferenceNumber = booking.ReferenceNumber < 1 ? booking.ReferenceNumber : 0 ;
                }
                
                // Display the "Receipt" with reference number
                new TransactionSummaryTable(order, booking);
                
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } finally
            {
                // IMPORTANT: Reset after successful/or failed order
                OrderLine.clearAll();
                SelectedItem = -1;
                SelectedQty = -1;
                SelectedFunction = null;
                Workshop = null;
            }
        }

        // Method that assigns functionalities
        // Functionalities are mentioned on ProcessSelectedFunction()
        private void AssignAvailableFunctions()
        {
            AvailableFunctions = new List<string>();
            AvailableFunctionChars = new List<string>();

            // If currently at "Display Products" page...
            if ( ! CurrentlyAtWorkshop)
            {
                // Functionalities for pagination 'P' and 'N'
                if (ProductListTable.CurrentPage > 0)
                {
                    AvailableFunctions.Add("'P' Previous Page");
                    AvailableFunctionChars.Add("P");
                }

                if (ProductListTable.CurrentPage + 1 < ProductListTable.NumPages)
                {
                    AvailableFunctions.Add("'N' Next Page");
                    AvailableFunctionChars.Add("N");
                }
            }

            
            AvailableFunctions.Add("'R' Return to Menu");
            AvailableFunctionChars.Add("R");

            // If there are orders, 'C' is available
            if (OrderLine.all().Count() > 0)
            {
                AvailableFunctions.Add("'C' Complete Transaction");
                AvailableFunctionChars.Add("C");
            }
        }

        // Selects an item. 
        // Products can be selected even if they are hidden, as long as they exist
        // This also allows selection of functionalities provided by List<string> AvailableFunctions field
        public void SelectItem(List<Product> products)
        {
            while (true)
            {
                Console.Write("Enter Item Number to purchase or function: ");
                string input = Console.ReadLine();
                input = input.Trim();
                bool validInt = int.TryParse(input, out SelectedItem);

                // invalid int, check if it is a one-letter function
                if (!validInt)
                {
                    // NOTE: AvailableFunctionChars were processed via AssignAvailableFunctions() method
                    string[] availOptions = AvailableFunctionChars.ToArray();
                    if (availOptions.Contains(input.ToUpper()))
                    {
                        SelectedFunction = input.ToUpper();
                        break;
                    }

                    Console.WriteLine("Invalid item number or function");
                    continue;

                }
                else
                {
                    // User selects an item. If item doesn't exists, ask again
                    Product p;
                    try
                    {
                        p = (from prod in products where prod.Id == SelectedItem select prod).First();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Product with ID {0} cannot be found.", input);
                        continue;
                    }


                    // This limits the user from purchasing too much
                    if (OrderLine.all().Count() > 0)
                    {
                        var notAvailAnymoreForPurchase = from ord in OrderLine.all()
                                                         where ord.ProductName == p.Name && ord.Qty == p.StockLevel
                                                         select ord;

                        if (notAvailAnymoreForPurchase.Count() > 0)
                        {
                            Console.WriteLine("This item stock level is all taken out. Please select another item");
                            continue;
                        }

                    }

                    // check for quantity
                    SelectedQty = SelectQuantity(p);

                    // add the order
                    AddOrder(p, SelectedQty);

                }

                break;
            }
        }


        // Ask user for quantity of the item they want to buy
        public int SelectQuantity(Product p)
        {

            int choice = -1;
            while (true)
            {
                Console.Write("Please enter quantity: ");
                string quantity = Console.ReadLine();
                quantity.Trim();

                bool validQty = int.TryParse(quantity, out choice);

                // invalid quantity
                if (!validQty)
                {
                    Console.WriteLine("Invalid quantity!");
                    continue;
                }

                // remaining qty
                var itemInOrder = from ord in OrderLine.all()
                                  where ord.ProductName == p.Name
                                  select ord;
                int remainingQty = 0;
                if (itemInOrder.Count() > 0)
                {
                    remainingQty = p.StockLevel - itemInOrder.First().Qty;
                    if (choice > remainingQty)
                    {
                        Console.WriteLine("Insufficient stock level, quantity must be between 1 to {0}", remainingQty);
                        continue;
                    }
                }

                // quantity is out of range
                if (choice < 1 || choice > p.StockLevel)
                {
                    Console.WriteLine("Quantity is out of range.");
                    continue;
                }

                break;
            }

            return choice;
        }

        // Selects a workshop. MORNING AND AFTERNOON workshops only
        // This offers user to press 'q' to quit
        // Return:
        // null  - user selects to either 'Return to Menu' or 'Complete Transaction'
        // 0     - user presses 'q'
        // any   - user select a workshop of similar ID 
        public int? SelectWorkshop(List<Workshop> workshops)
        {
            int choice;

            while (true)
            {
                Console.Write("Please select a workshop (select 'q' to quit): ");
                string input = Console.ReadLine();

                if (input == "q") return 0;

                bool validInt = int.TryParse(input, out choice);
                if (!validInt)
                {
                    // NOTE: AvailableFunctionChars were processed via AssignAvailableFunctions() method
                    string[] availOptions = AvailableFunctionChars.ToArray();
                    if (availOptions.Contains(input.ToUpper()))
                    {
                        SelectedFunction = input.ToUpper();
                        return null;
                    }

                    Console.WriteLine("Please insert a number");
                    continue;
                }

                // Display workshop that is the table
                var res = from workshop in workshops
                          where workshop.ID == choice
                          select workshop;

                if (res.Count() == 0)
                {
                    Console.WriteLine("Workshop with id {0} is not in the table", input);
                    continue;
                }

                // Do not select unavailable workshops
                if ( res.First().SpacesAvailable() <= 0 )
                {
                    Console.WriteLine("This workshop is not available. Please select another one");
                    continue;
                }


                choice = res.First().ID;
                break;
            }

            return choice;
        }


        // Adds an order
        private void AddOrder(Product p, int quantity)
        {
            OrderLine.add(p.Name, p.Price, quantity);
        }
        

        private bool SelectBoolean(string message)
        {
            string[] allowed = {
                "y", "Y", "Yes", "yes", "YES",
                "n", "n", "No", "no", "NO"
            };

            bool result = true;

            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                bool validBoolean = allowed.Contains(input.ToString().Trim());
                if (!validBoolean)
                {
                    // if there's an invalid selection, just return and say no
                    Console.WriteLine("Please select yes or no only");
                    return false;
                }

                // 4,5,6,7 is false
                if (Array.IndexOf(allowed, input.ToString().Trim()) > 4)
                    result = false;

                break;
            }

            Console.WriteLine("{0} selected", result.ToString());

            return result;
        }


        // Displays workshop in a store
        // This will also displays the shopping cart and...
        // will not allow user to book into a workshop if cart empty
        public void DisplayWorkshops()
        {
            Console.Clear();

            List<Workshop> workshops = Workshop.All(CurrentLocation);
            
            // Display list of workshops
            new WorkshopsTable(workshops);


            // Display list of all orders
            if (OrderLine.all().Count() > 0){
                DisplayOrders();
            }

            // Alert the user that shopping cart is filled
            Console.WriteLine();
            Console.WriteLine("You have a pending order");
            if (Workshop != null)
            {
                Console.WriteLine("ALERT: You have selected to book to {0} {1} workshop", Workshop.Location, Workshop.Type);
                Console.WriteLine("Press 'C' process your order and booking or book to another workshop");
                Console.WriteLine();
            }


            // Assign and display available functions 
            AssignAvailableFunctions();
            DisplayAvailableFunctions();


            // if there are no orders, exit immediately
            if (OrderLine.all().Count() == 0)
            {
                Console.WriteLine("WARNING: You cannot book into workshop unless you create an order");
                return;
            }


            // select a workshop, if q is selected (which is 0), quit immediately
            int? choice = SelectWorkshop(workshops);

            if (choice == null)
            {
                ProcessSelectedFunction();
                return;
            }
            else if (choice == 0)
            {
                return;
            }
            else
            {
                Workshop = Workshop.Find((int) choice);
                DisplayWorkshops();
            }

        }

        // Prompts the user to go back
        private void GoBack()
        {
            Console.Write("Press any key to go back");
            Console.ReadKey();
            DisplayMenus();
        }
        
    }
}
