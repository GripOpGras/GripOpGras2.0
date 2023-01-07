Feature: GripOpGras2.LoginToFarmMaps

A short summary of the feature

Scenario: The app navigates to the login page
	When I open the Grip op Gras application
	Then the application should navigate to the FarmMaps login page

Scenario: The user logs into the application with FarmMaps
	Given that I am on the FarmMaps login screen
	When I enter my username and password
	And I click the login button
	Then I will have to be redirected to the home page of the application
	And the page should show the users email address


