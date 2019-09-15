using System;
namespace DriverAllocation
{
    public class Consumer
    {
        public Consumer()
        {
        }

        public Consumer(string fname, string faddressString, int faddressX, int faddressY)
        {
            name = fname;
            address = new Address();

            address.addressString = faddressString;
            address.x = faddressX;
            address.y = faddressY;
        }

        public string name { get; set; }
        public Address address { get; set; }
    }
}
