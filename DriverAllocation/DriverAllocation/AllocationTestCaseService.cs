using System;
using System.Collections;
using System.Collections.Generic;

namespace DriverAllocation
{
    /// <summary>
    /// Class for holding the test case data for testing the function that determines which driver is more deserving
    /// </summary>
    public class DriverComparisonTestPair
    {
        public AllocationEngine.CandidateDriver driver1 { get; set; }
        public AllocationEngine.CandidateDriver driver2 { get; set; }
        //assuming for now driver name is unique , ideally this should be an ID or so
        public string expectedSelectionDrivername { get; set; }

        //convenience contructor to help generate the test case 
        public DriverComparisonTestPair(string d1Name, int d1Orders, int d1Rating, int d1Distance, int d1Duration,
            string d2Name, int d2Orders, int d2Rating, int d2Distance, int d2Duration, string expectedSelectionName)
        {
            driver1 = new AllocationEngine.CandidateDriver()
            {
                driverInfo = new Driver()
                {
                    name = d1Name,
                    todaysOrders = d1Orders,
                    reviewRating = d1Rating,
                },
                tripDetails = new AllocationEngine.TripDetails()
                {
                    distance = d1Distance,
                    durationMinutes = d1Duration
                }

            };
            driver2 = new AllocationEngine.CandidateDriver()
            {
                driverInfo = new Driver()
                {
                    name = d2Name,
                    todaysOrders = d2Orders,
                    reviewRating = d2Rating,
                },
                tripDetails = new AllocationEngine.TripDetails()
                {
                    distance = d2Distance,
                    durationMinutes = d2Duration
                }
            };
            expectedSelectionDrivername = expectedSelectionName;

        }

    }

  
    /// <summary>
    /// Class holding the data for the driver allocation test case
    /// This one holds a list of drivers and list of orders with the expectation that the test function will
    /// try to allocate a driver to each of the orders one after the other
    /// </summary>
    public class DriverAllocationTestCase
    {
        public List<Driver> availableDrivers { get; set; }

        //the tuple holds the order and the string which has the name of the driver expected to get allocated for this order
        public List<Tuple<Order, string>> ordersWithExpectedAllocation { get; set; }   
    }

    /// <summary>
    /// Class that holds the functions that supply the test data for different test cases
    /// Currently all hard coded, later this can read from a configurable source like a csv or other
    /// </summary>
    public class AllocationTestCaseService
    {
        public AllocationTestCaseService()
        {
        }
        /// <summary>
        /// Enumeration - for now temporary mechanism - to represent different allocation test cases
        /// ideally these would be external configiration from another source like a csv or so
        /// </summary>
        public enum AllocationTestCases
        {
            SingleorderDifferentRatingDriver,
            SingleorderDifferentRatingDriverTestCaseList,
            AllDriversSameRating,
            NoDriverWithinPrepTime,
            NoDriverWithinPrepTime2,
            DriversWithDifferentRating
        }

        public static List<DriverComparisonTestPair> GetDriverComparisonTestPairs()
        {
            //this list is hard coded here now, can be read from a test case file
            List<DriverComparisonTestPair> testCases = new List<DriverComparisonTestPair>()
            {
                //each row lists details for 2 drivers
                //row fields in the order driverName, todaysOrders, rating, distance, duration, repeat for second driver
                //last value in each row indicates who is expected to be picked
                new DriverComparisonTestPair("James", 0, 4, 60, 24, "Bruce", 0, 4, 50, 20, "Bruce")
                ,new DriverComparisonTestPair("James", 0, 4, 60, 24, "Ethan", 0, 4, 40, 16, "Ethan")
                ,new DriverComparisonTestPair("Bruce", 0, 3, 60, 24, "Ethan", 2, 4, 40, 16, "Bruce")
                ,new DriverComparisonTestPair("Bruce", 0, 3, 60, 24, "Ethan", 1, 4, 40, 16, "Ethan")
            };
            return testCases;
        }

        /// <summary>
        /// Helper function to get the right test case data based on the pre-defined test caseId
        /// </summary>
        /// <param name="fTestCaseId"></param>
        /// <returns></returns>
        public static DriverAllocationTestCase GetDriverAllocationTestCase(AllocationTestCases fTestCaseId)
        {
            switch (fTestCaseId)
            {
                case AllocationTestCases.SingleorderDifferentRatingDriver : return GetSingleorderDifferentRatingDriverTestCase();
                case AllocationTestCases.AllDriversSameRating : return GetSameRatingDriversAllocationTestCase();
                case AllocationTestCases.DriversWithDifferentRating : return GetDriversWithDifferentRatingTestCase();
                case AllocationTestCases.NoDriverWithinPrepTime : return GetNoDriverWithinPrepTimeTestCase();
                case AllocationTestCases.NoDriverWithinPrepTime2: return GetNoDriverWithinPrepTimeTestCase2();
                default: return null;
            }
        
        }

        public static DriverAllocationTestCase GetSingleorderDifferentRatingDriverTestCase()
        {
            DriverAllocationTestCase testCase = new DriverAllocationTestCase();

            //this list is hard coded here now, can be read from a test case file
            testCase.availableDrivers = new List<Driver>()
            {
                //name, addressstring,x,y,todaysOrders,reviewrating
                new Driver("James", "Wonder Drive", 35, 20, 0, 3)
                , new Driver("Bruce", "Mountain drive", 25, 20, 2, 4)
                , new Driver("Ethan", "Central square", 22, 20, 2, 4)
                , new Driver("Jason", "Farm Drive", 30, 20, 2, 5) 
            };


            //the idea is to call allocation for orders one after the other
            //for now keeping it simple, not considering which driver is busy etc., assuming whoever is in the
            //above driver list are available at their home location and each order request comes one after the other
            //after the previous order is delivered and driver returned to base
            //so that each order in below list will be processed one after the other
            //assumed that whenever allocation is done todaysOrder count for each driver is incremented in the system

            //for testing the driver list with dynamic changes , another test case to be written

            //this list holds the order and the expected driver allocation for that when called one after the other
            //this list is hard coded here now, can be read from a test case file
            testCase.ordersWithExpectedAllocation = new List<Tuple<Order, string>>()
            {
                //new Order(orderDescString, prep time, consumerName, consumerAddr, consumerX, consumerY, restarantName, restaurantAddr, restaurantX, restaurantY), expectedSelection
                new Tuple<Order, string>(new Order("#1 Medium Pizza; INR 350", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Jason")
            };

            return testCase;
        }

        public static List<DriverAllocationTestCase> GetSingleorderDifferentRatingDriverTestCaseList()
        {
            List<DriverAllocationTestCase> testCaseList = new List<DriverAllocationTestCase>();

            //Test case with higher rating driver gets allocated due delta being within buffer
            //though lower rating driver has less orders
            DriverAllocationTestCase testCase = new DriverAllocationTestCase();
            //this list is hard coded here now, can be read from a test case file
            testCase.availableDrivers = new List<Driver>()
            {
                //name, addressstring,x,y,todaysOrders,reviewrating
                new Driver("James", "Wonder Drive", 35, 20, 0, 3)
                , new Driver("Bruce", "Mountain drive", 25, 20, 2, 4)
                , new Driver("Ethan", "Central square", 22, 20, 2, 4)
                , new Driver("Jason", "Farm Drive", 30, 20, 2, 5)
            };
            testCase.ordersWithExpectedAllocation = new List<Tuple<Order, string>>()
            {
                //new Order(orderDescString, prep time, consumerName, consumerAddr, consumerX, consumerY, restarantName, restaurantAddr, restaurantX, restaurantY), expectedSelection
                new Tuple<Order, string>(new Order("#1 Medium Pizza; INR 350", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Jason")
            };
            testCaseList.Add(testCase);

            //Test case with lower rating driver gets allocated due to less orders as all higher rating drivers are beyond their bufferthreshold 
            testCase = new DriverAllocationTestCase();
            //this list is hard coded here now, can be read from a test case file
            testCase.availableDrivers = new List<Driver>()
            {
                //name, addressstring,x,y,todaysOrders,reviewrating
                new Driver("James", "Wonder Drive", 35, 20, 0, 3)
                , new Driver("Bruce", "Mountain drive", 25, 20, 2, 4)
                , new Driver("Ethan", "Central square", 22, 20, 2, 4)
                , new Driver("Jason", "Farm Drive", 30, 20, 3, 5)
            };
            testCase.ordersWithExpectedAllocation = new List<Tuple<Order, string>>()
            {
                //new Order(orderDescString, prep time, consumerName, consumerAddr, consumerX, consumerY, restarantName, restaurantAddr, restaurantX, restaurantY), expectedSelection
                new Tuple<Order, string>(new Order("#1 Medium Pizza; INR 350", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "James")
            };
            testCaseList.Add(testCase);

            return testCaseList;
        }


        /// <summary>
        /// Function that returns the test case with all drivers having same rating
        /// Hard coded for now, later to read from a source like json
        /// </summary>
        /// <returns></returns>
        public static DriverAllocationTestCase GetSameRatingDriversAllocationTestCase()
        {
            DriverAllocationTestCase testCase = new DriverAllocationTestCase();

            //this list is hard coded here now, can be read from a test case file
            testCase.availableDrivers = new List<Driver>()
            {
                //name, addressstring,x,y,todaysOrders,reviewrating
                new Driver("James", "Wonder Drive", 35, 20, 0, 4)
                , new Driver("Bruce", "Mountain drive", 25, 20, 0, 4)
                , new Driver("Ethan", "Central square", 22, 20, 0, 4)
            };


            //the idea is to call allocation for orders one after the other
            //for now keeping it simple, not considering which driver is busy etc., assuming whoever is in the
            //above driver list are available at their home location and each order request comes one after the other
            //after the previous order is delivered and driver returned to base
            //so that each order in below list will be processed one after the other
            //assumed that whenever allocation is done todaysOrder count for each driver is incremented in the system

            //for testing the driver list with dynamic changes , another test case to be written

            //this list holds the order and the expected driver allocation for that when called one after the other
            //this list is hard coded here now, can be read from a test case file
            testCase.ordersWithExpectedAllocation = new List<Tuple<Order, string>>()
            {
                //new Order(orderDescString, prep time, consumerName, consumerAddr, consumerX, consumerY, restarantName, restaurantAddr, restaurantX, restaurantY), expectedSelection
                new Tuple<Order, string>(new Order("#1 Medium Pizza; INR 350", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
                , new Tuple<Order, string>(new Order("#2 Large Pizza; INR 650", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Bruce")
                , new Tuple<Order, string>(new Order("#3 Large Combo; INR 950", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "James")
                , new Tuple<Order, string>(new Order("#4 Jumbo combo; INR 1250", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
            };

            return testCase;
        }

        /// <summary>
        /// Function that returns the test case with  drivers having different rating
        /// Hard coded for now, later to read from a source like json
        /// </summary>
        /// <returns></returns>
        public static DriverAllocationTestCase GetDriversWithDifferentRatingTestCase()
        {
            DriverAllocationTestCase testCase = new DriverAllocationTestCase();

            //this list is hard coded here now, can be read from a test case file
            testCase.availableDrivers = new List<Driver>()
            {
                //name, addressstring,x,y,todaysOrders,reviewrating
                new Driver("James", "Wonder Drive", 35, 20, 0, 3)
                , new Driver("Bruce", "Mountain drive", 25, 20, 0, 4)
                , new Driver("Ethan", "Central square", 22, 20, 0, 4)
            };


            //the idea is to call allocation for orders one after the other
            //for now keeping it simple, not considering which driver is busy etc., assuming whoever is in the
            //above driver list are available at their home location and each order request comes one after the other
            //after the previous order is delivered and driver returned to base
            //so that each order in below list will be processed one after the other
            //assumed that whenever allocation is done todaysOrder count for each driver is incremented in the system

            //for testing the driver list with dynamic changes , another test case to be written

            //this list holds the order and the expected driver allocation for that when called one after the other
            //this list is hard coded here now, can be read from a test case file
            testCase.ordersWithExpectedAllocation = new List<Tuple<Order, string>>()
            {
                //new Order(orderDescString, prep time, consumerName, consumerAddr, consumerX, consumerY, restarantName, restaurantAddr, restaurantX, restaurantY), expectedSelection
                new Tuple<Order, string>(new Order("#1 Medium Pizza; INR 350", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
                , new Tuple<Order, string>(new Order("#2 Large Pizza; INR 650", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Bruce")
                , new Tuple<Order, string>(new Order("#3 Large Combo; INR 950", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
                , new Tuple<Order, string>(new Order("#4 Jumbo combo; INR 1250", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Bruce")
                , new Tuple<Order, string>(new Order("#4 Garlic Bread; INR 250", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "James")
                , new Tuple<Order, string>(new Order("#5 Happy meals; INR 500", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
            };

            return testCase;
        }

        /// <summary>
        /// Function that returns the test case with no driver having duration that is less than order prep time...
        /// ...and the last one in the list being the right selection
        /// Hard coded for now, later to read from a source like json
        /// </summary>
        /// <returns></returns>
        public static DriverAllocationTestCase GetNoDriverWithinPrepTimeTestCase()
        {
            DriverAllocationTestCase testCase = new DriverAllocationTestCase();

            //this list is hard coded here now, can be read from a test case file
            testCase.availableDrivers = new List<Driver>()
            {
                //name, addressstring,x,y,todaysOrders,reviewrating
                new Driver("James", "Wonder Drive", 85, 20, 0, 4)
                , new Driver("Bruce", "Mountain drive", 75, 20, 0, 4)
                , new Driver("Ethan", "Central square", 72, 20, 0, 4) 
            };


            //the idea is to call allocation for orders one after the other
            //for now keeping it simple, not considering which driver is busy etc., assuming whoever is in the
            //above driver list are available at their home location and each order request comes one after the other
            //after the previous order is delivered and driver returned to base
            //so that each order in below list will be processed one after the other
            //assumed that whenever allocation is done todaysOrder count for each driver is incremented in the system

            //for testing the driver list with dynamic changes , another test case to be written

            //this list holds the order and the expected driver allocation for that when called one after the other
            //this list is hard coded here now, can be read from a test case file
            testCase.ordersWithExpectedAllocation = new List<Tuple<Order, string>>()
            {
                //new Order(orderDescString, prep time, consumerName, consumerAddr, consumerX, consumerY, restarantName, restaurantAddr, restaurantX, restaurantY), expectedSelection
                new Tuple<Order, string>(new Order("#1 Medium Pizza; INR 350", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
                , new Tuple<Order, string>(new Order("#2 Large Pizza; INR 650", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
                , new Tuple<Order, string>(new Order("#3 Large Combo; INR 950", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
                , new Tuple<Order, string>(new Order("#4 Jumbo combo; INR 1250", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Ethan")
            };
           
            return testCase; 
        }

        /// <summary>
        /// Function that returns the test case with no driver having duration that is less than order prep time...
        /// ...and one of the in between one in the list being the right selection
        /// Hard coded for now, later to read from a source like json
        /// </summary>
        /// <returns></returns>
        public static DriverAllocationTestCase GetNoDriverWithinPrepTimeTestCase2()
        {
            DriverAllocationTestCase testCase = new DriverAllocationTestCase();

            //this list is hard coded here now, can be read from a test case file
            testCase.availableDrivers = new List<Driver>()
            {
                //name, addressstring,x,y,todaysOrders,reviewrating
                new Driver("James", "Wonder Drive", 85, 20, 0, 4)
                , new Driver("Bruce", "Mountain drive", 72, 20, 0, 4)
                , new Driver("Ethan", "Central square", 75, 20, 0, 4)
            };


            //the idea is to call allocation for orders one after the other
            //for now keeping it simple, not considering which driver is busy etc., assuming whoever is in the
            //above driver list are available at their home location and each order request comes one after the other
            //after the previous order is delivered and driver returned to base
            //so that each order in below list will be processed one after the other
            //assumed that whenever allocation is done todaysOrder count for each driver is incremented in the system

            //for testing the driver list with dynamic changes , another test case to be written

            //this list holds the order and the expected driver allocation for that when called one after the other
            //this list is hard coded here now, can be read from a test case file
            testCase.ordersWithExpectedAllocation = new List<Tuple<Order, string>>()
            {
                //new Order(orderDescString, prep time, consumerName, consumerAddr, consumerX, consumerY, restarantName, restaurantAddr, restaurantX, restaurantY), expectedSelection
                new Tuple<Order, string>(new Order("#1 Medium Pizza; INR 350", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Bruce")
                , new Tuple<Order, string>(new Order("#2 Large Pizza; INR 650", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Bruce")
                , new Tuple<Order, string>(new Order("#3 Large Combo; INR 950", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Bruce")
                , new Tuple<Order, string>(new Order("#4 Jumbo combo; INR 1250", 10, "Forrest", "210, Lake Drive", 10, 10, "Pizza King", "220, Central Square", 20, 20), "Bruce")
            };

            return testCase;
        }
    }


}
