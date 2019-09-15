using System;
namespace DriverAllocation
{
    /// <summary>
    /// Common address class to be used to capture location info of entities
    /// </summary>
    public class Address
    {
        public Address()
        {
        }

        public string addressString { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }
}
