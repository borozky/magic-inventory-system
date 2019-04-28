using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
using WDT_s3485376.Helpers;

namespace WDT_s3485376.Models
{
    class StockRequest
    {
        // Fields for JSON.NET
        public int Id = 0;
        public string StoreName;
        public string ProductName;
        public int Quantity = -1;
        
        
        // gets all stock requests
        public static List<StockRequest> All()
        {

            string path = Path.Combine(PathHelper.BASEPATH, @"Resources/stockrequests.txt");

            try
            {
                // file must exist
                if (!File.Exists(path))
                    throw new ArgumentException("Stock request file cannot be found");
                
                string contents = File.ReadAllText(path);

                // if file is empty, insert an empty json array
                if (contents.Trim() == "")
                {
                    contents = "[]";
                    File.WriteAllText(path, "[]");
                }

                return JsonConvert.DeserializeObject<List<StockRequest>>(contents);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        

        // finds a stock request by ID
        public static StockRequest Find(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID must be greater than 0");

                List<StockRequest> storerequests = StockRequest.All();

                StockRequest res = (from req in storerequests
                               where req.Id == id
                               select req).First();

                if (res != null)
                    return res;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return null;
        }


        // Saves a stock request. This may add or edit exiting requests. 
        // Since this is an instance method, it is best that you'll use 
        // StockRequest.find() to get the instance of request you want to edit
        public bool Save()
        {
            // saving operation may fail
            try
            {
                if (StoreName == null)
                    throw new Exception("Store name is required");
                if (ProductName == null)
                    throw new Exception("Product name is required");
                if (Quantity < 0)
                    throw new Exception("Quantity is required");
                

                List<StockRequest> stockRequests = StockRequest.All();
                
                // autogenerate ID for new requests
                int useID = 1;
                if (stockRequests.Count() > 0)
                {
                    // ID must be unique
                    if (Id != 0)
                    {
                        int matches = ( from sr in stockRequests
                                        where sr.Id == Id
                                        select sr ).Count();
                        if (matches > 0)
                            throw new Exception("Item with similar ID already exists");
                    }

                    // ID of new item is the ID of latest item plus 1;
                    var latestItem = (from sr in stockRequests
                                      orderby sr.Id descending
                                      select sr).First();
                    useID = latestItem.Id + 1;
                }

                // stock requests can ONLY be added, not modified
                StockRequest stockRequest = new StockRequest();
                stockRequest.Id = useID;
                stockRequest.StoreName = StoreName;
                stockRequest.ProductName = ProductName;
                stockRequest.Quantity = Quantity;

                stockRequests.Add(stockRequest);
                
                // insert into file
                string path = Path.Combine(PathHelper.BASEPATH, @"Resources/stockrequests.txt");
                string json = JsonConvert.SerializeObject(stockRequests, Formatting.Indented);
                File.WriteAllText(path, json);

                return true;
                
            }
            // if anything goes wrong, return false
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        // Deletes an exiting request.
        // Since this is an instance method, it is best that you'll use 
        // StockRequest.find() to get the instance of request you want to delete
        public bool Delete()
        {
            string path = Path.Combine(PathHelper.BASEPATH, @"Resources/stockrequests.txt");
            try
            {
                if (Id == 0)
                    throw new Exception("Request ID is required");
                if (StoreName == null)
                    throw new Exception("Store name is required");
                if (ProductName == null)
                    throw new Exception("Product name is required");

                if (StockRequest.Find(Id) == null)
                    throw new Exception("This request cannot be found");

                List<StockRequest> items = StockRequest.All();
                var itemtoremove = items.Single(item => item.Id == Id);
                items.Remove(itemtoremove);

                string json = JsonConvert.SerializeObject(items, Formatting.Indented);
                File.WriteAllText(path, json);
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
