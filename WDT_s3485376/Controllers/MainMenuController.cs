using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;
using WDT_s3485376.Views;

namespace WDT_s3485376.Controllers
{
    class MainMenuController : Controller, IMenuController
    {
        public MainMenuController()
        {
            // Default action
            DisplayMenus();
        }

        public void DisplayMenus()
        {
            Console.Clear();

            // main menu page
            new MainMenuPage(Menus.MAIN_MENU);

            int choice = SelectOption(Menus.MAIN_MENU);
            GoTo(choice);
        }

        public void GoTo(int choice)
        {
            switch (choice)
            {
                case 1:
                    new OwnerController();
                    break;
                case 2:
                    new FranchiseOwnerController();
                    break;
                case 3:
                    new CustomerController();
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
            }
        }
    }
}
