Feature: GripOpGras2.CalculateGrassIntakeV1

The first approach devised to calculate the total grass intake of a herd

Scenario: Calculate grass intake of a herd
	Given my plot has a size of <plotArea> ha
	And my plot has <netDryMatter> net kg dm/ha
	When I calculate the total grass intake
	Then the total grass intake of the herd should be <totalGrassIntake> kg dm
Examples:
| plotArea | netDryMatter | totalGrassIntake |
| 0.5      | 2500         | 1062.5           |
| 0        | 2500         | 0                |
| 0.5      | 0            | 0                |
| 0        | 0            | 0                |
| 2        | 2000         | 3400             |