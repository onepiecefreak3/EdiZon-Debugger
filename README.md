# EdiZon-Debugger
This tool should provide help in debugging JSON configs and LUA scripts for use in EdiZon, a Switch Save Editor.

![alt text](https://i.imgur.com/YGUPGUZ.png)

## Technical details
You need to have .NET 4.7.2 installed to use this tool.<br>
The lua interpretation utilizes vJine.Lua 0.1.0.13, which is based on Lua 5.3. There were no changes made to its source code and is just used to load up and execute the lua scripts in the "script" folder.

## Setup
To get the debugger set up, you need to either start the application once or create all needed folders yourself.<br>
You need 4 folders in the same folder as the application:
- config
- script
- save
- lib

The config folder holds all JSON configs. Just put them in there and open them up with the debugger.<br>
The script folder holds all LUA scripts. Like in EdiZon the lua script is determined by the "filetype" field in the JSON.<br>
The save folder holds all saves. The save folder represents the root directory of a games save location. Meaning that if a save is buried down in more folders, this folder structure is to be built up in the save folder to get it working. Else this also works like in EdiZon. The debugger chooses the save file automatically based on "saveFilePaths" and "files" in the JSON. If more than one file was found a dialog will open, asking you which save file to use.<br>
The lib folder holds all external lua libraries used. So if your LUA script utilizes external libraries, place them there so the debugger can use them.

## Usage
Click on "File" > "Open config" to choose a JSON config to load.<br>
After successful loading, the left list will be filled with all found categories. Clicking one will load up the items in that category.

## Debugging
If an item produces an error, its value will be shown as "???".<br>
All items that produce errors, because they were out of range of the given widget parameters, are written in the right "Errors" box.<br>
Those errors will tell you which items had which errors, and help you pinpoint and resolving it.

## Extracting a save
Beside the functionality of pinpointing errors, one can also extract a save file modified with "File" > "Extract Edited Save". It's basically an EdiZon on PC, so you can also edit every save file on PC with it, if a JSON config and LUA script is given.<br>
But who wants that if EdiZon itself works directly on Switch and makes things so muc more comfortable ;)
