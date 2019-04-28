using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_s3485376.Controllers
{
    interface IMenuController
    {
        void DisplayMenus();
        void GoTo(int choice);
    }
}
