# ScriptPush_RetroBat
![image](https://github.com/user-attachments/assets/964e692c-91b7-4322-b848-06371ab2d338)


Donwload first pack : https://archive.org/details/retrobat-script-demulshooter-packes-custom-nixx.-7z

# ScriptPush

A lightweight Windows application designed to manage and execute DemulShooter for RetroBat and AutoHotkey scripts "optional", providing automated game control and configuration.

## Features

- **Script Execution Management**
  - Automated script execution based on game launch
  - Support for multiple script types:
    - Start scripts
    - Remap scripts
    - End scripts
  - Process monitoring and script synchronization

- **Integration Support**
  - RetroBat
  - DemulShooter integration
  - System-wide script management
  - Automatic process detection

- **Configuration**
  - Simple configuration file setup
  - Custom script path support
  - Process name monitoring
  - Error handling and logging

## Requirements

- Windows 10/11
- .NET 8.0 or higher
- AutoHotkey v2 (for script execution)

## Installation

1. Download the latest release from the [releases page](https://github.com/Aynshe/ScriptPush_RetroBat/releases)
2. Extract the archive to your desired location
3. Run `ScriptPush.exe`

## Usage

1. **Basic Setup**
   - Place your AutoHotkey scripts in the appropriate system folders
   - Configure process names to monitor
   - Start the application to begin script monitoring

2. **Script Organization**
   - Place start scripts in the `.serial-send/.edit/Start.ahk` folder
   - Place remap scripts in the `.serial-send/.edit/remap.ahk` folder
   - Place end scripts in the `.serial-send/.edit/End.ahk` folder

3. **Operation**
   - The application runs in the background
   - Scripts are executed automatically based on process detection
   - Check the application log for execution status

## Contributing

Contributions are welcome! Please feel free to submit pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- RetroBat community
- DemulShooter by [argonlefou](https://github.com/argonlefou/DemulShooter)
- AutoHotkey community

sources :
#https://github.com/Aynshe/ScriptPush_RetroBat  /  #https://github.com/Aynshe/Edit_LsTRoms

#https://retrobatofficial.itch.io/retrobat / https://social.retrobat.org/discord
#https://github.com/argonlefou/DemulShooter/wiki/Outputs
#https://github.com/argonlefou/DemulShooter/wiki/Usage#supported-options-
#https://github.com/argonlefou/DemulShooter/wiki/Usage#supported-target-
#https://dragonking.arcadecontrols.com/static.php?page=aboutmamehooker
#https://pistolasdeluzpedia.miraheze.org/wiki/Configuraci%C3%B3n_Mamehooker_y_GUN4IR
#https://github.com/AutoHotkey/AutoHotkey
#https://github.com/AutoHotkey/Ahk2Exe
#https://github.com/nixxou

#https://www.gun4ir.com  /  #https://discord.gg/HJyfYja
