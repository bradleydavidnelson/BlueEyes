# BlueEyes

BlueEyes is a software interface for Bluetooth Low-Energy sensors developed using the BLE113.

---
## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Contributors](#contributors)
- [License](#license)

---
## Installation

This software may be run in Visual Studio. The community version is [available for free](https://visualstudio.microsoft.com/downloads/). Compiled executables will be located in `BlueEyes/bin/Debug` or `BlueEyes/bin/Release` for debug and release versions respectively.

This software requires the Silicon Labs BLED112 USB dongle to communicate with the BLE113. This may be found at [Digikey](https://www.digikey.com/product-detail/en/silicon-labs/BLED112-V1/1446-1030-ND/4245505).

Required software drivers are currently available at [Silicon Labs](https://www.silabs.com/documents/login/software/Bluegiga-ble-1.5.0-137.exe). A Silicon Labs account is required for download, and may be created for free. This will include drivers for the BLED112.

---
## Usage

This project is organized using the Model-View-ViewModel (MVVM) structure. Views are elements that the user interacts with directly, such as windows and UI elements. ViewModels are associated with each View, and provide the behind-the-scenes logic for each view. Models contain data which may be used within ViewModels.

On start-up, two windows will be loaded: a main window and a debug window. The sequence to connect to a sensor is performed as follows: 

> 1. Select the BLED112 from the drop-down menu of USB elements (`Bluegiga Bluetooth Low Energy (COMX)`). ![](https://github.com/bradleydavidnelson/BlueEyes/blob/master/Media/port-select.gif?raw=true)
> 1. Push the `Attach` button to open serial communication with the selected USB component and begin scanning. Identified BLE devices will appear in the Discovered Device panel on the left.
> 1. Push the `Connect` button next to the desired device to establish a Bluetooth connection with it. The connected device will appear in the Connected Devices panel on the right.

---
## Contributors
- [Brad Nelson](https://github.com/bradleydavidnelson)

---
## License
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
