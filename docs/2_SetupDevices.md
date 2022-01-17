**Setup Devices | Excite-O-Meter**

Table of Contents

- [Basic Setup](#basic-setup)
  - [Wearing the Sensor <a name="sensor"></a>](#wearing-the-sensor-)
  - [Collecting data](#collecting-data)
- [Excite-O-Meter | Devices > Android](#excite-o-meter--devices--android)
  - [Installation](#installation)
  - [Execution](#execution)
- [Excite-O-Meter | Devices > Windows 10 UWP](#excite-o-meter--devices--windows-10-uwp)
  - [Installation](#installation-1)
  - [Execution](#execution-1)
- [Continue with installation in Unity](#continue-with-installation-in-unity)

## Basic Setup

Excite-O-Meter has been developed and tested to be compatible with the following cardiac sensor:

1. Polar H10 chest strap ([link](https://support.polar.com/e_manuals/H10_HR_sensor/Polar_H10_user_manual_English/manual.pdf ))

<img src="./docsimgs/polarH10.jpg" width="30%">

The whole system includes the use of two different types of applications

1. **Excite-O-Meter - Devices:** Communication interface for the physiological sensor, collects data via Bluetooth and streams them to Unity. It is available as an independent *Win10 UWP app* or an *.apk* for Android.
2. **Excite-O-Meter - Unity Plugin:** .unitypackage that is included in your project to include physiological analysis with just few clicks.

### Wearing the Sensor <a name="sensor"></a>

First, you need to wear the Polar H10 chest strap:
- Moisten the electrode area of the strap
- Wear the chest strap
- Attach the connector to activate the HR sensor

### Collecting data

These set of applications are in charge of connecting to the physiological sensor via Bluetooth LE and send data to all applications running with the Excite-O-Meter plugin from Unity.

**Data from the sensor can be collected EITHER through Android mobile phone or a Windows 10 computer**. Some important notes about data communication are:

- The device that receives data from the sensor needs to be in the same local network (WiFi, LAN) than the device that runs your XR application in Unity.

- Due to a loopback restriction in Win10 UWP, in case you use *Excite-O-Meter Devices* on Win10 to collect data, **IT MUST BE A DIFFERENT PC** than the PC running your XR application, but still on the same LAN network.

- When creating a **built of your app in Unity** for Windows, it will prompt a Windows Security alert the first time you open your `.exe`. This message is shown because the Excite-O-Meter uses internally socket communication to send data between the sensor and Unity. If you do not approve this message, your application will not be able to receive data from the Excite-O-Meter. This issue can be fixed creating an explicit exception for incoming messages in Windows Firewall.

## Excite-O-Meter | Devices > Android

The application is compatible with Android v6.0 or greater, API>=23.

### Installation

1. Send the `.apk` to the mobile phone either connecting it to the computer or direct download from the Excite-O-Meter [website](http://exciteometer.eu/).
1. Search in the **Settings** of the phone the option to allow unknown sources ([help](https://www.androidcentral.com/unknown-sources)) from either the file explorer or Google Chrome (depending the app used to download the APK).
1. If the pop-up with Google Play Protect appears, click on **Install anyway**. This prompt is likely to be shown because physiological data is sent through the network silently.
1. During execution, the app requires the following permissions: 

- *Turn on Localization* (same used for GPS, required by Android to pair with BLE devices). This  is **NOT explicitly requested by the application, but mandatory** to gather data from the sensor.
- *Access to location* (allow all the time to allow sending data even when the screen is off)
- *Turn on bluetooth* (to discover the sensor) It is necessary to have the Bluetooth ON, but **NOT** paired to the sensor, the app will automatically find the device when HR+RR or ECG signals are requested.

### Execution

The application looks like the image below. It allows to send either HR+RR data or ECG, but not both signals at the same time.

<img src="./docsimgs/EoM_Android.jpg" width="100%">

1. The first time that you open the app. You need to setup the Polar device's ID, which are the 8 letters located on top of the sensor. Every time you run the application, it will try to connect to the latest configured ID.
1. Click on **Connect HR/RR**, the application will connect through Bluetooth to the sensor and start collecting data. All the collected physiological values, are immediately forwarded to the network for Excite-O-Meter clients.

## Excite-O-Meter | Devices > Windows 10 UWP

Requires *Minimum Windows 10, version 1803 (10.0; Build 171734)* 

### Installation


1. Extract the .zip file of the Excite-O-Meter-Devices for Win10 from the project's [website](http://exciteometer.eu/)

2. Right click the file `Add-AppDevPackage.ps1` and click on `Run with Powershell`. A new Powershell console will appear asking to proceed: Press `Y` to approve.

**NOTE:** *When installing the UWP application, it verifies that the Windows developer's certificate is still valid. It only lasts one year. If there are problems with this certificate when installing, please post an issue on GitHub and we will generate an updated certificate for the installation. See [this issue for reference](https://github.com/luiseduve/exciteometer/issues/3)*.

<img src="./docsimgs/install_win10_1.png" width="80%">

1. If the computer is not setup as developer or the certificate is not trusted, it will prompt a second Powershell console asking for permission to execute these steps, as shown next, press `Y` to accept. If these two conditions are met beforehand, then the installation of the package should start immediately.

<img src="./docsimgs/install_win10_2.png" width="80%">

4. Developer settings in Windows will open and you should choose `Developer mode` to allow the application install the respective certificate. Wait until Windows shows that the external packages have been installed. (*If concerned about security, once the application is installed, you can set back this feature to 'Microsoft Store or Sideloading', but any future update of the package will require to enable developer mode temporarily*)

<img src="./docsimgs/install_win10_3.png" width="90%">

1. Installation of the package should proceed as shown.

<img src="./docsimgs/install_win10_4.png" width="80%">

1. You find the application in the Windows Menu.

<img src="./docsimgs/install_win10_5.png" width="60%">

### Execution

- In the first screen, `Enumerate Devices` to see the available Bluetooth LE devices in advertising mode.
- If the device has never been connected to the app, press `Pair` and it will prompt connection with Windows.
- When the device is paired it will enable the option `Continue`. If a device starting with `Polar H10 ` is detected, it will jump to configure their characteristics. Otherwise, the second screen will be opened to explore their services and characteristics.
- Adjust the toggles according to the variables that want to collect from the sensor (HR+RRi, ECG, ACC). Note that ECG and ACC cannot be simultaneously enabled, the device stopped responding to requests when busy sending both streamings.

<img src="./docsimgs/ExciteOMeter_4.png" width="90%">

## Continue with installation in Unity

After this step, you can continue setting up the package in Unity, as described in the [user manual documentation](./1_UserManual.md)