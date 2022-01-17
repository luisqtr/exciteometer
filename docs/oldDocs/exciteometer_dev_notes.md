# <img src="logo/Logo_lowres.png" width="5%"> Excite-O-Meter / Development Journal

This document works as a journal to keep track of important aspects resulting from the development of the plugin Excite-O-Meter for Unity.

---
---

# Notes dev Excite-O-Meter plugin for Unity

The demo scene uses the following external packages:

- LSL (downloaded from github repo and modified for Unity 2019.4)
- TextMeshPro (Included in latest versions of Unity or downloadable through Asset store)

## Overview

### Prefabs

- `ExciteOMeter_Manager` is mandatory to handle the LSL communication and triggering events to other scripts. 

- `ExciteOMeter_SignalEmulator` can be used to simulate incoming data from Physiological sensor (HR and RRI) in case the sensor is not available.


### Main Events

The file `EoM_Events` wraps all the events that are sent from the Excite-O-Meter.

*How to subscribe to events*


The file `ExampleGetExciteOMeterData.cs` includes examples of reception of events for incoming data, status of LSL inlets, and logging status. Note that they should be done in the *OnEnable()* and *OnDisable()* methods of each MonoBehaviour.

- The script `EoM_Base_CardiacFeatures.cs` shows how to subscribe to incoming data. The event sends the type of incoming data in addition to the float value.
- The children of the GameObject `InletFlags` contain the script `ReactInletUI.cs` that shows how to subscribe to changes in LSL connection.
- GameObject  '`Logger` contains `LogMessageUI.cs` to subscribe to changes in Log status.

### Logs

The main folder is where the executable is located `.exe`.

- The file `./StreamingAssets/configFile.json` contains variables that are kept between sessions.
- The folder `./LogFiles_ExciteOMeter/*` contains one folder per log session, each session contains multiple files with the recorded data (sensors, actions, features, etc.)


---
---

# ExciteOMeter-Devices | Android

The application is compatible with Android v6.0 or greater, API>=23.

![EoM_Android](../docs/imgs/EoM_Android.jpg)

### LSL Libraries

The APK contains native libraries **liblsl.so** and **libjnidispatch.so**, compiled for architecture `armv8-a`, this is compatible with the CPU of the Snapdragon 835, embedded in the Oculus Quest.

These library is compiled with `MinSDK=23` and `TargetSDK=26`. To keep in mind when using these libraries to compile Quest apps directly from Unity.

---
---

# ExciteOMeter-Devices | Windows 10 UWP

Requires *Minimum Windows 10, version 1803 (10.0; Build 171734)*

![Setup Excite-O-Meter Windows](imgs/ExciteOMeter_4.png)

### Manage trusted certificates

To check or delete installed certificates. Search and run the Windows application `certlm.msc`, the trusted certificate should be in the folder `Trusted People > Certificates`. The certificate for the ExciteOMeter application starts with *7703E1C0* and expires on 2021-07-16.
Once the certificate is expired, a new compilation of the Visual Studio project is needed with a new certificate, and restart the installation process.

Example of certificate
![Trusted Certificate](imgs/certificate.png)

---
`Note:` Excite-O-Meter application was uploaded to the Windows Store:
https://www.microsoft.com/store/apps/9PFMNFQJB99Q. However, when LSL was incorporated, it was not approved by the Windows Apps Certification Kit because uses API that are not available for UWP. *Possible workarund for the future:* Send data through UDP/TCP using UWP libraries (info [here](https://docs.microsoft.com/en-us/windows/uwp/networking/sockets)).


---
---

# LabStreamingLayer Specifications for ExciteOMeter-Devices

## LSL Outlets for Polar H10

| Name | Type | Channels | Nominal Rate | Format | Source ID |
| --- | --- | --- | --- | --- | --- | --- | 
| "HeartRate" | "ExciteOMeter" | 1 | LSL.IRREGULAR_RATE | int16 | \<deviceID\> |
| "RRinterval" | "ExciteOMeter" | 1 | LSL.IRREGULAR_RATE | float32 | \<deviceID\> |
| "RawECG" | "ExciteOMeter" | 1 | LSL.IRREGULAR_RATE | int32 | \<deviceID\> |
| "RawACC" | "ExciteOMeter" | 3 | LSL.IRREGULAR_RATE | int32 | \<deviceID\> |

---
---

# Bluetooth LE Specifications for Polar H10

According to the [Polar Data Specification](https://github.com/polarofficial/polar-ble-sdk/blob/master/technical_documentation/Polar_Measurement_Data_Specification.pdf) and user's manual, the sensor Polar H10 is a *Bluetooth Smart* device that acts as a low-energy-only device and requires another *Bluetooth Smart Ready* device to function (e.g. smartphone, computer, smartwatch).

The main steps to communicate between Polar H10 and another device through Bluetooth LE are:

1. Request *pairing* between both devices
1. Request available *services* from Polar H10
1. Request specific *characteristics* from each service
1. Subscribe to notification when the value of the characteristic changes.

## Pairing

Consists on the process of looking for advertisement messages from the Polar H10, which act as *server* capable to emit information. The device starts advertising when the cheststrap is worn.

## Service Discovery

The behavior of the device is encapsulated in [GATT Services](https://www.bluetooth.com/specifications/gatt/services/). A lot of them are already standardized (like the service for Heart Rate), but others are customized by each manufacturer (in the Polar H10, the streaming of ECG or accelerometer data).

## Characteristics Types

Each service contains a set of GATT Characteristics, which are the attributes of the data that the service can emit. Some actions can be performed on each characteristic, such as Write, Read, Notify, Indicate.

For instance, the GATT characteristic for *Heart Rate Measurement* has a [standard]((https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Characteristics/org.bluetooth.characteristic.heart_rate_measurement.xml)) from Bluetooth which allows to know how to access the HR value, Energy Expended, and RR-interval.

However, the characteristic to stream data from the Polar H10 sensor (i.e. ECG, accelerometer) is not standardized and is described in the official [Polar Data Specification](https://github.com/polarofficial/polar-ble-sdk/blob/master/technical_documentation/Polar_Measurement_Data_Specification.pdf).

## Subscription

When the characteristic has been enabled, notification can be enabled through specific exchange of ATT packages between the server (Polar H10) and the client (computer, smartphone).

# Summary of Data Specification for Polar H10

The UUID for the services and characteristics are:

``` c#
public Guid BATTERY_SERVICE              = new Guid("0000180f-0000-1000-8000-00805f9b34fb");
public Guid BATTERY_LEVEL_CHARACTERISTIC = new Guid("00002a19-0000-1000-8000-00805f9b34fb");

// Heart Rate
public Guid HR_SERVICE              = new Guid("0000180D-0000-1000-8000-00805f9b34fb");
public Guid BODY_SENSOR_LOCATION    = new Guid("00002a38-0000-1000-8000-00805f9b34fb");
public Guid HR_MEASUREMENT          = new Guid("00002a37-0000-1000-8000-00805f9b34fb");

// Streaming Measurement
public Guid PMD_SERVICE     = new Guid("FB005C80-02E7-F387-1CAD-8ACD2D8DF0C8");
public Guid PMD_DATA        = new Guid("FB005C82-02E7-F387-1CAD-8ACD2D8DF0C8");
public Guid PMD_CP          = new Guid("FB005C81-02E7-F387-1CAD-8ACD2D8DF0C8");
```

The service *Polar Measurement Data* (`PMD`) is in charge of streaming and is not standardized by bluetooth but customized by the manufacturer. The process to activate streaming of data (ECG or accelerometer) from Polar H10 is:

1. Enable `Indicate` to characteristic `PMD_CP`
2. Enable `Notify` to charactersitic `PMD_DATA`
3. Fetch current streaming settings (Sampling rate, resolution, etc.):
    - ECG: Write to `PMD_CP` the value `0x0100`
    - ACC: Write to `PMD_CP` the value `0x0102`
4. Start streaming of data:
    - ECG: Write to `PMD_CP` the value `0x02000001820001010E00`
    - ACC: Write to `PMD_CP` the value `0x02020001C8000101100002010800`
5. Stop streaming of data:
    - ECG: Write to `PMD_CP` the value `0x0300`
    - ACC: Write to `PMD_CP` the value `0x0302`


## Additional Dev instructions for BLE

For more technical information about Bluetooth LE refer to [dev notes from polar](./instructions_dev_PolarH10.md) and the book *Getting Started with Bluetooth Low Energy - Tools and Techniques for Low-Power Networking. Kevin Townsend, Carles Cufi, Akiba, Robert Davidson. O'Reilly Media (2014)*


---
---

# Python Data Validation

The file `simulate_lsl_outlets.py` simulates a local LSL outlet that sends HR and RRi every second. To run it, it needs the Python package `pylsl`.

[Neurokit2](https://neurokit2.readthedocs.io/en/latest/examples/hrv.html) was used to compare the acquisition rate and validity of HRV features from Python and from Unity. The PDF with the results of the validation can be found in [this file](../validation_feature_calculation.pdf)



---
---

# Known issues

## Problems in ExciteOMeter-Devices | Windows 10 UWP

### Network isolation for UWP on Windows 10

Due to network isolation issues from Windows 10 (info for [general info](https://docs.microsoft.com/en-us/previous-versions/windows/apps/hh770532(v=win.10)#network-isolation-and-loopback), [sockets](https://docs.microsoft.com/en-us/windows/uwp/networking/sockets), [enable loopback between apps](https://docs.microsoft.com/en-us/windows/uwp/communication/interprocess-communication#loopback)), it cannot be used for communication between two different apps running on the **same machine**. However, LSL allows reception in other computers on the network.

*Solution*: 
1. Run the UWP App *Excite-O-Meter* in one computer.
2. Run the receiving XR application in another computer connected to the same local network.
4. Accept the firewall protection notification if it pops-up.
3. Verify that the internet network is marked as *Private/Local* and NOT *Public*

Powershell command to verify which apps are exempted from Network Isolation:

`CheckNetIsolation LoopbackExempt -s` [Docs](https://docs.microsoft.com/en-us/previous-versions/windows/apps/hh780593(v=win.10))

### Other projects in UWP using LSL

Another project ([BlueMuse](https://github.com/kowalej/BlueMuse/tree/master/LSLBridge)) used another workaround, where the main UWP launches a Win32 app that has all the networking priviledges.

Similar projects in Unity: https://github.com/ElliotMebane/HeartRateMonitor_Win_And_Hololens_SampleProject : It is from 2017 and the demo did not discover the service from the Polar H10.

BT Specifications of Polar H10: https://launchstudio.bluetooth.com/ListingDetails/81657

UWP in Unity cannot be used because it would not work when compiling for desktop (Oculus, HTC, etc.)

Compatibility between Unity and .NET 4.x: https://docs.microsoft.com/en-us/visualstudio/cross-platform/unity-scripting-upgrade?view=vs-2019

### TODO

- Update app in Windows Store with UDP/TCP communication instead of LSL.
- Look for exception of Network Isolation so you can get LSL packages in the same computer.


