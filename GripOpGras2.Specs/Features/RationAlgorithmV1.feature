Feature: GripOpGras2.RationAlgorithmV1

The first approach devised to create a ration for a herd

Scenario: Create a ration for a herd, when having two roughage products
	Given I have the roughage product <roughage1> that contains <dm-roughage1> kg dm, <re-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <dm-roughage2> kg dm, <re-roughage2> g protein, and <vem-roughage2> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
	Then the ration should contain <kg-roughage1> kg dm of <roughage1>
	And the ration should contain <kg-roughage2> kg dm of <roughage2>
	And the ration must contain <grass-intake> kg of grass
Examples: 
| roughage1 | dm-roughage1 | re-roughage1 | vem-roughage1 | roughage2 | dm-roughage2 | re-roughage2 | vem-roughage2 | herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | kg-roughage1 | kg-roughage2 |
| kuilgras  | 400          | 160          | 920           | mais      | 370          | 60           | 960           | 100       | 1062.5       | 1000      | 210           | 3000           | 275          | 708          |

Scenario: Create a ration for a herd, when having three roughage products and two supplementary product
	Given I have the roughage product <roughage1> that contains <dm-roughage1> kg dm, <re-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <dm-roughage2> kg dm, <re-roughage2> g protein, and <vem-roughage2> VEM
	And I have the roughage product <roughage3> that contains <dm-roughage3> kg dm, <re-roughage3> g protein, and <vem-roughage3> VEM
	And I have the supplementary product <supplementary1> that contains <dm-supplementary1> kg dm, <re-supplementary1> g protein, and <vem-supplementary1> VEM
	And I have the supplementary product <supplementary1> that contains <dm-supplementary2> kg dm, <re-supplementary2> g protein, and <vem-supplementary2> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
	Then the ration should contain <kg-roughage1> kg dm of <roughage1>
	And the ration should contain <kg-roughage2> kg dm of <roughage2>
	And the ration should contain <kg-roughage3> kg dm of <roughage3>
	And the ration should contain <kg-supplementary1> kg dm of <supplementary1>
	And the ration should contain <kg-supplementary2> kg dm of <supplementary2>
	And the ration must contain <grass-intake> kg of grass
Examples: 
| roughage1 | dm-roughage1 | re-roughage1 | vem-roughage1 | roughage2 | dm-roughage2 | re-roughage2 | vem-roughage2 | roughage3 | dm-roughage3 | re-roughage3 | vem-roughage3 | supplementary1 | dm-supplementary1 | re-supplementary1 | vem-supplementary1 | supplementary2 | dm-supplementary2 | re-supplementary2 | vem-supplementary2 | herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | kg-roughage1 | kg-roughage2 | kg-roughage3 | kg-supplementary1 | kg-supplementary2 |

Scenario: Create a ration for a herd, when having two roughage products and one supplementary product
	Given I have the roughage product <roughage1> that contains <dm-roughage1> kg dm, <re-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <dm-roughage2> kg dm, <re-roughage2> g protein, and <vem-roughage2> VEM
	And I have the supplementary product <supplementary1> that contains <dm-supplementary1> kg dm, <re-supplementary1> g protein, and <vem-supplementary1> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
	Then the ration should contain <kg-roughage1> kg dm of <roughage1>
	And the ration should contain <kg-roughage2> kg dm of <roughage2>
	And the ration should contain <kg-supplementary1> kg dm of <supplementary1>
	And the ration must contain <grass-intake> kg of grass
Examples: 
| roughage1 | dm-roughage1 | re-roughage1 | vem-roughage1 | roughage2 | dm-roughage2 | re-roughage2 | vem-roughage2 | supplementary1 | dm-supplementary1 | re-supplementary1 | vem-supplementary1 | herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | kg-roughage1 | kg-roughage2 | kg-supplementary1 |

Scenario: Create a ration for a herd, when having three roughage products
	Given I have the roughage product <roughage1> that contains <dm-roughage1> kg dm, <re-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <dm-roughage2> kg dm, <re-roughage2> g protein, and <vem-roughage2> VEM
	And I have the roughage product <roughage3> that contains <dm-roughage3> kg dm, <re-roughage3> g protein, and <vem-roughage3> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
	Then the ration should contain <kg-roughage1> kg dm of <roughage1>
	And the ration should contain <kg-roughage2> kg dm of <roughage2>
	And the ration should contain <kg-roughage3> kg dm of <roughage3>
	And the ration must contain <grass-intake> kg of grass
Examples: 
| herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | roughage1 | dm-roughage1 | re-roughage1 | vem-roughage1 | roughage2 | dm-roughage2 | re-roughage2 | vem-roughage2 | roughage3 | dm-roughage3 | re-roughage3 | vem-roughage3 | kg-roughage1 | kg-roughage2 | kg-roughage3 |

Scenario: Create a ration for a herd, without having any feed products
	Given I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	When I let Grip op Gras 2 create a ration
	Then the ration must contain <grass-intake> kg of grass
Examples: 
| herd-size | grass-intake | vem-grass | protein-grass |
| 200       | 100          | 1000      | 200           |
| 100       | 0            | 10        | 200           |
| 100       | 100          | 0         | 0             |

Scenario: Create a ration for a herd when haven't grazed today and having two roughage products
	Given I have a herd with <herd-size> cows in it
	And my herd has produced <lmilk-produced> liters of milk
	And I have the roughage product <roughage1> that contains <dm-roughage1> kg dm, <re-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <dm-roughage2> kg dm, <re-roughage2> g protein, and <vem-roughage2> VEM
	When I let Grip op Gras 2 create a ration
	Then the ration should contain <kg-roughage1> kg dm of <roughage1>
	And the ration should contain <kg-roughage2> kg dm of <roughage2>
	And the ration must contain 0 kg of grass
Examples: 
| herd-size | lmilk-produced | roughage1 | dm-roughage1 | re-roughage1 | vem-roughage1 | roughage2 | dm-roughage2 | re-roughage2 | vem-roughage2 | kg-roughage1 | kg-roughage2 |

Scenario: Create a ration for a herd that doesnt contain any cows
	Given I have a herd with 0 cows in it
	And I have the roughage product mais that contains 370 kg dm, 60 g protein, and 960 VEM
	When I let Grip op Gras 2 create a ration
	Then an exception with the message 'The herd doesn't contain any cows' should be thrown
