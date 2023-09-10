# **Example Unity | Excite-O-Meter**

*Quick links*

- [**Example Unity | Excite-O-Meter**](#example-unity--excite-o-meter)
- [Setup a compatible Unity project](#setup-a-compatible-unity-project)
  - [Requirements](#requirements)
  - [Folder Structure](#folder-structure)
- [Description of Example Scene](#description-of-example-scene)
- [Including the Excite-O-Meter in your project](#including-the-excite-o-meter-in-your-project)
  - [Optional setups](#optional-setups)
- [UI: Online Recorder](#ui-online-recorder)
  - [UI: Offline Analysis](#ui-offline-analysis)
  - [Accessing Excite-O-Meter Logs](#accessing-excite-o-meter-logs)
  - [Build your application](#build-your-application)
  - [Advanced configurations](#advanced-configurations)
---

# Setup a compatible Unity project

## Requirements

The example requires that you

- Developed and tested in Unity 2021.3.17f1
- Install *TextMesh Pro* (tested with v3.0.6). If your project does not include it yet, Unity will show a prompt message as shown below when you drag and drop the prefab containing the UI of the Excite-O-Meter.
    
<img src="./docsimgs/Unity_TMPro.jpg" width="60%">

## Folder Structure

Once the package is imported in Unity, the Excite-O-Meter folder structure looks like the one below.

```
Assets
│
└─── [Your Assets]
└─── ...
└─── exciteometer/
│   │
│   └─── docs/
│   │   └─── ... // Project's Documentation
│   └─── EoM/
│   │   └─── ... // Internal EoM functionality
│   └───Prefabs/
│   │   └─── EoM_BiofeedbackWorldSpace.prefab
│   │   └─── EoM_SignalEmulator.prefab
│   │   └─── ExciteOMeter_Manager.prefab
│   │   └─── ExciteOMeter_UI_OnlineAnalysis.prefab
│   └───Scenes/
│   │   └─── ExciteOMeter_URP_NewInputSystem/
│   │   └─── Example_withURP_NewInputSystem.scene
│   │   └─── Example_withURP_NewInputSystemOpenXR.scene
│   │   └─── ExciteOMeter_OfflineAnalysis.scene
│   └─── README.md
└─── ...
```

# Description of Example Scene

The `Example_` scenes show a simple experiment lasting around 1 minute with three stages. Each stage shows a cube of a different color with an interim break between stages.

The application is developed in Universal Rendering Pipeline (URP) and the new Input System. In case your project uses built-in rendering pipeline, the example scene and the folder with the assets can be deleted without affecting the functionalities of the Excite-O-Meter. 

The reason why this new version uses the New Input System is to enable compatibility with the OpenXR standard that allows building cross-platform interactive applications. There is a desktop-based environment called `Example_withURP_NewInputSystem.scene`, and a VR-compatible version named `Example_withURP_NewInputSystemOpenXR.scene`. 

There are some additional buttons in the UI that enable control of the example experiment (e.g., training stage, close app) that are specific from the experiment and coexist with the UI from the Excite-O-Meter. A visualization bar displays when the user presses the trigger buttons of the controllers, useful for subjective feedback during the experiment.

# Including the Excite-O-Meter in your project

1. Drag and drop both prefabs `ExciteOMeter_Manager` and `ExciteOMeter_UI_OnlineAnalysis` to the ROOT of your hierarchy. Root is needed because these objects are not destroyed between scenes.
2. Open your *Build Settings* and add the scene `ExciteOMeter_OfflineAnalysis.scene` to the panel *Scenes in Build*. This is the scene that allows to conduct data visualization within the Unity Editor and the compiled application.

## Optional setups
1. **If you want to provide biofeedback**, i.e., let users see their own physiological signals inside the XR scene as a World Space Canvas, add the prefab `EoM_BiofeedbackWorldSpace` to your scene and it will automatically show the status connection and time series with the last 30 samples of data.
2. **If you don't have access to the sensor Polar H10**, but still want to test all the functionality of the Excite-O-Meter, you can drag and drop the prefab `EoM_SignalEmulator` to your hierarchy and it will simulate random data from the sensor.

# UI: Online Recorder

The user interface added with the prefab `Excite_Meter_UI_OnlineAnalysis` looks like the image below. Each of the numbered components refer to:

1. *Open offline analysis:* Stops the current recording and opens the scene that allows offline data visualization (described in the next section).
2. *Incoming Signals:* Allows to visualize whether the physiological signals are received or not. Each signal has a color indicating connection status (green or red), the last received value and the plot of the signal.
3. *Signal problem warning:* It is displayed when a connection is dropped. If this message appears check your Android phone or Win10 app to guarantee that it is awake and sending data.
4. *Session information:* Allows to define a name of a session, define whether the session should record movement from the headset, take periodic screenshots of the scene, and a button to start or stop the session recording.
5. *Markers:* Allow the addition of contextual information that associates a timestamp with a specific event. Write a text in the input field to specify a message and click in the '+' button to add it. Quick markers are used to add markers with one click.
6. *Markers visualizer:* All the markers that are defined in the current session are displayed sequentially in this area. The markers will restart whenever the session is stopped.
7. *Biofeedback Panel:* Example of the GameObject that allows signal visualization inside the Unity scene.
8. *Scene:* Example Objects that compose your XR environment and experimental setup.

<img src="./docsimgs/OnlineAnalysisGuiExplanation.png" width="100%">

## UI: Offline Analysis

The user interface of the offline analyzer looks like the image below. Each of the numbered components refer to:

1. *Session:* List of available sessions recorded in the disk. Each session corresponds to a folder inside the Excite-O-Meter logs folder (described below).
1. *Excite-O-Meter level:* Visualization of the Excite-O-Meter value calculated during the recorded session. The image shows one color per loaded session, up to three sessions could be loaded simultaneously.
1. *Timeline:* Dragging the green square horizontally shows the instantaneous values of the signal. If screenshots are available, it will update the image to the closest screenshot to the left of the timeline marker.
2. *Signals selection:* The right panel allows to choose specific signals to visualize from each session. It includes: Heart rate, R-R interval, RMSSD, SDNN, manual markers, and movement features like headset rotation, velocity, and acceleration in yaw, pitch, roll.
3. *Signals visualizer:* Area where the selected signals will be visualized on a stacked layout. You can delete a specific signal from the 'X' in the left side of the panel.
4. *Screenshot Visualizer*: If the session contains screenshots, a square with the closest screenshot to the timeline marker will be displayed, the right hand shows the Excite-O-Meter level for each of the sessions (coded by color).
5. *Average Excite-O-Meter level*: Shows the average of the Excite-O-Meter level at the timestamp specified in the timeline.
6. *Back to main app:* This button closes the offline analysis and returns to the scene from where it was called, i.e. the XR scene.

<img src="./docsimgs/OfflineAnalysisGuiExplanation.png" width="100%">


## Accessing Excite-O-Meter Logs

The recorded sessions are stored in the folder named `LogFiles_ExciteOMeter`. If the sessions are recorded from the *Unity Editor*, this folder is located at the same level than `Application.dataPath` (usually same level than `Assets`). In case the recordings are created after *Building* your application, this folder is located at the same level than your executable file (`.exe` on Windows).

Each folder contains a `.json` file used for offline data visualization and multiple `.csv` files with the raw data that was collected. These files can be used later for offline analysis in other platforms like Python, R, Excel, etc.


## Build your application

Before building your application, delete the file `StreamingAsets/config.json` to generate a new configuration file the first time that the application is executed in the final build.
Every recorded session is inside the main folder named `LogFiles_ExciteOMeter`, found at the same level than your main `.exe` file.

## Advanced configurations

1. **Movement:**
The latest version of the Excite-O-Meter allows analysis of movement trajectories. Commonly useful for field-of-view analysis when tracking a VR headset. The object to be tracked needs to be configured from the component `MovementDataManager` within the `MovementRecorder`. By default, movement will be collected at 10Hz, but it can be configured from the UI to higher sampling rates. *Note that the maximum frequency corresponds to the FPS of the application (usually 60Hz), and high sampling frequencies might affect performance.*

<img src="./docsimgs/exampleHeadMovementSetup.png" width="80%">

2. **Screenshots:**
By default the Excite-O-Meter will take a screenshot of the scene every time that a marker is added. In addition, you can activate the checkbox to record screenshots at a periodic frequency (every 20 seconds by default). Further personalization is done in the file `config.json` inside `StreamingAssets`.

3. **Adding custom events:**
If you want to execute your own scripts when a session starts or finishes from the online UI. You can place your events on the inspector of the GameObject `ExciteOMeter_Manager`. This is useful, for instance, when you want to trigger pecific actions synchronized with data logging.

4. **Adding quick markers:**
The quick markers are used to create custom markers on the timeline with one click. To add your own markers, you can look for the GameObject `QUICK_MARKERS_ExciteOMeter` in the hierarchy and duplicate the prefab `QuickMarker`. In the inspector of each quick marker, the component `QuickMarkerEoM` allows you to change the message and a custom event that will be executed when the specific marker is triggered.

5. **Changing data processing parameters:**
The file `config.json` inside the `StreamingAssets` contains a key called `featureSettings` with basic configuration of the Excite-O-Meter that allows to change the parameters of the real-time processing workflow **even after building your application**.

- *isSampleBased: true/false* - Whether the feature is calculated when a number of samples are reached (true) or periodically every time (false)
- *windowTime: float* - For time-based: Time interval in seconds that will be used as buffer to calculate the feature.
- *overlapPercentageTime: float* - For time-based: Percentage of overlap between subsequent calculations (between 0.0 and 0.95)
- *sampleBufferLength: int* - For sample-based: Number of samples to reach before calculations.
- *overlapSamplesLength: int* - For sample-based: Number of samples to keep between subsequent calculations (integer lower than sampleBufferLength)
- *offsetSamplesInTimestamp: int* - For sample-based: Controls time-offset of the feature calculation. If 0, the feature calculation corresponds to the last collected timestamp. If 3, it will be assigned the timestamp of the sample  T-3.

  - **EXAMPLES:** To calculate a time-based feature that updates the first time every 10 seconds, and every second afterwards, the first two values are `10, 0.9`; respectively. To calculate a sample-based feature of buffer 10, updated every new sample, and corresponding to a window `[t-6,t+3]`. The last three features will be `10, 9, 3`; respectively.
