Feature: GripOpGras2.LoginToFarmMaps

A short summary of the feature

Scenario: The app navigates to the login page
	When I open the Grip op Gras application
	Then the application should navigate to the FarmMaps login page

Scenario: The user logs into the application with FarmMaps
	Given that I am on the FarmMaps login page
	When I enter my username and password
	And I click the login button
	Then I will have to be redirected to the home page of the application
	And the page should show my email address

Scenario: The user tries to log into the application with invalid FarmMaps credentials
	Given that I am on the FarmMaps login page
	When I enter an incorrect username and password
	And I click the login button
	Then the login page should show me an error message
	
Scenario: The user logs out of the application
	Given that I am logged into the application
	And that I am currently on the home page
	When I click the logout button
	Then I should be logged out of the application