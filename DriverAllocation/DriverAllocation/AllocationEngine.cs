using System;
using System.Collections;
using System.Collections.Generic;      
namespace DriverAllocation
{
    public class AllocationEngine
    {
        public AllocationEngine()
        {
        }

        public static List<Driver> DoAllocation(Address restaurantAddress, Address  consumerAddress, List<Driver> favailableDrivers)
        {
            List<Driver> selectedDrivers = new List<Driver>();
            Driver selectedDriver = null;

            //temp assumption that the distance will be withing int.maxvalue
            int distance = Int32.MaxValue;

            foreach (Driver driver in favailableDrivers)
            {
                //int deliveryDistance = FindRouteDistance(driver, fOrder);
                int driverDistance = FindDistance(driver.address.x, driver.address.y, restaurantAddress.x, restaurantAddress.y);
                if (driverDistance < distance)
                {
                    selectedDriver = driver;
                    distance = driverDistance;
                }

            }

            //at present we are returning only one driver and keeping the logic simple
            //return value is kept as a list in case multiple drivers need to be selected - say both of them matching in all parameters
            if (selectedDriver != null) selectedDrivers.Add(selectedDriver);
            return selectedDrivers;

        }

        /// <summary>
        /// The funtion to get Drivers allocated
        /// </summary>
        /// <param name="fOrder">The order for which allocation to be done</param>
        /// <param name="favailableDrivers">List of available drivers</param>
        /// <returns></returns>
        public static List<Driver> AllocateDrivers(Order fOrder, List<Driver> favailableDrivers)
        {
            List<Driver> selectedDrivers = new List<Driver>();
            Driver selectedDriver = null;

            //temp assumption that the distance will be withing int.maxvalue
            int distance = Int32.MaxValue;

            foreach (Driver driver in favailableDrivers)
            {
                //int deliveryDistance = FindRouteDistance(driver, fOrder);
                int driverDistance = FindDistance(driver.address.x, driver.address.y, fOrder.restaurant.address.x, fOrder.restaurant.address.y);
                if (driverDistance < distance)
                {
                    selectedDriver = driver;
                    distance = driverDistance; 
                }

            }

            //at present we are returning only one driver and keeping the logic simple
            //return value is kept as a list in case multiple drivers need to be selected - say both of them matching in all parameters
            if (selectedDriver != null) selectedDrivers.Add(selectedDriver);  
            return selectedDrivers;
           
        }

        /// <summary>
        /// Find the total distance between driver to restaurant to consumer location
        /// </summary>
        /// <param name="fDriver"></param>
        /// <param name="fOrder"></param>
        /// <returns></returns>
        public static int FindRouteDistance(Driver fDriver, Order fOrder)
        {
            int routeDistance = 0;

            int driverToRestaurantDistance = FindDistance(fDriver.address.x, fDriver.address.y, fOrder.restaurant.address.x, fOrder.restaurant.address.y);
            int restaurantToConsumerDistance = FindDistance(fOrder.restaurant.address.x, fOrder.restaurant.address.y, fOrder.consumer.address.x, fOrder.consumer.address.y);

            routeDistance = driverToRestaurantDistance + restaurantToConsumerDistance; 
            return routeDistance; 

        }


        /// <summary>
        /// Find the distance between 2 given locations
        /// </summary>
        /// <param name="fSrcX"></param>
        /// <param name="fSrcY"></param>
        /// <param name="fDestX"></param>
        /// <param name="fDestY"></param>
        /// <returns></returns>
        public static int FindDistance(int fSrcX, int fSrcY, int fDestX, int fDestY)
        {
            int distance = 0;

            int deltaX = fDestX - fSrcX;
            int deltaY = fDestY - fSrcY;

            /* if (deltaX < 0) deltaX = deltaX * -1;
             if (deltaY < 0) deltaX = deltaY * -1;*/

            //distance = (deltaX * deltaX) + (2 * deltaX * deltaY) + (deltaY * deltaY);

            distance = deltaX * deltaX + deltaY * deltaY;
            
            return distance;
        }
    }


}
