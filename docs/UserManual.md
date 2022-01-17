# User Manual | Excite-O-Meter 

Table of Contents

- [User Manual | Excite-O-Meter](#user-manual--excite-o-meter)
  - [How to use?](#how-to-use)
    - [Dependencies](#dependencies)
    - [Instructions](#instructions)
    - [Example](#example)
  - [Excitement Metric](#excitement-metric)
---

## How to use?

### Dependencies

Any Unity project using the `EoM` must also import [Text Mesh PRO](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html) (TMPro). Usually a pop-up message asks to import TMPro essentials directly in Unity when it is needed.

The `EoM` ships modified versions of two existing libraries that are required for the project:

1. [Lab Streaming Layer](https://github.com/sccn/labstreaminglayer): A great C# API that unifies measurement of time series data through the network. We specifically modified the Unity version available from [LSL4Unity](https://github.com/xfleckx/LSL4Unity).
2. [UI Extensions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/): This fantastic package facilitates the visualization of time series in the Unity UI.

If you want to collect cardiac data from the chest strap sensor [Polar H10](https://www.polar.com/us-en/products/accessories/h10_heart_rate_sensor), it is necessary to download the middleware software called `Excite-O-Meter | Devices`. It is available for Windows/Android and more information is [available in this document]().

### Instructions

There are two ways to include the `EoM` in your existing Unity project depending on whether your project already uses [GIT](https://git-scm.com/) or not. 

1) If you are already using Git as a version control system for your Unity project. Access your `Assets` directory from the terminal, and add the `EoM` as a submodule:
```cmd
>> cd Assets/
>> git submodule add https://github.com/luiseduve/exciteometer.git
```
2) If your Unity project is not using Git, download the `.unitypackage` from `PROVIDE LINK!!!!!`.

### Example

The package includes an example scene `Scenes\Example_withURP_NewInputSystem.unity`. Read the details in the [user manual](docs/UserManual.md).

The last version of the example scene was tested on:
- Unity 2020.3.19f1
- Universal Rendering Pipeline - [URP v10.6.0](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@10.6/manual/)
- Using the New Unity [Input System v1.0.2](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html).
- Project configured as standalone, and using OpenXR as plug-in provider.


## Excitement Metric

to calculate the level of 'excitement' that your application might be causing on participants. It is particularly suitable for researchers that conduct scientific experiments in XR with Unity but also for hobbists that are developing their own games and want to measure their own reaction to the game. The main **functionalities** that the Excite-O-Meter provides are:

- Scientifically validated set of metrics to estimate Excite-O-Meter level
- Easy integration of the tool by adding two prefabs to an existing Unity project.
- Instant access to real-time physiological data collection and offline data visualization module.
- Organize each participant in different sessions that are visualized in the Excite-O-Meter offline analysis tool, or get the CSV files with the data for your own data analysis.
