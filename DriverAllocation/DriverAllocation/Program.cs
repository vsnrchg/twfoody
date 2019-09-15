using System;
using System.Collections;
using System.Collections.Generic;

namespace DriverAllocation
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello Foody!");

            try
            {
                //test the basic compare between 2 given drivers based on the parameters
                //this test function will retrieve and run various test cases as defined in the testcaseservice module for this
                AllocationTester.TestDriverComparison();

                //test the overall allocation logic
                //this test function will retrieve and run various test cases as defined in the testcaseservice module
                //to test sequence of orders against a given driver list
                AllocationTester.TestBetterAllocationVariations();

                //test the overall allocation logic
                //this test function will retrieve and run various test cases as defined in the testcaseservice module
                //to test driver list variations against a given order
                AllocationTester.TestBetterAllocationDriverListVariations();  
            }
            catch (Exception testException)
            {
                Console.WriteLine("\nTest Exception " + testException.Message);
                Console.WriteLine("\n***********************\nTest case execution failed!!!\n\n****************************"); 
            }
       }
    }
}
