using System;
namespace DriverAllocation
{
    public class Order
    {
        public Order()
        {
        }
        public string DummyOrderDetails { get; set; }
        public Consumer consumer { get; set; }
        public Restaurant restaurant { get; set; }
    }
}
