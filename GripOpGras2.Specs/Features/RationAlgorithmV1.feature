Feature: GripOpGras2.RationAlgorithmV1

The first approach devised to create a ration for a herd

Scenario: Create an rantion for a herd, when having two 2 roughage products
	Given I have <roaghage1> that contains <dm-roaghage1> dm, <re-roaghage1> protein, and <vem-roaghage1> VEM
	And I have <roaghage2> that contains <dm-roaghage2> dm, <re-roaghage2> protein, and <vem-roaghage2> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
	Then the ration should contain <kg-roaghage1> kg dm of <roaghage1>
	And the ration should contain <kg-roaghage2> kg dm of <roaghage2>
	And the ration must contain the <grass-intake> kg of grass that the cows received during grazing
Examples: 
| herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | roaghage1 | dm-roaghage1 | re-roaghage1 | vem-roaghage1 | roaghage2 | dm-roaghage2 | re-roaghage2 | vem-roaghage2 | kg-roaghage1 | kg-roaghage2 |
| 120       | 720          | 720000    | 144000        | 27.14285713    | Graskuil  | 320          | 149          | 857           | Maïskuil  | 352          | 57           | 938           | 1300         | 900          |

Scenario: Create an rantion for a herd, without having any roughage products
	Given [context]
	When [action]
	Then [outcome]

Scenario: Create an rantion for a herd, when having no grass intake and two 2 roughage products
	Given [context]
	When [action]
	Then [outcome]