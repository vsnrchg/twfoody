using System;
namespace DriverAllocation
{
    /// <summary>
    /// Class representing a participating driver/ delivery person
    /// </summary>
    public class Driver
    {
        public Driver()
        {
        }

        public Driver(string fname, string faddressString, int faddressX, int faddressY, int fTodaysOrders, int fReviewRating)
        {
            name = fname;
            address = new Address();

            address.addressString = faddressString;
            address.x = faddressX;
            address.y = faddressY;

            todaysOrders = fTodaysOrders;
            reviewRating = fReviewRating; 
        }

        public string name { get; set; }
        public Address address { get; set; }
        public int todaysOrders { get; set; }
        public int reviewRating { get; set; }
    }
}
