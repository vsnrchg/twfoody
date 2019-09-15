using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;    

namespace DriverAllocation
{
    //deprecated - used for the locally written test TestSimpleAllocation initially;
    //retained for reference
    //all test data definition otherwise moved to AllocationtestCaseService class
    public class TestDriverSet
    {
        public List<Driver> testDrivers { get; set; }
        public Driver expectedSelection { get; set; }
    }

    /// <summary>
    /// The class holding the testing functions
    /// </summary>
    [TestFixture] 
    public class AllocationTester
    {
        public AllocationTester()
        {
        }

        /// <summary>
        /// test function to test the core logic of determining which one of given 2 drivers gets preference
        /// </summary>
        [Test]
        public static void TestDriverComparison()
        {
            List<DriverComparisonTestPair> testCases = AllocationTestCaseService.GetDriverComparisonTestPairs();
            List<int> failedTestCases = new List<int>();
            int i = 0;

            Console.WriteLine("\n*********\nSTART OF testing driver pair comparisons"); 
            foreach (DriverComparisonTestPair testCase in testCases)
            {
                AllocationEngine.CandidateDriver selection = AllocationEngine.GetDeservingCandidateDriver(testCase.driver1, testCase.driver2);
                Console.WriteLine("Compared " + testCase.driver1.driverInfo.name + " and " + testCase.driver2.driverInfo.name + " and selected " + selection.driverInfo.name) ;
                if (selection.driverInfo.name != testCase.expectedSelectionDrivername)
                {
                    failedTestCases.Add(i);
                    Console.WriteLine("Wrong selection");
                }
                else
                {
                    Console.WriteLine("Right selection");
                }
                i++;
            }

            if (failedTestCases.Count > 0)
            {
                Console.Write("TestDriverComparison failed tests : ");
                foreach (int n in failedTestCases) { Console.Write("driver pair " + n + ";"); }
            }
            else
            {
                Console.WriteLine("All driver compare tests passed!!"); 
            }

           Assert.IsTrue(failedTestCases.Count ==0 ); 
        }

        /// <summary>
        /// Test Allocation of drivers to orders with heuristics - with multiple test cases for scenarios
        /// </summary>
        //[Test]  
        public static void TestBetterAllocationVariations()
        {
            TestBetterAllocation(AllocationTestCaseService.AllocationTestCases.SingleorderDifferentRatingDriver); 
            TestBetterAllocation(AllocationTestCaseService.AllocationTestCases.AllDriversSameRating);
            TestBetterAllocation(AllocationTestCaseService.AllocationTestCases.DriversWithDifferentRating);
            TestBetterAllocation(AllocationTestCaseService.AllocationTestCases.NoDriverWithinPrepTime);
            TestBetterAllocation(AllocationTestCaseService.AllocationTestCases.NoDriverWithinPrepTime2);
        }

        public static void RunAllocationtestCase(DriverAllocationTestCase fTestCase, AllocationTestCaseService.AllocationTestCases fTestCaseId)
        {
            Console.WriteLine("\n********\nSTART OF Testing the allocation case  " + fTestCaseId.ToString());
            if (fTestCase == null)
            {
                Console.WriteLine("No test case found " + fTestCaseId.ToString());
                return;
            }
            //for each order in the test case, find the allocation and verify one after the other
            foreach (Tuple<Order, string> orderWithExpectedAllocation in fTestCase.ordersWithExpectedAllocation)
            {
                //Order testorder = GetTestOrder();

                //get the order in the test case for which allocation to be done and the expected driver to be allocated as per test case definition
                Order testOrder = (Order)orderWithExpectedAllocation.Item1;
                string expectedDriverName = (string)orderWithExpectedAllocation.Item2;

                Console.WriteLine("\nAllocating drivers to Order" + testOrder.DummyOrderDetails);

                //TestDriverSet testDriverSet = GetTestDrivers();

                //Perform the actual allocation
                List<Driver> selectedDrivers = AllocationEngine.DoBetterAllocation(testOrder.restaurant.address, testOrder.consumer.address, fTestCase.availableDrivers, testOrder.prepTimeMinutes);
                //for now we expect only one driver returned in the list
                if (selectedDrivers != null && selectedDrivers.Count > 0)
                {
                    Console.WriteLine("Selected Driver is " + selectedDrivers[0].name);

                    if (selectedDrivers[0].name == expectedDriverName)
                    {
                        Console.WriteLine("Right selection");
                        //increment the driver's orders so that in the next allocation call it is considered
                        selectedDrivers[0].todaysOrders++;
                    }
                    else
                    {
                        Console.WriteLine("Wrong selection");
                    }
                }
                else
                {
                    Console.WriteLine("No driver selected??!!");
                }

                Assert.IsTrue(selectedDrivers[0].name == expectedDriverName);
            }

            Console.WriteLine("\nEND OF Testing the allocation case  " + fTestCaseId.ToString() + "\n********\n");
        }
        /// <summary>
        /// Test function to test the  allocation function AllocationEngine.DoBetterAllocation with one test case
        /// </summary>
        [TestCase (AllocationTestCaseService.AllocationTestCases.SingleorderDifferentRatingDriver)]
        [TestCase(AllocationTestCaseService.AllocationTestCases.AllDriversSameRating)]
        [TestCase(AllocationTestCaseService.AllocationTestCases.DriversWithDifferentRating)]
        [TestCase(AllocationTestCaseService.AllocationTestCases.NoDriverWithinPrepTime)]
        [TestCase(AllocationTestCaseService.AllocationTestCases.NoDriverWithinPrepTime2)]
        public static void TestBetterAllocation( AllocationTestCaseService.AllocationTestCases fTestCaseId)
        {
            //get the test case data from a test case service 
            DriverAllocationTestCase theTestCase = AllocationTestCaseService.GetDriverAllocationTestCase(fTestCaseId);
            RunAllocationtestCase(theTestCase, fTestCaseId); 
        }

        [Test]
        public static void TestBetterAllocationDriverListVariations()
        {
            List<DriverAllocationTestCase> theTestCaseList = AllocationTestCaseService.GetSingleorderDifferentRatingDriverTestCaseList();
            Console.WriteLine("\n************\nSTART OF Testing list of driver list variations with single order\n"); 
            foreach (DriverAllocationTestCase theTestCase in theTestCaseList)
            {
                RunAllocationtestCase(theTestCase, AllocationTestCaseService.AllocationTestCases.SingleorderDifferentRatingDriver);
            }
            Console.WriteLine("\nEND OF Testing list of driver list variations with single order\n************\n");
        }

        /// <summary>
        /// test function to test the simple allocation from Day-1
        /// </summary>
        public static void TestSimpleAllocation()
        {
            Order testorder = GetTestOrder();

            Console.WriteLine("Allocating drivers to Order" + testorder.DummyOrderDetails);

            TestDriverSet testDriverSet  = GetTestDrivers();

            //List<Driver> selectedDrivers  = AllocationEngine.AllocateDrivers(testorder, testDrivers);
            List<Driver> selectedDrivers = AllocationEngine.DoAllocation(testorder.restaurant.address, testorder.consumer.address, testDriverSet.testDrivers);
            //for now we expect only one driver in the list
            if (selectedDrivers != null && selectedDrivers.Count > 0)
            {
                Console.WriteLine("Selected Driver is " + selectedDrivers[0].name);
                  
                if (selectedDrivers[0].name == testDriverSet.expectedSelection.name)
                {
                    Console.WriteLine("Right selection");
                }
                else
                {
                    Console.WriteLine("Wrong selection");
                }
            }
            else
            {
                Console.WriteLine("No driver selected??!!");
            }

            //Assert.IsTrue(selectedDrivers[0].name == testDriverSet.expectedSelection.name);
        }

        /// <summary>
        /// Deprecated - helper function for simple allocation test
        /// </summary>
        /// <returns></returns>
        public static Order GetTestOrder()
        {
            Order testOrder = new Order();
            testOrder.DummyOrderDetails = "1 Medium Pizza; INR 350";

            //assign a test customer
            testOrder.consumer = new Consumer();
            testOrder.consumer.name = "Forrest";
            testOrder.consumer.address = new Address
            {
                addressString = "210, lake Drive",
                x = 10,
                y = 10
            };

            //assign a test Restaurant
            testOrder.restaurant = new Restaurant();
            testOrder.restaurant.name = "Pizza King";
            testOrder.restaurant.address = new Address
            {
                addressString = "220, abcd Drive",
                x = 20,
                y = 20
            };

            testOrder.prepTimeMinutes = 10; 


            return testOrder;
        }

        /// <summary>
        /// Deprecated - helper function for simple allocation test
        /// </summary>
        /// <returns></returns>
        public static TestDriverSet GetTestDrivers()
        {
            TestDriverSet testDriverSet = new TestDriverSet();
            
            testDriverSet.testDrivers  = new List<Driver>();

            Driver d = new Driver();
            d.name = "James";
            d.address = new Address();
            d.address.addressString = "Wonder Drive";
            d.address.x = 55;
            d.address.y = 20;
            d.reviewRating = 4;
            d.todaysOrders = 4;
            testDriverSet.testDrivers.Add(d);

            d = new Driver();
            d.name = "Bruce";
            d.address = new Address();
            d.address.addressString = "Mountain Drive";
            d.address.x = 42;
            d.address.y = 20;
            d.reviewRating = 4;
            d.todaysOrders = 2;
            testDriverSet.testDrivers.Add(d);


            d = new Driver();
            d.name = "Ethan";
            d.address = new Address();
            d.address.addressString = "Central square";
            d.address.x = 45;
            d.address.y = 20;
            d.reviewRating = 4;
            d.todaysOrders = 2;
            testDriverSet.testDrivers.Add(d);

            testDriverSet.expectedSelection = testDriverSet.testDrivers[2];   

            return testDriverSet;
        }
    }

   
}
