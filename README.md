<a href="https://exciteometer.eu">
<img alt="Excite-O-Meter" src="./docs/images/ExciteOmeter_Name.png" width="60%"/>
</a>

<!-- ![Logo](./docs/images/ExciteOmeter_Name.png){:width="50%"} -->

<!-- [![Open in Visual Studio Code](https://open.vscode.dev/badges/open-in-vscode.svg)](https://open.vscode.dev/luiseduve/exciteometer) -->
![Contributors](https://img.shields.io/github/contributors/luiseduve/exciteometer?style=plastic)
![Forks](https://img.shields.io/github/forks/luiseduve/exciteometer?style=plastic)
![Stars](https://img.shields.io/github/stars/luiseduve/exciteometer?style=plastic)
![Licence](https://img.shields.io/github/license/luiseduve/exciteometer?style=plastic)
![Release](https://img.shields.io/github/v/release/luiseduve/exciteometer?style=plastic)
<!-- ![Issues](https://img.shields.io/github/issues/luiseduve/exciteometer?style=plastic) -->

Table of Contents

- [Description](#description)
- [How to use?](#how-to-use)
  - [Dependencies](#dependencies)
  - [Instructions](#instructions)
  - [Example](#example)
  - [Scientific references](#scientific-references)
- [How to contribute?](#how-to-contribute)
- [More information](#more-information)
  - [Project's website](#projects-website)
  - [More published papers about the `EoM`](#more-published-papers-about-the-eom)
  - [Credits](#credits)
  - [Previous releases](#previous-releases)

---

## Description

> The Excite-O-Meter (`EoM`) is a package that can be included in your existing standalone Unity projects to easily record users' data divided by sessions. It captures heart and motion activity, and enables easy data visualization in your compiled desktop Unity application.

---

The `EoM` enables the integation of heart activity and movement analysis in any standalone application created with Unity, intended for Extended Reality (XR). This plugin contains all the logic to record data from external sensors, log into persistent files, and visualize the captured data without leaving the Unity Editor. 

The tool is simple to use and doesn't require coding. The `EoM` is useful mainly in two **use cases**: 1) you are a *Unity developer* wanting to see the responses that your application induces on your users. 2) you are a *researcher* running studies (e.g., psychology or behavioral research) wanting to collect behavioral data in a Unity environment that you created or found available.

Currently, the `EoM` is compatible with the the chest strap sensor [Polar H10](https://www.polar.com/us-en/products/accessories/h10_heart_rate_sensor) to capture **heart rate (HR)** and heart rate variability data (HRV), and calculate cardiac features in real time (i.e., RMSSD, SDNN). It also records **movement** from any object in the scene, useful to record head movement from **Virtual Reality (VR)** headsets record headsets. Additional data like screenshots and manual markers can be added to label actions from the users while interacting with your XR environment. Finally, it includes a data visualizer to review the session of the user offline, showing in synchrony all the time-series data, screenshots, and markers. Everything works in the Unity Editor *and in the final compiled application*.

## How to use?

### Dependencies

The tool requires middleware software to connect to the external sensors.

The Unity project requires the previous packages:
- ui-extensions
- TMPro.

### Instructions

There are two ways to include the `EoM` package in your existing Unity project.
Instructions to add exciteometer to a Unity project, and continue contributing to the EoM while working in the specific project.

- The host Unity project should use git VCS.
- Change to the assets folder of the Unity project: `cd Assets`
- Add the exciteometer submodule: `git submodule add https://github.com/luiseduve/exciteometer.git`
- Reimport

**Note:** Check the step-by-step User Manual [HERE](docs/QuickReference.md)

### Example

Tested on:
- Unity 2020.3.19f1 using URP pipeline and new input system.


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

The contribution guidelines are available [HERE](./CONTRIBUTING.md).

## More information

### Project's website

The project's website (<http://exciteometer.eu/>) contains additional information. Although it gets updated less often than the [GitHub repository](https://github.com/luiseduve/exciteometer).

### More published papers about the `EoM`

> Soon!

### Credits

* The project is currently maintained by [Luis Quintero](http://luiseduve.github.io/), part of his PhD project at the [Data Science Group](http://datascience.dsv.su.se/) at Stockholm University, Sweden.
* The project leader was [Michael Gaebler](https://www.michaelgaebler.com/), who conceptualized the project and led the work from the first publication. The team was composed by Luis Quintero, John Muñoz, and Jeroen de Mooij.
* **Acknowledgements:** The authors wish to thank Anna Francová and Jessica Gärtner for their support in the empirical evaluation of the first version of the `EoM`;
as well as Johanne Tromp, Felix Klotzsche, Mert Akbal, and
Alexander Masurovsky for helping in the conceptualization of the
project on its first stage. This project received funding from the European Union’s Horizon 2020 research and innovation programme through the XR4ALL project with grant agreement N° 825545.

### Previous releases

The first version of the **Excite-O-Meter** is the **development** branch . To download the binaries and documentation please check the branch `release_v1.0.1`