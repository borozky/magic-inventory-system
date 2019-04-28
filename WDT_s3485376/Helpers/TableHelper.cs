using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDT_s3485376.Views
{
    // Helper for printing autoformatted tables
    static class TableHelper
    {
        // Prints list of key-value pairs as a table in the console
        public static void PrintTable(List<Dictionary<string, string>> table, int spacing = 3)
        {
            if (table.Count() == 0)
            {
                Console.WriteLine("============================");
                Console.WriteLine("    No items to display     ");
                Console.WriteLine("============================");
                return;
            }
            
            int[] numchars = new int[table.First().Count()];

            int n = 0;
            foreach (Dictionary<string, string> row in table)
            {
                // determine initial chars on first line
                if (n == 0)
                {
                    for (int i = 0; i < numchars.Length; i++)
                    {
                        numchars[i] = row.ElementAt(i).Key.Length + spacing;

                        // no spacing at the end
                        if (i == numchars.Length - 1)
                            numchars[i] -= spacing;
                    }
                }

                // cell value can be much longer than its key...
                // ...if so update formatting
                int m = 0;
                foreach (KeyValuePair<string, string> cell in row)
                {
                    if (cell.Value.ToString().Length > numchars[m] - spacing)
                    {
                        numchars[m] = cell.Value.ToString().Length + spacing;
                    }
                    m++;
                }

                n++;
            }

            // print the table
            int p = 0;
            foreach (Dictionary<string, string> row in table)
            {
                // header
                if (p == 0)
                {
                    // header row
                    int q = 0;
                    foreach (KeyValuePair<string, string> cell in row)
                    {
                        Console.Write("{0,-" + numchars[q] + "}", cell.Key);
                        q++;
                    }
                    Console.WriteLine();

                    // sepatator '='
                    int r = 0;
                    foreach (KeyValuePair<string, string> cell in row)
                    {
                        Console.Write("{0,-" + numchars[r] + "}", new string('=', numchars[r]));
                        r++;
                    }
                    Console.WriteLine();
                }

                // body
                int g = 0;
                foreach (KeyValuePair<string, string> cell in row)
                {
                    Console.Write("{0,-" + numchars[g] + "}", cell.Value.ToString());
                    g++;
                }
                Console.WriteLine();

                p++;
            }
        }
    }
}
