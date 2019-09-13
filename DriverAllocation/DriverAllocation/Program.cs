using System;
using System.Collections;
using System.Collections.Generic;

namespace DriverAllocation
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello Fuddy!");
            Order testorder = GetTestOrder();
            
            Console.WriteLine("Allocating drivers to Order" + testorder.DummyOrderDetails);

            List<Driver> testDrivers = GetTestDrivers();

            //List<Driver> selectedDrivers  = AllocationEngine.AllocateDrivers(testorder, testDrivers);
            List<Driver> selectedDrivers = AllocationEngine.DoAllocation(testorder.consumer.address, testorder.restaurant.address, testDrivers);
            //for now we expect only one driver in the list
            if (selectedDrivers != null && selectedDrivers.Count > 0)
            {
                Console.WriteLine("Selected Driver is " + selectedDrivers[0].name);
            }
            else
            {
                Console.WriteLine("No driver selected??!!"); 
            }
             
        }

        public static Order GetTestOrder()
        {
            Order testOrder = new Order();
            testOrder.DummyOrderDetails = "1 Medium Pizza; INR 350";

            //assign a test customer
            testOrder.consumer = new Consumer();
            testOrder.consumer.name = "Forrest";
            testOrder.consumer.address = new Address
            {
                addressString = "210, httpd square",
                x = 10,
                y = 10
            };

            //assign a test Restaurant
            testOrder.restaurant  = new Restaurant();
            testOrder.restaurant.name = "Pizza King";
            testOrder.restaurant.address = new Address
            {
                addressString = "220, abcd Drive",
                x = 20,
                y = 20
            };


            return testOrder;
        }

        public static List<Driver> GetTestDrivers()
        {
            List<Driver> testDriverList = new List<Driver>();

            Driver d = new Driver();
            d.name = "James";
            d.address = new Address();
            d.address.addressString = "MI6 square";
            d.address.x = 35;
            d.address.y = 20;

            testDriverList.Add(d);
            d = new Driver();
            d.name = "Bruce";
            d.address = new Address();
            d.address.addressString = "Cave Drive";
            d.address.x = 25;
            d.address.y = 20;

            testDriverList.Add(d);

            
            d = new Driver();
            d.name = "Ethan";
            d.address = new Address();
            d.address.addressString = "impossible square";
            d.address.x = 22;
            d.address.y = 20;
            testDriverList.Add(d);

            return testDriverList;
        }
    }
}
