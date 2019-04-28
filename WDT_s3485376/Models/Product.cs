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
    class Product
    {
        // fields for parsing JSON
        public int Id = 0;
        public string Name;
        public int StockLevel = 0;
        public double Price = 0;
        

        // Gets all the products
        public static List<Product> All(string location)
        {
            string ext = location.ToLower() == "owners" ? "json" : "txt";
            string path = Path.Combine(PathHelper.BASEPATH, string.Format(@"Resources/{0}_inventory.{1}", location, ext));
            try
            {
                // file must exist
                if (! File.Exists(path))
                    throw new ArgumentException(string.Format("Invalid location. File, {0}_inventory.txt does not exist", location));
                
                string contents = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Product>>(contents);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }


        // Finds a product from a store by ID
        public static Product Find(int id, string location)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("ID must be greater than 0");

                List<Product> inventory = Product.All(location);

                Product res = (from inv in inventory
                          where inv.Id == id
                          select inv).First();

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


        // Finds a product from a store by name
        public static Product Find(string productname, string location)
        {
            try
            {
                if (productname.Trim() == "")
                    throw new ArgumentException("Product name cannot be empty");

                List<Product> inventory = Product.All(location);

                Product res = (from inv in inventory
                               where inv.Name.ToLower() == productname.ToLower()
                               select inv).First();

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


        // Gets the index of a product from a store
        public static int IndexOf(int id, string location)
        {
            // cannot simply use IndexOf() on Collections for some reason it is misbehaving
            // http://stackoverflow.com/questions/5184702/how-do-you-find-an-element-index-in-a-collectiont-inherited-class
            return Product.All(location)
                            .Select((item, index) => new {
                                Item = item,
                                Index = index
                            })
                            .First(i => i.Item.Id == id)
                            .Index;
        }


        // Saves/updates a product from a store.
        // When updating items, recommend use Inventory::find() method to supply data
        public bool Save(string location)
        {
            string ext = location.ToLower() == "owners" ? "json" : "txt";
            string path = Path.Combine(PathHelper.BASEPATH, string.Format(@"Resources/{0}_inventory.{1}", location, ext));

            List<Product> inventories = Product.All(location);
            
            // updating/creating records may produce errors
            try
            {
                // don't create items with no names
                if (Id == 0 && Name == null)
                {
                    throw new Exception("Save failed! Name is required");
                }
                else if (Name != null)
                {
                    // don't create items that already exists
                    var itemsWithSameName = from inv in inventories
                                            where inv.Name.ToLower() == Name.ToLower()
                                            select inv;

                    if (itemsWithSameName.Count() > 0)
                    {
                        if (itemsWithSameName.First().Id != Id)
                        {
                            throw new Exception("Save failed! Item with same name already exists");
                        }
                    }
                }

                // if id is not provided, assign id as the latest id plus one
                if (Id == 0)
                {
                    // assume there's no inventory on this store, set ID = 1
                    Id = 1;

                    var latestItem = from inv in inventories
                                     orderby inv.Id descending
                                     select inv;

                    // inventory is not empty
                    if (latestItem.Count() > 0)
                        Id = latestItem.First().Id + 1;
                    

                    //  create record
                    Product item = new Product();
                    item.Id = Id;                   // created automatically if not provided
                    item.Name = Name;               // name is required
                    item.StockLevel = StockLevel;   // defaults to 0 if not provided
                    item.Price = Price;

                    inventories.Add(item);
                    

                }

                // modify record
                else
                {
                    int index = Product.IndexOf(Id, location);
                    Product item = inventories.ElementAt(index);
                    
                    item.Name = Name != null ? Name : item.Name;
                    item.StockLevel = StockLevel > -1 ? StockLevel : item.StockLevel;
                    
                }

                // save
                string json = JsonConvert.SerializeObject(inventories, Formatting.Indented);
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
