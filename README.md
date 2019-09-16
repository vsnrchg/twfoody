# twfoody
Note - Solution for exercise-2 is in "AllocationEngine.DoBetterAllocation()" . This also will by-default apply the basic algo of exercise-1 in case it does not find any drivers who are available within the preparation time. Pls. refer Main()  in Program.cs for the test entry points and to follow the flow. Some functions and nested classes in AllocationEngine could ideally be private - retaining them as public for now for ease of calling for the current test purpose. Can be refactored later.   

# Driver allocation logic
- Filter drivers who can each the restaurant within the order prep time

- Within the filtered drivers, assign to the person with the least orders

- If there are more than one with the least order, the person with highest rating is allocated

- To give an edge to drivers with higher rating, additional provision is made:

  - A driver with a higher rating can have x number of orders more than a person with lower rating and may still be considered as on par with the lower rating person, so that they get the allocation based on higher rating 

  - A place holder logic is implemented where x is calculated as x = (driver1.rating - driver2.rating)

  - So if there is a 3 rated driver with 1 order and a 4 rated driver with 2 orders, the 4 rated driver still gets precedence (x is 1 here (4 minus 3). In case the 4 rated driver has 3 orders, then the 3 rated driver gets precedence. Between a 3-rated driver with 1 order and a 5-rated driver with 3 orders, the 5-rated driver still gets precedence (x is 2 here)  

  - This logic is for now abstracted in GetPermittedBuffer() function and can be turned on/ off by changing IsBufferToBeApplied

- In case no driver found who is within the prep time, then by default allocation is done to the driver who is nearest (basic logic from exercise-1). Only the time is considered and not any other parameters in this case 

# Testing

Testing is done at following levels:
- Test for the basic function that decides which driver gets a chance between 2 drivers. This is the basic logic which is called within the allocation logic to decide between drivers. Test many possible combinations here. 

- Test for allocating an order against different driver lists - with each list having drivers of different review rating, Order count, and distance from restaurant

- Test for allocating multiple orders - one after the other - against a list of drivers having different review rating, Order count, and distance from restaurant. As they are tested in sequence, increase the order count of the allocated driver so that in the next step the increased order count is considered (all other parameters remain same)

- All tests assume driver location as given in the driver list remains static for the duration of the test. For e.g. if an order is allocated to a driver, then it is assumed that the next round of allocation for the next order happens after this driver has delivered the order and come back to their location as specified in the driver property  


# Test case data
- Each test is supplied data that can be input to the compare or allocation logic to be tested and includes information on which driver is expected to be selected. The test calls the logic and then compares with the expectedSelection to verify if the test succeeded or failed

- Determining the various combinations of test scenarios/ data is assumed to be a manual activity based on which these test case data can be defined

- For now test case data is hard coded in separate functions in TestCaseServiceClass, these can be modified to read the test case combinations from an external csv or json file

- The test case variations hard coded now in code are indicative, and covers many scenarios. The actual list can have more combination scenarios
