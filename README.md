# Epic Edit

Official site: http://epicedit.stifu.fr

Epic Edit is a track editor for Super Mario Kart (Super Nintendo). It requires a Super Mario Kart ROM, which is not included. Games modified with Epic Edit can then be played on a real SNES or with SNES emulators.

Epic Edit makes it easy to:
  - Modify track tiles and overlay tiles
  - Move track lap line and driver start positions
  - Move and modify track objects
  - Modify the AI (the path followed by computer-controlled drivers)
  - Modify graphics and colors
  - Modify track and driver names
  - Modify game settings (item probabilities, attributed score points...)
  - Import/export all kinds of data
  - Directly compress/decompress data into/from the ROM (advanced users)

This application is written in C# and uses Windows.Forms. It is distributed under the GPL. It requires the .NET Framework 2.0 or above on Windows, and also works with Mono on Linux. It works on Mac too, using Mono with an X11 server installed, by passing a command line argument, this way:
> MONO_MWF_MAC_FORCE_X11=1 mono EpicEdit.exe
