Feature: GripOpGras2.RationAlgorithmV1

The first approach devised to create a ration for a herd

Scenario: Create a ration for a herd, when having two roughage products
	Given I have the roughage product <roughage1> that contains <protein-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <protein-roughage2> g protein, and <vem-roughage2> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
    Then the ration should contain between <dm-roughage-ration-min> and <dm-roughage-ration-max> kg dm of roughage products
    And the ration should contain between <dm-supplementary-ration-min> and <dm-supplementary-ration-max> kg dm of supplementary products
    And the ration should contain between <protein-ration-min> and <protein-ration-max> g protein
    And the ration should contain between <vem-ration-min> and <vem-ration-max> VEM
	And the ration must contain <grass-intake> kg of grass
#Examples: 
#| roughage1 | protein-roughage1 | vem-roughage1 | roughage2 | protein-roughage2 | vem-roughage2 | herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | dm-roughage-ration-min | dm-roughage-ration-max | dm-supplementary-ration-min | dm-supplementary-ration-max | protein-ration-min | protein-ration-max | vem-ration-min | vem-ration-max |


Scenario: Create a ration for a herd, when having three roughage products
	Given I have the roughage product <roughage1> that contains <protein-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <protein-roughage2> g protein, and <vem-roughage2> VEM
	And I have the roughage product <roughage3> that contains <protein-roughage3> g protein, and <vem-roughage3> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
    Then the ration should contain between <dm-roughage-ration-min> and <dm-roughage-ration-max> kg dm of roughage products
    And the ration should contain between <protein-ration-min> and <protein-ration-max> g protein
    And the ration should contain between <vem-ration-min> and <vem-ration-max> VEM
	And the ration must contain <grass-intake> kg of grass
#Examples: 
#| roughage1 | protein-roughage1 | vem-roughage1 | roughage2 | protein-roughage2 | vem-roughage2 | roughage3 | protein-roughage3 | vem-roughage3 | herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | dm-roughage-ration-min | dm-roughage-ration-max | protein-ration-min | protein-ration-max | vem-ration-min | vem-ration-max | kg-roughage3-min | kg-roughage3-max |


Scenario: Create a ration for a herd, when having two roughage products and three supplementary product
	Given I have the roughage product <roughage1> that contains <protein-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <protein-roughage2> g protein, and <vem-roughage2> VEM
	And I have the supplementary product <supplementary1> that contains <protein-supplementary1> g protein, and <vem-supplementary1> VEM
	And I have the supplementary product <supplementary2> that contains <protein-supplementary2> g protein, and <vem-supplementary2> VEM
	And I have the supplementary product <supplementary3> that contains <protein-supplementary3> g protein, and <vem-supplementary3> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
    Then the ration should contain between <dm-roughage-ration-min> and <dm-roughage-ration-max> kg dm of roughage products
    And the ration should contain between <dm-supplementary-ration-min> and <dm-supplementary-ration-max> kg dm of supplementary products
    And the ration should contain between <protein-ration-min> and <protein-ration-max> g protein
    And the ration should contain between <vem-ration-min> and <vem-ration-max> VEM
	And the ration must contain <grass-intake> kg of grass
Examples: 
| roughage1   | protein-roughage1 | vem-roughage1 | roughage2     | protein-roughage2 | vem-roughage2 | supplementary1 | protein-supplementary1 | vem-supplementary1 | supplementary2 | protein-supplementary2 | vem-supplementary2 | supplementary3 | protein-supplementary3 | vem-supplementary3 | herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | dm-roughage-ration-min | dm-roughage-ration-max | dm-supplementary-ration-min | dm-supplementary-ration-max | protein-ration-min | protein-ration-max | vem-ration-min | vem-ration-max |
| silo 1-2019 | 161.56            | 861.56        | maissilo 2018 | 81.82             | 959.39        | PERSPULP 24%   | 95.24                  | 1037.62            | GEPLETTE TARWE | 117.14                 | 1219.29            | TARWE MEEL     | 121.11                 | 1240               | 59        | 123.9        | 903.81    | 180           | 1498.6         | 725.7                  | 806.58                 | 259.6                       | 309.41                      | 151925             | 162053.33          | 998870         | 1098757        |

Scenario: Create a ration for a herd, when having two roughage products and one supplementary product
	Given I have the roughage product <roughage1> that contains <protein-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <protein-roughage2> g protein, and <vem-roughage2> VEM
	And I have the supplementary product <supplementary1> that contains <protein-supplementary1> g protein, and <vem-supplementary1> VEM
	And I have a herd with <herd-size> cows in it, which have taken in <grass-intake> kg dm grass
	And each kg dm grass contains <vem-grass> VEM and <protein-grass> g protein
	And my herd has produced <lmilk-produced> liters of milk
	When I let Grip op Gras 2 create a ration
    Then the ration should contain between <dm-roughage-ration-min> and <dm-roughage-ration-max> kg dm of roughage products
    And the ration should contain between <dm-supplementary-ration-min> and <dm-supplementary-ration-max> kg dm of supplementary products
    And the ration should contain between <protein-ration-min> and <protein-ration-max> g protein
    And the ration should contain between <vem-ration-min> and <vem-ration-max> VEM
	And the ration must contain <grass-intake> kg of grass
#Examples:
#| roughage1 | protein-roughage1 | vem-roughage1 | roughage2 | protein-roughage2 | vem-roughage2 | supplementary1 | protein-supplementary1 | vem-supplementary1 | herd-size | grass-intake | vem-grass | protein-grass | lmilk-produced | dm-roughage-ration-min | dm-roughage-ration-max | dm-supplementary-ration-min | dm-supplementary-ration-max | protein-ration-min | protein-ration-max | vem-ration-min | vem-ration-max |


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
	And I have the roughage product <roughage1> that contains <protein-roughage1> g protein, and <vem-roughage1> VEM
	And I have the roughage product <roughage2> that contains <protein-roughage2> g protein, and <vem-roughage2> VEM
	When I let Grip op Gras 2 create a ration
    Then the ration should contain between <dm-roughage-ration-min> and <dm-roughage-ration-max> kg dm of roughage products
    And the ration should contain between <protein-ration-min> and <protein-ration-max> g protein
    And the ration should contain between <vem-ration-min> and <vem-ration-max> VEM
	And the ration must contain 0 kg of grass
Examples: 
| roughage1              | protein-roughage1 | vem-roughage1 | roughage2     | protein-roughage2 | vem-roughage2 | herd-size | lmilk-produced | dm-roughage-ration-min | dm-roughage-ration-max | protein-ration-min | protein-ration-max | vem-ration-min | vem-ration-max |
| Kuil 1 2020 + 21 balen | 180               | 907           | Maiskuil 2019 | 58.3              | 963           | 62        | 2002.6         | 954.89                 | 1018.55                | 143234.10          | 162968.57          | 1242170        | 1366387        |

Scenario: Create a ration for a herd that doesnt contain any cows
	Given I have a herd with 0 cows in it
	And I have the roughage product mais that contains 60 g protein, and 960 VEM
	When I let Grip op Gras 2 create a ration
	Then an exception with the message 'The herd doesn't contain any cows' should be thrown
