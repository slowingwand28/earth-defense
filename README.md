# Earth Defense

A turn-based strategy game built with Godot where players defend Earth from increasingly difficult attack waves. Manage resources and military strength across different regions, upgrade defenses, and survive until turn 45 to achieve victory.

## Instructions for Build and Use

Steps to build and/or run the software:

1. Install Godot 4.6 or later with C# support
2. Launch Godot and import the project folder
3. Select the project and click edit to open it in the Godot editor
4. Press F5 or click the Play button to start the game

Instructions for using the software:

1. Each turn, select a region by clicking on it to open the upgrade menu
2. Choose from three upgrade types: Infrastructure (resources), Military (strength), or Efficiency (growth multipliers)
3. Plan your defenses strategically—each region grows resources and strength each turn based on multipliers
4. Attack waves occur every 4 turns with increasing difficulty
5. Survive until turn 45 to win; if your regions are destroyed, you lose

## Development Environment

To recreate the development environment, you need the following software and/or libraries with the specified versions:

* Godot Engine 4.6 (with C# support)
* .NET 8.0 or higher
* VSCode with the following extensions:
  * C# Dev Kit
  * C# Tools for Godot
  * Godot Tools
* Gimp or any image editing software for creating and editing game assets

## Useful Websites to Learn More

I found these websites useful in developing this software:

* [Godot Engine Documentation](https://docs.godotengine.org)
* [Godot C# API Documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/)
* [W3Schools C# Tutorial](https://www.w3schools.com/cs/index.php)
* [Godot Audio Documentation](https://docs.godotengine.org/en/stable/tutorials/audio/index.html)
* [Godot Tween Documentation](https://docs.godotengine.org/en/stable/classes/class_tween.html)
* [Godot 2D Particle Systems Documentation](https://docs.godotengine.org/en/latest/tutorials/2d/particle_systems_2d.html)
* [SFXR Sound Effect Generator](https://sfxr.me/)

## Future Work

The following items I plan to fix, improve, and/or add to this project in the future:

* [ ] Add visual feedback for region upgrades
* [ ] Adjust game balance for attack wave difficulty and resource growth, posibly reworking the entire upgrade system
* [ ] Fix bug causing end turn button to open upgrade menu
* [ ] Make region information more clear and accessible
