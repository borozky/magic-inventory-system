using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_s3485376.Models
{
    static class Menus
    {
        public static readonly string[] MAIN_MENU =
        { "Owner", "Franchise Owner", "Customer", "Quit" };

        public static readonly string[] OWNER_MENU =
        {
            "Display All Stock Requests",
            "Display Stock Requests (True/False)",
            "Display All Product Lines",
            "Return to Main Menu",
            "Exit"
        };
        public static readonly string[] FRANCHISE_OWNER_MENU =
        {
            "Display Inventory",
            "Display Inventory (Low/High)",
            "Add New Invetory Item",
            "Return to Main Menu",
            "Quit"
        };

        public static readonly string[] CUSTOMER_MENU =
        {
            "Display Products",
            "Display Workshops",
            "Return to Main Menu",
            "Exit"
        };

        public static readonly string[] STORE_LOCATIONS =
        {
            "CBD",
            "North",
            "South",
            "East",
            "West"
        };
    }
}
