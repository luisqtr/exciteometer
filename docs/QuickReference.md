# Quick Reference Guide

**Project Website:** [http://exciteometer.eu/](http://exciteometer.eu/)

The Excite-O-Meter is a software framework that allows developers to integrate physiological data in Extended Reality (XR) applications to calculate the level of 'excitement' that your application might be causing on participants. It is particularly suitable for researchers that conduct scientific experiments in XR with Unity but also for hobbists that are developing their own games and want to measure their own reaction to the game. The main **functionalities** that the Excite-O-Meter provides are:

- Scientifically validated set of metrics to estimate Excite-O-Meter level
- Easy integration of the tool by adding two prefabs to an existing Unity project.
- Instant access to real-time physiological data collection and offline data visualization module.
- Organize each participant in different sessions that are visualized in the Excite-O-Meter offline analysis tool, or get the CSV files with the data for your own data analysis.

### Excite-O-Meter Devices

Excite-O-Meter is compatible with Polar H10 chest strap ([link](https://www.polar.com/us-en/products/accessories/h10_heart_rate_sensor)). This module is the communication interface for the physiological sensor, collects data via Bluetooth and streams them to Unity. It is available for two platforms:

1. Option A: Download the `.apk` and install it on an Android phone.
1. OR option B: Download the `.zip` and install the Windows 10  application with Powershell, it needs to be a different computer than the one running the XR application.

### Unity Plugin

Include the Excite-O-Meter to your project to enable physiological analysis with just few clicks. Compatible with Unity 2019.4 or above.

1. Import the `UnityUIExtensions-2019.4.unitypackage` or download it from https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads
1. Import the `ExciteOMeter.unitypackage` to your project.
1. Drag and drop both prefabs `ExciteOMeter_Manager` and `ExciteOMeter_UI_OnlineAnalysis` to the ROOT of your hierarchy.
1. Open your *Build Settings* and add the scene `ExciteOMeter_OfflineAnalysis` the panel *Scenes in Build*. This is the scene that allows to conduct data visualization.

### Build your application

Before building your application, delete the file `StreamingAsets/config.json` to generate a new configuration file the first time that the application is executed in the final build.
Every recorded session is inside the main folder named `LogFiles_ExciteOMeter`, found at the same level than your main `.exe` file.