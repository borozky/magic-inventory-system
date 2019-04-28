using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDT_s3485376.Helpers;

namespace WDT_s3485376.Models
{
    class Booking
    {
        public enum BookingType { MORNING, AFTERNOON }

        // Fields for JSON.NET to populate
        public long ReferenceNumber;
        public int OrderID;
        public string Location;
        public string Type;


        // Cache all bookings
        private static List<Booking> all;


        // Creates a booking
        public Booking(int orderID, string location, BookingType type)
        {
            OrderID = orderID;
            Location = location;
            Type = type.ToString();
            ReferenceNumber = GenerateReferenceNumber();
        }


        // Generates a unique reference number.
        private long GenerateReferenceNumber()
        {
            return DateTime.Now.Ticks;
        }


        // Gets all bookings
        public static List<Booking> All()
        {
            return all == null ? Retrieve() : all;
        }

        // Finds all booking by location
        public static List<Booking> findByLocation(string location)
        {
            var res =  from item in All()
                       where item.Location.ToLower() == location.ToLower()
                       select item;
            return (res == null || res.Count() == 0 ) ? new List<Booking>() : res.ToList();
        }


        // Gets all bookings from file
        private static List<Booking> Retrieve()
        {
            try
            {
                string path = Path.Combine(PathHelper.BASEPATH, @"Resources/Bookings.txt");
                string contents = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<Booking>>(contents);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }


        // Uploads all cached booking data into the file
        public static bool Persist()
        {
            try
            {
                string path = Path.Combine(PathHelper.BASEPATH, @"Resources/Bookings.txt");
                string json = JsonConvert.SerializeObject(all, Formatting.Indented);
                File.WriteAllText(path, json);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }


        // Saves the current booking
        public bool Save()
        {
            int max = (from item in Workshop.All()
                       where item.Location == Location && item.Type == Type
                       select item).First().Max;

            List<Booking> obj = Retrieve();

            try
            {
                // check if the workshop is full
                var res = from space in obj
                          where space.Location == Location && space.Type == Type
                          select space;
                
                if (res != null)
                {
                    List<Booking> bookedSpaces = res.ToList<Booking>();
                    if (bookedSpaces.Count() >= max)
                        throw new Exception("Workshop spaces are already full. Please pick another workshop");
                }

                obj.Add(this);

                string path = Path.Combine(PathHelper.BASEPATH, @"Resources/Bookings.txt");
                string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
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
