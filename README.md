<a href="https://exciteometer.eu">
<img alt="Excite-O-Meter" src="./docs/images/EoM_logo_name_horizontal.png" width="65%"/>
</a>

<!-- ![Logo](./docs/images/ExciteOmeter_Name.png){:width="50%"} -->

![Contributors](https://img.shields.io/github/contributors/luiseduve/exciteometer?style=plastic)
![Forks](https://img.shields.io/github/forks/luiseduve/exciteometer?style=plastic)
![Stars](https://img.shields.io/github/stars/luiseduve/exciteometer?style=plastic)
![Licence](https://img.shields.io/github/license/luiseduve/exciteometer?style=plastic)
![Release](https://img.shields.io/github/v/release/luiseduve/exciteometer?style=plastic)


*Table of Contents*

- [Description](#description)
- [How to use?](#how-to-use)
  - [Prerequisites](#prerequisites)
  - [Importing in Unity](#importing-in-unity)
  - [Example](#example)
  - [Scientific references](#scientific-references)
- [How to contribute?](#how-to-contribute)
- [More information](#more-information)
  - [Project's website](#projects-website)
  - [More published papers about the `EoM`](#more-published-papers-about-the-eom)
  - [Credits](#credits)

---

## Description

> The Excite-O-Meter (`EoM`) is a package that extends a standalone Unity project with functionalities to easily record users' data in experimental sessions. It captures heart and motion activity, extracts relevant features automatically, and visualizes recorded data directly in your compiled desktop Unity application. Useful for Unity developers wanting to analyze users' behavior or researchers conducting empirical studies with XR systems.

---

The `EoM` enables the integation of heart activity and movement analysis in any standalone application created with Unity, intended for Extended Reality (XR). This plugin contains all the logic to record data from external wearable sensors, log into persistent files, and visualize the captured data without leaving the Unity Editor. 

The tool is simple to use and doesn't require coding. The `EoM` is particularly suitable in two **use cases**: 1) for hobbyists or *Unity developers* wanting to measure the body responses that your application induces on your users. 2) for *researcher* running scientific experiments (e.g., psychology or behavioral research) in XR and wanting to easily collect data using a Unity environment that you created or available online.

The `EoM` may be used without external wearable sensors. However, the tool's main advantage is the easy integration with body sensors. Currently, it is compatible with the chest strap sensor [Polar H10](https://www.polar.com/us-en/products/accessories/h10_heart_rate_sensor) to capture **heart rate (HR)** and heart rate variability data (HRV), and calculate cardiac features in real time (i.e., RMSSD, SDNN). It also records **movement** from any object in the scene, useful to record head movement from **Virtual Reality (VR)** headsets record headsets. Additional data like screenshots and manual markers can be added to label actions from the users while interacting with your XR environment. Finally, it includes a data visualizer to review the session of the user offline, showing in synchrony all the time-series data, screenshots, and markers. Everything works in the Unity Editor *and in the final compiled application*.

## How to use?

_**Note:** The description below is a summary of the complete step-by-step [user manual available HERE](./docs/1_UserManual.md).

### Prerequisites

The `EoM` package already ships modified versions of two library dependencies: 1) [LSL](https://github.com/sccn/labstreaminglayer) for handling time series through the network, and 2) [UI Extensions v2.2.0](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/) to visualize time series data in the Unity UI. In addition, you need to install [Text Mesh PRO](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html) (TMPro) in your Unity project.

To collect data from external sensors, it is required to use the `Excite-O-Meter | Devices`, a middleware software that we developed as a bridge between wearable sensors non-compatible with LSL and the Unity package. Read the details about how to setup the devices in [this document](./docs/2_SetupDevices.md)

### Importing in Unity

You can import the `.unitypackage`. from `PROVIDE DOWNLOAD LINK!!!!`. If your Unity project already uses GIT, you can use this branch as a submodule: `git add submodule https://github.com/luiseduve/exciteometer.git`. Read more details in the [user manual](./docs/1_UserManual.md).

### Example

The package includes an example scene `Scenes/Example_withURP_NewInputSystem.unity`. Read more details in the [description of the example](./docs/3_Example.md).

The last version of the example scene was tested on:
- Unity 2020.3.19f1
- Universal Rendering Pipeline - [URP v10.6.0](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@10.6/manual/)
- Using the New Unity [Input System v1.0.2](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html).
- Project configured as standalone, and using OpenXR as plug-in provider.


### Scientific references

If the `EoM` is useful for your research, please consider citing the following paper:

> Quintero L, Muñoz JE, de Mooji J, Gaebler M. Excite-O-Meter: Software Framework to Integrate Heart Activity in Virtual Reality. In: *IEEE International Symposium on Mixed and Augmented Reality (ISMAR)*. Bari, Italy; 2021. p. 357–66. <https://doi.org/10.1109/ISMAR52148.2021.00052>

*Bibtex:*
```tex
@inproceedings{Quintero2021_EoM,
    address = {Bari, Italy},
    author = {Quintero, Luis and Mu{\~{n}}oz, John E and de Mooji, Jeroen and Gaebler, Michael},
    booktitle = {IEEE International Symposium on Mixed and Augmented Reality (ISMAR)},
    doi = {10.1109/ISMAR52148.2021.00052},
    pages = {357--366},
    title = {{Excite-O-Meter: Software Framework to Integrate Heart Activity in Virtual Reality}},
    year = {2021}
}
```

## How to contribute?

The contribution guidelines are available [HERE](./docs/CONTRIBUTING.md).

## More information

### Project's website

The project's website (<http://exciteometer.eu/>) contains additional information. Although it gets updated less often than the [GitHub repository](https://github.com/luiseduve/exciteometer).

### More published papers about the `EoM`

> Soon!

### Credits

* The project is currently maintained by [Luis Quintero](http://luiseduve.github.io/), part of his PhD project at the [Data Science Group](http://datascience.dsv.su.se/) at Stockholm University, Sweden.
* The project leader was [Michael Gaebler](https://www.michaelgaebler.com/), who conceptualized the project and led the work from the first publication, as found in the branch [`release_v1.0.1`](https://github.com/luiseduve/exciteometer/tree/release_v1.0.1).
* *Acknowledgements:*
* The authors wish to thank Anna Francová and Jessica Gärtner for their support in the empirical evaluation of the first version of the `EoM`; as well as Johanne Tromp, Felix Klotzsche, Mert Akbal, and Alexander Masurovsky for helping in the conceptualization of the project on its first stage.
* This project received funding from the European Union’s Horizon 2020 research and innovation programme through the XR4ALL project with grant agreement N° 825545.
* Thanks to all contributors from the required libraries [LSL](https://github.com/sccn/labstreaminglayer), [UI Extensions](https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/).