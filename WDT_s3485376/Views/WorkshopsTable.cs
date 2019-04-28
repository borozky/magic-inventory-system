using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;

namespace WDT_s3485376.Views
{
    // View component for displaying multiple workshops
    class WorkshopsTable
    {
        public WorkshopsTable(List<Workshop> workshops)
        {

            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            foreach (Workshop workshop in workshops)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("ID", workshop.ID.ToString());
                row.Add("Location", workshop.Location);
                row.Add("Runs on", workshop.Type);
                row.Add("Remaining Spaces", workshop.SpacesAvailable().ToString());
                row.Add("Available?", workshop.IsAvailable() ? "Yes" : "No");

                table.Add(row);
            }

            Console.WriteLine("WORKSHOPS");
            TableHelper.PrintTable(table);
            Console.WriteLine();
        }
    }
}
