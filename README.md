# Toll's ENB Manager

A comprehensive and highly customizable ENB manager that allows the user to quickly and safely switch between various ENB presets.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Future](#future)
- [License](#license)

## Features

- Multiple games supported
- Unlimited amounts of presets
- Game launcher
- Screenshot collector

## Installation

1. Download the single executable file and save it anywhere you like
2. Run the executable (.exe) (Windows might prevent the app from running the first time)
3. Select the games to manage and continue

## Usage

### Managing games

The application scans the system registry for installed games the first time the app is opened. At this point, the user can choose what games to manage. If a game it not found automatically, it is possible to manually browse the game executable.
At any point in time, the user can revisit this function via the menu to either manage a new game or unmanage and old game, providing a clean and up-to-date application.

NOTE: Unmanaging a previously managed game will remove all presets and settings for that particular game.

### Configuration

A wide array of settings are available and can be accessed via the 'Settings' menu. Including themes, colors, shortcuts, logging and application behavior.

### Binaries

ENB binares are a fundamental part of the application and its usage. The user can take 2 different approaches in this regard:

1. The user allows the application to automatically manage the binaries. This includes copying and removing the binaries from the game directory when needed. This is the default behavior.
2. The user is responsible for managing binaries. Switching between or deactivating presets will not affect the binaries.

### Dashboard

The Dashboard acts as a control center for a particular game. In this module, the user is greeted with messages, warnings and errors that communicates the current state of the game. For example, the user will be notified if binaries are missing. The Dashboard also provides information about the number of presets, the currently active preset and more.

### Presets

A preset within the application is a set of files and folders that would otherwise be copied directly into the game directory. Each preset is tied to a specific game and only exists for that game. Binaries should NEVER be a part of an ENB preset. They are either managed by the application or by the user. The idea is that binaries are static and unaffected by the chosen preset. As a result, only one file version of the binaries can be managed at once.

#### Add

The most basic approach to adding a new preset is to manually browse for a folder that acts as the parent for the various files. Typically, the root folder would be the same folder as in the file system. Once a folder has been chosen, the file structure is presented in a TreeView where it's possible to choose a new root folder. It is also possible to delete files and folders.

Another approach to add a preset is to save the current one if present. The idea is that whatever preset exists in the game directory, will be saved and managed by the application. The files included in the preset are based on a set of keywords. For example, files and folders containing the keyword 'enb'.

NOTE: This feature is, as of yet, experimental as it requires further testing and feedback.

#### Delete

Deleting a preset is simple - Click delete and make sure you deleted the correct preset. Depending on configuration, screenshots are also deleted in this process.

NOTE: Only inactive presets can be deleted.

#### Rename

Renaming a preset can be performed at any time.

#### Activating/Deactivating

Presets can be activated or deactivated with a simple switch. Activating a preset means that the files are copied to the game directory. If binaries are managed by the application, they are also injected. Obviously, only one preset can be active at a time, which means that activating one preset will deactivate another. It's also possible to deactivate the only active preset and thus not having any preset active.

Switching between presets is supported in-game which lets the user change presets without exiting the game. Simply activate another preset, and load the new configuration via the ENB menu.

#### Update

It's not uncommon that an ENB preset is adjusted and changed in-game via the ENB menu. This means that the active preset is no longer up-to-date. The user is notified about the divergence and is prompted to update the preset according to the changes and adjustments.

### Screenshots

If enabled, the application will monitor and automatically collect screenshots taken in-game and save them according to the active preset. After enabling screenshot collection, you may minimize the application and it will work quietly in the background.
Depending on configuration, screenshots might also be collected without an active preset.

NOTE: Screenshots are collected from the game directory.

## Future

- Multi-language support
- Nexus Mods API integration
- Export presets
- More games
- Preset editor & comparer
- Better preset detection and preset creation
- Screenshot file type configuration

## License

Copyright (C) 2020  André Toll 

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
