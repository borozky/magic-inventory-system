using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_s3485376.Controllers
{
    // Base Controller
    abstract class Controller
    {
        public int SelectOption(string[] menus)
        {
            return SelectOption(menus, "Please enter a choice: ");
        }

        public int SelectOption(string[] menus, string message)
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

                if (choice > menus.Length || choice < 1)
                {
                    Console.WriteLine("Please enter number 1 to {0}", menus.Length);
                    continue;
                }

                break;
            }

            return choice;
        }
    }
}
