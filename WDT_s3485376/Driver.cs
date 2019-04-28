using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using WDT_s3485376.Controllers;
using WDT_s3485376.Models;
using WDT_s3485376.Views;

namespace WDT_s3485376
{
    class Driver
    {
        // I used MVC even though this is only a console-based project
        static void Main(string[] args)
        {
            new MainMenuController();
        }
    }
}
