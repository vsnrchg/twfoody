using System;
namespace DriverAllocation
{
    /// <summary>
    /// Class holding the relevant parameters of an order for the purpose of this exercise
    /// </summary>
    public class Order
    {
        public Order()
        {
        }
        public Order(string fDummyDetails, int fPrepTimeMinutes,
            string fConsumerName, string fConsumerAddrString, int fConsumerAddrX, int fConsumerAddrY,
            string fRestaurantName, string fRestaurantAddrString, int fRestaurantAddrX, int fRestaurantAddrY)
        {
            DummyOrderDetails = fDummyDetails;
            prepTimeMinutes = fPrepTimeMinutes;
            consumer = new Consumer(fConsumerName, fConsumerAddrString, fConsumerAddrX, fConsumerAddrY);
            restaurant = new Restaurant(fRestaurantName, fRestaurantAddrString, fRestaurantAddrX, fRestaurantAddrY);

        }
        public string DummyOrderDetails { get; set; }
        public Consumer consumer { get; set; }
        public Restaurant restaurant { get; set; }
        public int prepTimeMinutes { get; set; }
    }
}
