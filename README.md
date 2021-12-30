Branch for keeping track of bugs etcetera. Just contains a markdown file that I can modify on Github.

| Description | Replication | Fixed | Other Details/notes/braindumps/skoobly bloogus |
| ---         | ---         | ---   | ---                                            |
| Flipping the x velocity resets the y velocity to 0 | It's most easily replicated in the debug_parallax map. Go to one of the platforms and, while falling, repeatedly mash the A and D keys. You will fall slower. | [ ] |  |
| NullReferenceException in the "back to title" confirmation | Open the menu and select the "title" option. Then press ESC before selecting yes or no. Then select "yes." | [ ] | Doesn't trigger when pressing "yes" while the menu is still open. Occurs because pressing "yes" tries to destroy the menu instance, but it was already destroyed when pressing ESC. Just make it impossible to close the menu while the input queue length is greater than 1 or something |