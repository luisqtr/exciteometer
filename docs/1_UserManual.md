**User Manual | Excite-O-Meter**

*Table of Contents*

- [Excite-O-Meter | Unity plugin](#excite-o-meter--unity-plugin)
  - [Including Excite-O-Meter in your XR project from Unity Editor](#including-excite-o-meter-in-your-xr-project-from-unity-editor)
    - [UI: Online Recorder <a name="app2a"></a>](#ui-online-recorder-)
    - [UI: Offline Analysis <a name="app2b"></a>](#ui-offline-analysis-)
    - [Accessing Excite-O-Meter Logs](#accessing-excite-o-meter-logs)
    - [Advanced configurations](#advanced-configurations)
- [Scientific Disclaimer <a name="disclaimer"></a>](#scientific-disclaimer-)
  - [Excitement Metric](#excitement-metric)
---


## Description

The Excite-O-Meter `EoM` is a software framework that allows developers to integrate physiological data and movement analysis in any Extended Reality (XR) applications developed in Unity. The vision of the tool is to facilitate the data collection of bodily data that allows to automatically estimate the *'excitement level'* that an XR scenario induces on users. The main functionalities that the `EoM` gives to your XR application are:

- Easy integration (no code) with existing standalone Unity projects. A bit more than dragging and dropping few prefabs into an existing scene.
- Instant access to real-time physiological data collection, movement trajectories, and offline data visualization module.
- Transparent organization of trials in 'sessions' that can be visualized with the integrated *offline data visualizer* or using the stored CSV files for your own post-hoc analysis.


## Configuration

### Dependencies

Any Unity project using the `EoM` must also import separately the package [Text Mesh PRO](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html) (TMPro). If your project does not use TMPro yet, Unity will prompt a message when you drag and drop the `EoM` in your scene.

The `EoM` package **already contains** modified versions of two prerequired libraries (*no need to install them manually*):

1. [Lab Streaming Layer](https://github.com/sccn/labstreaminglayer): A great C# API that simplifies measurement of time series data through the network. Specifically, we modified the Unity version available from [LSL4Unity](https://github.com/xfleckx/LSL4Unity).
2. [UI Extensions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/): This fantastic package makes possible easy time series visualization in the Unity UI.

### Setup external sensors

**The `EoM` can be utilized without external sensors**, we have included a prefab that simulates cardiac activity so you can try the package without needing to setup additional software. However, the main advantage of the `EoM` is the simplicity to collect bodily data in Unity. Currently compatible with the chest strap sensor [Polar H10](https://www.polar.com/us-en/products/accessories/h10_heart_rate_sensor).

To use the external wearable, it is necessary to download the middleware software called `Excite-O-Meter | Devices`. It works as a bridge between the communication protocol from the device (e.g., Bluetooth Low Energy) and the LSL data captured in Unity.

The `Excite-O-Meter | Devices` is available for Windows or Android. Please follow the instructions [available in this document](./2_SetupDevices.md) if you want to try the `EoM` with real cardiac data in Unity.

### Import the package into Unity

There are two ways to include the `EoM` in your existing Unity project depending on whether your project already uses [GIT](https://git-scm.com/) or not. 

1) If you are already using Git as a version control system for your Unity project. Access your `Assets` directory from the terminal, and add the `EoM` as a submodule:
```bash
$ cd Assets/
$ git submodule add https://github.com/luiseduve/exciteometer.git
$ git submodule update --init
```
2) If your Unity project is not using Git, download the `.unitypackage` from [this branch](`PROVIDE LINK!!!!!`).

### Example scenario

The package also includes an example scene `Scenes/Example_withURP_NewInputSystem.unity`. The [description of the example](./docs/3_Example.md) guides your through the use of each of the existing Unity prefabs, their functionality, and some minor customizations that you can do to adapt the `EoM` to your specific needs.

The last version of the example scene was tested on:
- Unity 2020.3.19f1
- Universal Rendering Pipeline - [URP v10.6.0](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@10.6/manual/)
- Using the New Unity [Input System v1.0.2](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html).
- Project configured as standalone, and using OpenXR as plug-in provider.


---


# Excite-O-Meter | Unity plugin

The main plugin is found as a *.unitypackage* from the Excite-O-Meter [website](http://exciteometer.eu/)

**Requirements:**

- Developed and tested in Unity 2019.4
- Install *Unity UI Extensions* (tested with v2.2.2) official download from: [https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/wiki/Downloads)
- Install *TextMesh Pro* (tested with v2.0.1). 

![TMPro](./docsimgs/)

**Folder Structure:**

Once the package is imported in Unity, the Excite-O-Meter folder structure looks like the one below.

```
Assets
│   ...
│
└───ExciteOMeter
│   │
│   └───EoM
│   │   └─── ... // Internal EoM functionality
│   └───Prefabs
│   │   └─── EoM_BiofeedbackWorldSpace
│   │   └─── EoM_SignalEmulator
│   │   └─── ExciteOMeter_Manager
│   │   └─── ExciteOMeter_UI_OnlineAnalysis
│   └───Scenes
│   │   └─── ExciteOMeter_Demo
│   │   └─── ExciteOMeter_OfflineAnalysis
│   └─── EoM_UserManual.md
│   └─── EoM_UserManual.pdf
└─── ...
```



## Including Excite-O-Meter in your XR project from Unity Editor

1. Drag and drop both prefabs `ExciteOMeter_Manager` and `ExciteOMeter_UI_OnlineAnalysis` to the ROOT of your hierarchy. Root is needed because this objects are not destroyed between scene, in case your application runs over multiple scenes.
1. Open your *Build Settings* and add the scene `ExciteOMeter_OfflineAnalysis` the panel *Scenes in Build*. This is the scene that allows to conduct data visualization.

If you want to provide biofeedback, i.e. allow users to see the physiological signals inside the XR scene as a World Space Canvas, add the prefab `EoM_BiofeedbackWorldSpace` to your scene and it will automatically show the status connection and time series with the last 30 samples of data.

If you don't have access to the physiological sensor, but still want to test all the functionality of the Excite-O-Meter, you can drag and drop the prefab `EoM_SignalEmulator` to your hierarchy and it will simulate random data from the sensor.

### UI: Online Recorder <a name="app2a"></a>

The user interface of the online recorder looks like the image below. Each of the numbered components refer to:

1. *Open offline analysis:* Stops the current recording and opens the scene that allows offline data visualization (described in the next section).
1. *Incoming Signals:* Allows to visualize whether the physiological signals are received or not. Each signal has a color indicating connection status (green or red), the last received value and the plot of the signal.
1. *Signal problem warning:* It is displayed when a connection is dropped. If this message appears check your Android phone or Win10 app to guarantee that it is awake and sending data.
1. *Session information:* Allows to define a name of a session, define whether the session should take periodic screenshots of the scene, and a button to start or stop the session recording.
1. *Markers:* Allow the addition of contextual information that associates a timestamp with a specific event. Write a text in the input field to specify a message and click in the '+' button to add it. Quick markers are used to add markers with one click.
1. *Markers visualizer:* All the markers that are defined in the current session are displayed sequentially in this area. The markers will restart whenever the session is stopped.
1. *Biofeedback Panel:* Example of the GameObject that allows signal visualization inside the Unity scene.
1. *Scene:* Example Objects that compose your XR environment.

<img src="D:\Ludwig\Empresas\GIT-repos\XR4ALL\excite-o-meter\docs/imgs/EoM_Online_UI.jpg" width="100%">

### UI: Offline Analysis <a name="app2b"></a>

The user interface of the offline analyzer looks like the image below. Each of the numbered components refer to:

1. *Session:* List of available sessions recorded in the disk. Each session corresponds to a folder inside the Excite-O-Meter logs folder (described below).
1. *Excite-O-Meter level:* Visualization of the Excite-O-Meter value calculated during the recorded session. The image shows one color per loaded session, up to three sessions could be loaded simultaneously.
1. *Timeline:* Dragging the green square horizontally shows the instantaneous values of the signal. If screenshots are available, it will update the image to the closest screenshot to the left of the timeline marker.
1. *Signals selection:* The right panel allows to choose specific signals to visualize from each session. It includes: Heart rate, R-R interval, RMSSD, SDNN and manual markers.
1. *Signals visualizer:* Area where the selected signals will be visualized on a stacked layout. You can delete a specific signal from the 'X' in the left side of the panel.
1. *Screenshot Visualizer*: If the session contains screenshots, a square with the closest screenshot to the timeline marker will be displayed, the right hand shows the Excite-O-Meter level for each of the sessions (coded by color).
1. *Average Excite-O-Meter level*: Shows the average of the Excite-O-Meter level at the timestamp specified in the timeline.
1. *Back to main app:* This button closes the offline analysis and returns to the scene from where it was called, i.e. the XR scene.

<img src="D:\Ludwig\Empresas\GIT-repos\XR4ALL\excite-o-meter\docs/imgs/EoM_Online_UI_2.jpg" width="100%">


### Accessing Excite-O-Meter Logs

The recorded sessions are stored in the folder named `LogFiles_ExciteOMeter`. If the sessions are recorded from the *Unity Editor*, this folder is located at the same level than `Application.dataPath` (usually same level than `Assets`). In case the recordings are created after *Building* your application, this folder is located at the same level than your executable file (`.exe` on Windows).

Each folder contains a `.json` file used for offline data visualization and multiple `.csv` files with the raw data that was collected. These files can be used later for offline analysis in other platforms like Python, R, Excel, etc.

### Advanced configurations

1. **Screenshots:**
By default the Excite-O-Meter will take a screenshot of the scene every time that a marker is added. In addition, you can activate the checkbox that takes screenshots every 15 seconds periodically. Further personalization is done in the file `config.json` inside `StreamingAssets`.
1. **Adding custom events:**
If you want to execute your own scripts when a session starts or finishes from the online UI. You can place your events on the inspector of the GameObject `ExciteOMeter_Manager`. This is useful, for instance, when you want to trigger specific actions synchronized with data logging.
1. **Adding quick markers:** 
The quick markers are used to create custom markers on the timeline with one click. To add your own markers, you can look for the GameObject `QUICK_MARKERS_ExciteOMeter` in the hierarchy and duplicate the prefab `QuickMarker`. In the inspector of each quick marker, the component `QuickMarkerEoM` allows you to change the message and a custom event that will be executed when the specific marker is triggered.
1. **Changing data processing parameters:** 
The file `config.json` inside the `StreamingAssets` contains a key called `featureSettings` with basic configuration of the Excite-O-Meter that allows to change the parameters of the real-time processing workflow **even after building your application**.

- *isSampleBased: true/false* - Whether the feature is calculated when a number of samples are reached (true) or periodically every time (false)
- *windowTime: float* - For time-based: Time interval in seconds that will be used as buffer to calculate the feature.
- *overlapPercentageTime: float* - For time-based: Percentage of overlap between subsequent calculations (between 0.0 and 0.95)
- *sampleBufferLength: int* - For sample-based: Number of samples to reach before calculations.
- *overlapSamplesLength: int* - For sample-based: Number of samples to keep between subsequent calculations (integer lower than sampleBufferLength)
- *offsetSamplesInTimestamp: int* - For sample-based: Controls time-offset of the feature calculation. If 0, the feature calculation corresponds to the last collected timestamp. If 3, it will be assigned the timestamp of the sample  T-3.
- **EXAMPLES:** To calculate a time-based feature that updates the first time every 10 seconds, and every second afterwards, the first two values are `10, 0.9`; respectively. To calculate a sample-based feature of buffer 10, updated every new sample, and corresponding to a window `[t-6,t+3]`. The last three features will be `10, 9, 3`; respectively.


# Scientific Disclaimer <a name="disclaimer"></a>

The estimation of the Excite-O-Meter level shown by this application is a result of the first phase of scientific validation, which most likely will change in future development iterations. It does not represent an objective measure for medical or psychological trials.

- **Excite-O-Meter level:** The 'excitement' level was defined experimentally as follows: When data collection for a session has stopped, independently calculate perform a z-normalization of RRi and  RMSSD. Then, each value is converted into its corresponding percentile over the cumulative density function. Finally, the excitement level per timestamp is mapped as one minus average percentile of RRi and RMSSD.

The calculation of cardiac features is developed as defined on the folowing paper: *Shaffer F and Ginsberg JP (2017) An Overview of Heart Rate Variability Metrics and Norms. Front. Public Health 5:258. doi: 10.3389/fpubh.2017.00258* [view online](https://www.frontiersin.org/articles/10.3389/fpubh.2017.00258/full), and compared with the features calculated by the library Neurokit2 in Python.

- **RMSSD:** The root mean square of successive differences between normal heartbeats (RMSSD) is obtained by first calculating each successive time difference between heartbeats in ms. Then, each of the values is squared and the result is averaged before the square root of the total is obtained.
- **SDNN:** The standard deviation of the IBI of normal sinus beats (SDNN) is measured in ms. The related standard deviation of successive RR interval differences (SDSD) only represents short-term variability.





## Excitement Metric

to calculate the level of 'excitement' that your application might be causing on participants. It is particularly suitable for researchers that conduct scientific experiments in XR with Unity but also for hobbists that are developing their own games and want to measure their own reaction to the game. The main **functionalities** that the Excite-O-Meter provides are:

- Scientifically validated set of metrics to estimate Excite-O-Meter level
- Easy integration of the tool by adding two prefabs to an existing Unity project.
- Instant access to real-time physiological data collection and offline data visualization module.
- Organize each participant in different sessions that are visualized in the Excite-O-Meter offline analysis tool, or get the CSV files with the data for your own data analysis.
