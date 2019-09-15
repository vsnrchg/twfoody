using System;
namespace DriverAllocation
{
    /// <summary>
    /// Class holding the relevant parameters of a restaurant for the purpose of this exercise
    /// </summary>
    public class Restaurant
    {
        public Restaurant()
        {
        }

        public Restaurant(string fname, string faddressString, int faddressX, int faddressY)
        {
            name = fname;
            address = new Address();

            address.addressString = faddressString;
            address.x = faddressX;
            address.y = faddressY;
        }

        public string name { get; set; }
        public Address  address { get; set; }
    }
}
