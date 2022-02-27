Branch for keeping track of bugs etcetera. Just contains a markdown file that I can modify on Github.

| Description | Replication | Other Details/notes/braindumps/skoobly bloogus |
| ---         | ---         | ---                                            |
| NullReferenceException in the "back to title" confirmation | Open the menu and select the "title" option. Then press ESC before selecting yes or no. Then select "yes." | Doesn't trigger when pressing "yes" while the menu is still open. Occurs because pressing "yes" tries to destroy the menu instance, but it was already destroyed when pressing ESC. Just make it impossible to close the menu while the input queue length is greater than 1 or something |
| NPCs becoming followers doesn't make canInteractObj inactive | Go to debug_alpha_roof and interact with Sam |  |
| The player does not continue running if they are holding shift when changing map | Self-explanatory | |
