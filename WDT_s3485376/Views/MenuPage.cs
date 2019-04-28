using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_s3485376.Views
{
    // Parent class for all view components that displays multiple menus.
    // Menus are provided from Menus model (see Models/Menus.cs)
    abstract class MenuPage
    {

        // Constructor (overloaded)
        public MenuPage(string[] menus): this(menus, ""){}
        public MenuPage(string[] menus, string storeName)
        {
            DisplayHeader(storeName);
            DisplayMenus(menus);
        }


        // Displays the header
        public void DisplayHeader(string storeName)
        {
            string additionalInfo = storeName == "" ? "" : String.Format(" ({0})", storeName);
            Console.WriteLine("Welcome To Marvellous Magic{0}", additionalInfo);
            Console.WriteLine("==============================");
        }


        // Displays list of available menus
        public void DisplayMenus(string[] menus)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                Console.WriteLine();
                Console.WriteLine("\t{0}. {1}", i + 1, menus[i]);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
