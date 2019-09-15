using System;
using System.Collections;
using System.Collections.Generic;      
namespace DriverAllocation
{
    /// <summary>
    /// Class that contains multiple functions for driver comparison, simple allocations and allocation based on heuristics
    /// </summary>
    public class AllocationEngine
    {
        //place holder class to hold temporary info along with driver info that may help in the selection algo
        public class CandidateDriver
        {
            public Driver driverInfo { get; set; }
            //here for example the duration from restaurant is included
            //to be used in the driver selection logic in case of a tie on all other aspects
            public TripDetails  tripDetails { get; set; }
        }

        //class to hold trip details as supplemetary information while doiong allocation logic 
        public class TripDetails
        {
            public int srcX { get; set; }
            public int srcY { get; set; }
            public int destX { get; set; }
            public int destY { get; set; }
            public int distance { get; set; }
            public int durationMinutes { get; set; }
        }


        public AllocationEngine()
        {
        }
        /// <summary>
        /// Function with beter heuristics to get a driver allocated to the order
        /// Part of second day exercise
        /// </summary>
        /// <param name="restaurantAddress">The address of the restaurant in the order</param>
        /// <param name="consumerAddress">The address of the consumer in the order</param>
        /// <param name="favailableDrivers">List of available drivers</param>
        /// <param name="fOrderPrepTime">Preperation time for the order</param>
        /// <returns></returns>
        public static List<Driver> DoBetterAllocation(Address restaurantAddress, Address consumerAddress, List<Driver> favailableDrivers, int fOrderPrepTime)
        {
            //the list to be returned; empty by default
            List<Driver> selectedDrivers = new List<Driver>();

            //temporary objects to hold transient info along with driver info that can be used for selection based on heuristics
            CandidateDriver selectedCandidateDriver = new CandidateDriver();
            CandidateDriver currentCandidateDriver = null;// new CandidateDriver();

            //initialising the variable to be used for backup selection logic in case no one who can reach exactly within prep time
            //see second half of the loop below for the backup logic
            //backup selection to be held in thos
            Driver backUpDriverSelection = null;
            //temp assumption that the distance will be withing int.maxvalue
            int duration = Int32.MaxValue;

            //run thru each available driver info
            foreach (Driver driver in favailableDrivers)
            {
                //START - apply the better heuristics

                //get duration and distance of driver from restaurant
                TripDetails tripDetails = FindTripDetails(driver.address.x, driver.address.y, restaurantAddress.x, restaurantAddress.y);

                //shortlist this driver if their duration is within order prep time
                //this comparison below could be < or <= depending on how actual situations will be; for now going with <
                if (tripDetails.durationMinutes < fOrderPrepTime)
                {
                    //if we already have a selection, check and override it if current driver in the loop is more suitable
                    if (selectedCandidateDriver.driverInfo != null)
                    {
                        currentCandidateDriver = new CandidateDriver(); 
                        currentCandidateDriver.driverInfo = driver;
                        //assign trp details as this could be used in the GetDeservingCandidateDriver call
                        currentCandidateDriver.tripDetails = tripDetails;

                        //apply the heuristics to find which of the two are more deserving
                        selectedCandidateDriver = GetDeservingCandidateDriver(currentCandidateDriver, selectedCandidateDriver); 
                   }
                    else//assigning first driver as default selected
                    {
                        selectedCandidateDriver.driverInfo = driver;
                        selectedCandidateDriver.tripDetails = tripDetails; 
                    }
                }
                //END OF - apply the better heuristics

                //START - default logic from exercise-1 as backup in case no one available who can reach within the food preperation time, then
                //at present to demonstrate defaulting to basic logic that whoever is nearest is allocated at least
                //can change this later to beter logic with a geo-fence applied on how away they are to the restaurant
                //or to decide based on ratings or orders if more than one are exactly at the same distance from restaurant...
                ///...that will be similar to what's done above in exercise-2
                //less expensive to do this default logic in this same loop as all required params are available here, hence the trade off to do it here
                if (tripDetails.durationMinutes  < duration)
                {
                    backUpDriverSelection = driver;
                    duration = tripDetails.durationMinutes;
                }
                //END - default logic

            }

            //if driver available and selected based on heuristics
            if (selectedCandidateDriver.driverInfo != null)
            {
                Console.WriteLine("Allocating driver based on heuristics: " + selectedCandidateDriver.driverInfo.name); 
                selectedDrivers.Add(selectedCandidateDriver.driverInfo);  
            }
            else// get the driver selected based on the basic-distance-only-logic
            {
                Console.WriteLine("Going for backup selection" + backUpDriverSelection.name); 
                selectedDrivers.Add(backUpDriverSelection);  
            }

            //at present we are returning only one driver and keeping the logic simple
            //return value is kept as a list in case multiple drivers need to be selected
            //- say if both of them matching in all parameters or so and there is some interactive way of driver selection later
            return selectedDrivers; 
            
        }

        /// <summary>
        /// Function that can compare two given drivers and return the selected driver
        /// </summary>
        /// <param name="fCandidateDriver1"></param>
        /// <param name="fCandidateDriver2"></param>
        /// <returns></returns>
        public static CandidateDriver GetDeservingCandidateDriver(CandidateDriver  fCandidateDriver1, CandidateDriver fCandidateDriver2)
        {
            //assume driver2 as default selection
            CandidateDriver deservingCandidateDriver = fCandidateDriver2;

            //basic logic is to see if driver1 has lesser orders than driver2 then driver1 is more deserving
            int orderDiff = fCandidateDriver1.driverInfo.todaysOrders - fCandidateDriver2.driverInfo.todaysOrders;

            //keeping a provision for some adjustment
            //i.e., assume that higher rating drivers can still get the allocation if
            //the delta of their total order with a lower rating driver is within a defined limit

            //permittedBuffer can be used to adjust the orderDifference if required
            //by default keeping this 0; make IsBufferToBeApplied=true if buffer need to be applied
            //TODO: make this  config IsBufferToBeApplied = true/false 
            int permittedBuffer = 0;
            bool IsBufferToBeApplied = true;

            if (IsBufferToBeApplied)
            {
                //get the buffer applicable
                permittedBuffer = GetPermittedBuffer(fCandidateDriver1.driverInfo , fCandidateDriver2.driverInfo);
            }

            //apply the buffer on the delta of orders
            orderDiff -= permittedBuffer;


            //if driver1 has lesser orders after adjustment
            if (orderDiff < 0)
            {
                deservingCandidateDriver = fCandidateDriver1;
            }
            else if (orderDiff == 0)//if both have same order count after adjustment
            {
                //if driver1 is better rated
                if (fCandidateDriver1.driverInfo.reviewRating > fCandidateDriver2.driverInfo.reviewRating)
                {
                    deservingCandidateDriver = fCandidateDriver1;
                }
                //if both are having same rating
                else if(fCandidateDriver1.driverInfo.reviewRating == fCandidateDriver2.driverInfo.reviewRating)
                {
                    //select the one who can reach faster - given everything else remaining same
                    //no immediate advantage by doing this; but in case order gets prepared earlier...
                    //...or if we are considering things like less fuel spent
                    //...or the if other driver is nearer to another restaurant which may be good for the next order 
                    if(fCandidateDriver1.tripDetails.durationMinutes < fCandidateDriver2.tripDetails.durationMinutes)
                    {
                        deservingCandidateDriver = fCandidateDriver1; 
                    }
                }
            }
            return deservingCandidateDriver;
        }

        /// <summary>
        /// Place holder function with temporary logic to calculate a buffer for order count delta permitted
        /// between different rating levels
        /// </summary>
        /// <param name="fDriver1"></param>
        /// <param name="fDriver2"></param>
        /// <returns></returns>
        static int GetPermittedBuffer(Driver fDriver1, Driver fDriver2)
        {
            //how to arrive at that buffer limit could be any logic, so now abstracting to a fn with a default logic
            //i.e., for now keeping this defined limit the same as the rating diff
            //So, if there is a 3 rated driver with 1 order and 4 rated driver with 2 orders, still
            //the allocation will be given to the 4 rated driver
            //if there is a 3 rated driver with 1 order and 4 rated driver with 3 orders, then
            //the allocation will be given to the 3 rated driver

            return fDriver1.reviewRating - fDriver2.reviewRating;
        }

       
        /// <summary>
        /// The function to get Drivers allocated for a given order using simple logic of who is nearer by distance
        /// Part of first day exercise
        /// </summary>
        /// <param name="restaurantAddress">The address of the restaurant in the order</param>
        /// <param name="consumerAddress">The address of the consumer in the order</param>
        /// <param name="favailableDrivers">List of available drivers</param>
        /// <returns></returns>
        public static List<Driver> DoAllocation(Address restaurantAddress, Address  consumerAddress, List<Driver> favailableDrivers)
        {
            List<Driver> selectedDrivers = new List<Driver>();
            Driver selectedDriver = null;

            //temp assumption that the distance will be withing int.maxvalue
            int distance = Int32.MaxValue;

            foreach (Driver driver in favailableDrivers)
            {
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
        /// Find the total distance between driver to restaurant to consumer location
        /// </summary>
        /// <param name="fDriver"></param>
        /// <param name="fOrder"></param>
        /// <returns></returns>
        static int FindRouteDistance(Driver fDriver, Order fOrder)
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
        static int FindDistance(int fSrcX, int fSrcY, int fDestX, int fDestY)
        {
            int distance = 0;

            int deltaX = fDestX - fSrcX;
            int deltaY = fDestY - fSrcY;

            Convert.ToInt32(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));

            return distance;
        }

        /// <summary>
        /// Find the duration in minutes between two given locations - temporary logic now
        /// first finds distance and then applies a temp logic
        /// </summary>
        /// <param name="fSrcX"></param>
        /// <param name="fSrcY"></param>
        /// <param name="fDestX"></param>
        /// <param name="fDestY"></param>
        /// <returns></returns>
        static int FindTripDuration(int fSrcX, int fSrcY, int fDestX, int fDestY)
        {
            int distance = 0;

            int deltaX = fDestX - fSrcX;
            int deltaY = fDestY - fSrcY;


            Convert.ToInt32(Math.Sqrt(deltaX * deltaX + deltaY * deltaY));

            //temporary logic - 5 points equals 1Km, and 1Km takes 2 min.
            int duration = distance / 5 * 2;
            return duration;
        }

        /// <summary>
        /// Function that finds both distance and duration between two given points
        /// temporary logic for distance and duration in minutes
        /// assumes distance and duration are always less than int32.max
        /// </summary>
        /// <param name="fSrcX"></param>
        /// <param name="fSrcY"></param>
        /// <param name="fDestX"></param>
        /// <param name="fDestY"></param>
        /// <returns></returns>
        static TripDetails  FindTripDetails(int fSrcX, int fSrcY, int fDestX, int fDestY)
        {
            int distance = 0;

            int deltaX = fDestX - fSrcX;
            int deltaY = fDestY - fSrcY;

            distance = Convert.ToInt32 (Math.Sqrt(deltaX * deltaX + deltaY * deltaY));
            

            //temporary logic - 5 points equals 1Km, and 1Km takes 2 min.
            int duration = (distance / 5) * 2;

            TripDetails tripDetails = new TripDetails
            {
                srcX = fSrcX,
                srcY = fSrcY,
                destX = fDestX,
                destY = fDestY,
                distance = distance,
                durationMinutes = duration
            };

            return tripDetails;
        }
    }


}
