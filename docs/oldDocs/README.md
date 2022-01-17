# Excite-O-Meter
A Software Framework to Integrate Physiological Data in Extended Reality Applications

## Description

The plugin comprises three modules to:

1. Receive and process physiological activity data (**Physiological Signal Manager**)
2. Log and visualise it in real time, combine it with other sources (e.g., head movement) and create Unity events (**Real Time Monitor**)
3. Visualise and analyse it offline (**Offline Replay / Analysis Tool**).

## Architecture

![Architecture](https://drive.google.com/uc?export=view&id=1f0yHymVYLYZadV02IW4ibX114zbLco-6)

# Development

## Requirements

Developed using:

- Unity 2019.4.1 LTS (Plugin should be compatible with lower versions)
- Python 3.7 ()

## Folder structure

The repository is divided in three parts:
1. Unity folder that contains the main project with the development of the plugin.
2. Built applications that act as communication bridge between the sensor and Excite-O-Meter applications (available for Windows 10 and Android phones).
3. Python folder with intermediate analysis to assess quality of data acquisition and validity of physiological features calculated in Excite-O-meter plugins.

```
excite-o-meter
│   README.md
│   ...
│
└───Excite-O-Meter              /Unity Project/
│   │
│   └───Assets
│   │   │   ...
│   │   └───...
└───ExciteOMeter-Devices    
│   │
│   └───ExciteOMeter_0.x.x_Win10.zip      /Windows 10 App/
│   └───ExciteOMeter_1.x.x_Android.apk    /Android App/
│
└───ValidationDataAnalysis      /Python Project/
    │   files.py
    │   notebooks.ipynb
    └───...
```

# Excite-O-Meter - Devices

These set of applications are in charge of connecting to the physiological sensor and send data to the client applications using the Excite-O-Meter plugin from Unity.

The sensor can be connected through Bluetooth to either an Android mobile phone or a Windows 10 computer.

This communication to the sensor Polar H10 is done via Bluetooth LE, and data is sent through the local network using LabStreamingLayer ([LSL](https://github.com/sccn/labstreaminglayer))

### General Requirements

- The device that receives data from the sensor needs to be in the same local network (WiFi, LAN) than the device that visualizes the physiological signals.
- In case a Win10 PC is used to run ExciteOMeter-Devices, the receiving application in XR needs to run in a different PC on the same LAN network. (*Due to a restriction of loopback in Win10 Universal Windows App*)

**Testing LSL reception**

To test if there is a proper reception of the LSL data, you can run the application from the folder `ExciteOMeter-Devices/LSL-LabRecorder.zip` in a computer connected to the LAN. When pressing **Update**, a list of streams should appear in the list.

## How to wear the sensor Polar H10

Wear the Polar H10:
- Moisten the electrode area of the strap
- Wear the chest strap
- Attach the connector to activate the HR sensor

## ExciteOMeter-Devices | Android

The application is compatible with Android v6.0 or greater, API>=23.

### Installation

1. Send the .apk to the mobile phone either connecting it to the computer or direct download from the repository.
1. Search in the **Settings** of the phone the option to allow unknown sources ([help](https://www.androidcentral.com/unknown-sources)) from either the file explorer or Google Chrome (depending the app used to download the APK).
1. If the pop-up with Google Play Protect appears, click on **Install anyway**. This prompt is likely to be shown because data is sent through the network silently.
1. During execution, the app requires the following permissions: 

- *Access to location* (allow all the time to allow sending data even when the screen is off)
- *Turn on bluetooth* (to discover the sensor) It is necessary to have the Bluetooth ON, but **NOT** paired to the sensor, the app will automatically find the device when HR+RR or ECG signals are requested.
- *Turn on Localization* (same used for GPS, required by Android to pair with BLE devices). This  is not explicitly asked by the application, but unless this option is active, it won't gather data from the sensor.

### Execution

The application looks like the image below. It allows to send either HR+RR data or ECG, but not both signals at the same time.
![EoM_Android](docs/imgs/EoM_Android.jpg)

1. The first time that you open the app. You need to setup the Polar device's ID, which are the 8 letters located on top of the sensor. Every time you run the application, it will try to connect to the latest configured ID.
1. Click on either **Connect ECG** or **Connect HR/RR**, the application will connect through Bluetooth to the sensor and start collecting data. All the collected physiological values, are immediately forward to the network for Excite-O-Meter clients.

## ExciteOMeter-Devices | Windows 10 UWP

Requires *Minimum Windows 10, version 1803 (10.0; Build 171734)*

### Installation
1. Extract the .zip file in the folder ExciteOMeter-Devices
1. Right click the file `Add-AppDevPackage.ps1` and click on `Run with Powershell`. A new Powershell console will appear asking to proceed: Press `Y` to approve.
![Install](docs/imgs/install_win10_1.png)
1. If the computer is not setup as developer or the certificate is not trusted, it will prompt a second Powershell console asking for permission to execute these steps, as shown next, press `Y` to accept. If these two conditions are met beforehand, then the installation of the package should start immediately.
![Install](docs/imgs/install_win10_2.png)
1. Developer settings in Windows will open and you should choose `Developer mode` to allow the application install the respective certificate. Wait until Windows shows that the external packages have been installed. (*If concerned about security, once the application is installed, you can set back this feature to 'Microsoft Store or Sideloading', but any future update of the package will require to enable developer mode temporarily*)
![Install](docs/imgs/install_win10_3.png)
1. Installation of the package should proceed as shown.
![Install](docs/imgs/install_win10_4.png)
1. You find the application in the Windows Menu.
![Install](docs/imgs/install_win10_5.png)

### Execution

- In the first screen, `Enumerate Devices` to see the available Bluetooth LE devices in advertising mode.
- If the device has never been connected to the app, press `Pair` and it will prompt connection with Windows.
- When the device is paired it will enable the option `Continue`. If a device starting with `Polar H10 ` is detected, it will jump to configure their characteristics. Otherwise, the second screen will be opened to explore their services and characteristics.
- Adjust the toggles according to the variables that want to collect from the sensor (HR+RRi, ECG, ACC). Note that ECG and ACC cannot be enabled, the device stopped responding to requests when was busy sending both streamings.

![Setup Excite-O-Meter Windows](docs/imgs/ExciteOMeter_4.png)

## Excite-O-Meter | Unity plugin

### Installation

TBD

### Execution

TBD


## Available Cardiac Features

Based on this [paper](https://www.frontiersin.org/articles/10.3389/fpubh.2017.00258/full):

- **RMSSD:** The root mean square of successive differences between normal heartbeats (RMSSD) is obtained by first calculating each successive time difference between heartbeats in ms. Then, each of the values is squared and the result is averaged before the square root of the total is obtained. While the conventional minimum recording is 5 min, researchers have proposed ultra-short-term periods of 10 s (30), 30 s (31), and 60 s (36).
- **SDNN:** The standard deviation of the IBI of normal sinus beats (SDNN) is measured in ms. "Normal" means that abnormal beats, like ectopic beats (heartbeats that originate outside the right atrium’s sinoatrial node), have been removed. While the conventional short-term recording standard is 5 min (11), researchers have proposed ultra-short-term recording periods from 60 s (30) to 240 s (31). The related standard deviation of successive RR interval differences (SDSD) only represents short-term variability (9).

