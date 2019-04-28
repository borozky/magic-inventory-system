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
    class Workshop
    {
        // Fields for JSON.NET to populate
        public int ID;
        public string Location;
        public string Type;
        public int Max;


        // Caches all workshops here
        private static List<Workshop> all = new List<Workshop>();
        

        // Gets all workshops
        public static List<Workshop> All()
        {
            try
            {
                string path = Path.Combine(PathHelper.BASEPATH, @"Resources/workshops.txt");
                string json = File.ReadAllText(path);
                all = JsonConvert.DeserializeObject<List<Workshop>>(json);
                return all;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        // Gets all workshop by location
        public static List<Workshop> All(string location)
        {
            List<Workshop> allWorkshops = All();
            return (from item in allWorkshops
                    where item.Location.ToLower() == location.ToLower()
                    select item).ToList<Workshop>();
        }


        // Finds a workshop by ID
        public static Workshop Find(int ID)
        {
            var res = from workshop in All()
                      where workshop.ID == ID
                      select workshop;
            return res != null ? res.First() : null;
        }


        // Checks if the current workshop is available
        public bool IsAvailable()
        {
            try
            {
                return SpacesAvailable() > 0 ? true : false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        // Gets remaining spaces
        public int SpacesAvailable()
        {
            try
            {
                List<Booking> bookings = Booking.All();

                var resBookings = from item in bookings
                                  where item.Location.ToLower() == Location.ToLower() && item.Type.ToLower() == Type.ToLower()
                                  select item;

                List<Booking> foundBookings = resBookings.ToList();

                return Max - foundBookings.Count();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }


    }

    
}
