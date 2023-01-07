Feature: GripOpGras2.LoginToFarmMaps

A short summary of the feature

Scenario: The app navigates to the login page
	When I open the Grip op Gras application
	Then the application should navigate to the FarmMaps login page

Scenario: The user logs into the application with FarmMaps
	Given the user is on the login page of FarmMaps
	When the user enters the username and password
	And the user clicks the login button
	Then the application should navigate to the FarmMaps dashboard
	And the application should show the users email address