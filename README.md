# Whimsical Weather
Adds a new pyrovision-inspired weather type to Lethal Company.

Made solo over the course of a week for the [2023 Lethal Company Modjam](https://itch.io/jam/2023-lethal-company-modjam).

## Features
When in "Whimsical Vision", this mod has many visual changes, many of which have config options:
- Custom postprocessing shader
- Rainbow UI and text (with configurable speed)
- Disabled blood/gore
- Changes to the following items: Walkie Talkie, 7Ball, ZapGun, Magnifying Glass, Apparatus, Shovel, Stop Sign, Yield Sign
- Changes to the following ship objects: Manual, Sticky Note, Teleporter, Inverse Teleporter, Ship Horn
- Changes to the following enemies: Eyeless Dogs, Jester, Hygrodere
- Rainbow item dropship lights
- Pitched up voice chat (with configurable pitch)
- A laughing sound effect when the game's fear effect plays

All changes are completely reversible, and will exclusively effect your game when in Whimsical Vision.
## Credits
This mod makes heavy use of the following resources to make its OKHSL-based hue shifting possible:
- [Björn Ottosson's original blog post on OKHSL](https://bottosson.github.io/posts/colorpicker/) (MIT license)
- [AcerolaFX](https://github.com/GarrettGunnell/AcerolaFX) by GarrettGunnell (MIT license)
- [Unicolour](https://github.com/waacton/Unicolour) by waacton (MIT license)

To add custom weather to the game, this repo uses [LethalLib](https://github.com/EvaisaDev/LethalLib), which is licensed under MIT.