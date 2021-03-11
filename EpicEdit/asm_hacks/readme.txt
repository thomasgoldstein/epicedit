All these ASM hacks have been written by smkdan for Epic Edit.
The hacks are supposed to be applied on a Super Mario Kart ROM using xkas.exe (command example: "xkas hack.asm smk.sfc").
The last character of each asm file name specifies the region of the ROM the hack should be applied on (Eur, Jap, US).
Epic Edit automatically applies the binary result of these hacks when you save a ROM with it, which is done in the EpicEdit.Rom.Game.SaveDataToBuffer method.

Folders:
- "epic racers player select"
This particular hack is not used in Epic Edit but in Epic Racers (Super Mario Kart hack).
It heavily modifies the player select screen to include portraits, driver stats, a glowing selection indicator, etc.
It requires bass.exe rather than xkas.exe.

- "independent battle tracks"
Makes it possible to have independant driver starting positions for each battle track.
In the original game, Battle Course 1, 2 and 4 share the same driver starting positions, while Battle Course 3 has a specific set of driver starting positions.
Method: EpicEdit.Rom.Game.SaveBattleStartPositions

- "object properties"
Makes all tracks have independent object zones (Koopa Beach 1 and 2 share the same one in the original game).
Makes it possible to change the object type of each track (regular or GV pillar).
Removes Donut Plains pipe / mole hacks with something cleaner and reusable.
Makes it possible to mix the properties of each object (tileset, interaction, routine).
Method: EpicEdit.Rom.Game.SaveObjectData

- "object color palettes"
Makes it possible to define the object palette used for each track.
Makes it possible to add Rainbow Road Thwomp-like flashing (palette cycler) for each track.
Method: EpicEdit.Rom.Game.SaveObjectData

- "tileset types"
Gives a 256-byte behavior table for each theme in uncompressed form. It helps the load times a little bit, and allows theme-specific tile genre values for shared tiles.
Also reimplements the theme-specific behavior of the Browser Castle jump bars that slows you down, to make it reusable.
The "track" versions of the asm files give a 256-byte behavior table for each track rather than theme (feature not used in Epic Edit).
Method: EpicEdit.Rom.Game.SaveTileGenres
