Feature: GripOpGras2.TestRationFromFrondEnd


Scenario: The user requests an ration from the home page
	Given that I am logged into the application
	And I am on the home page
	And I have 3 Products and 2 Supplementary Feedproducts that should be able to make a correct Ration
	And I have a herd with 59 cows which have produced a total of 1498.6 L milk
	And the herd has grazed 123 KG Dm of grass with 904 VEM and 180 Re per Kg Dm
	When I request the ration
	Then the page should contain a Ration within 120 seconds
	And The ration should contain 3 products
	And The ration should contain 123 KG grass 
