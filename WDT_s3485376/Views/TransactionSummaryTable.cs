using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WDT_s3485376.Models;

namespace WDT_s3485376.Views
{
    // View component that displays customer receipt
    class TransactionSummaryTable
    {
        public TransactionSummaryTable(Orders order, Booking booking = null)
        {
            List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
            foreach (OrderLine line in order.orderLines)
            {
                Dictionary<string, string> row = new Dictionary<string, string>();
                row.Add("Product Name", line.ProductName);
                row.Add("Quantity", line.Qty.ToString());
                row.Add("Unit Price", string.Format("{0:C2}", line.UnitPrice));
                row.Add("Total Price", string.Format("{0:C2}", line.UnitPrice * line.Qty));

                table.Add(row);
            }
            
            Console.WriteLine();
            Console.WriteLine("Thank you for shopping at Marvellous Magic");
            Console.WriteLine("Your order has been processed.");
            Console.WriteLine();
            if (booking != null)
            {
                Console.WriteLine("You are booked to {0} {1}", booking.Location, booking.Type);
                Console.WriteLine("Your reference number is: {0}", booking.ReferenceNumber);
            }
            Console.WriteLine();
            Console.WriteLine("YOUR TRANSACTION SUMMARY");
            TableHelper.PrintTable(table);
            Console.WriteLine();
            Console.WriteLine("Subtotal: {0:C2}", order.SubTotal);
            Console.WriteLine("Discount: {0:C2} ({1} %)", order.DiscountPrice, order.DiscountRate);
            Console.WriteLine(string.Format("Grand Total: {0:C2}", order.GrandTotal));
            Console.WriteLine();
            if (booking != null)
                Console.WriteLine("You have choose to book into {0} {1} workshop", booking.Location, booking.Type);
            Console.WriteLine();
            Console.WriteLine();


            

        }
    }
}
