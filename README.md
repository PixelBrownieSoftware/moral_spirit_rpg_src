# Moral spirit source code
This is the source code for my non-combat RPG Moralspirit.

## Important note
This uses Magnum Foundation 2, (install it as a package https://github.com/PixelBrownieSoftware/Magnum-foundation-2-playground).
If you want to test the game, be sure start it on the Main Game scene.

## System (Src/system/)

### s_battlesyst
This is the heart of the battle system, keeping track of the playable characters and opponents. There is quite a bit of mess in this code and unused variables such as ```List<o_battleChar> sparable;```, since I made this a non-violent RPG very late into development.

#### Battle state
