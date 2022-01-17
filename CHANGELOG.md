# Changelog

Notable changes to `Excite-O-Meter` will be documented in this file.

## 1.1 (2022-01-17)

### Added

* Compatibility with multidimensional time-series (acquisition, feature extraction, and visualization)
* Added movement analysis from a GameObject in the Unity scene
* Calculation of motion features (velocity and acceleration per dimension)
* More documentation (setup devices, example, code-of-conduct, contributing, changelog)

### Changed
* Updated online UI interface to enable/disable acquisition of movement data
* Updated online UI interface to enable/disable automatic screenshots
* Updated logic to load .json for offline visualization. Showing a scrollable window with all the dimensions of the multidimensional features.

## 1.0.1 (2021-01-11)

Initial release as presented in the paper at [ISMAR2021](https://doi.org/10.1109/ISMAR52148.2021.00052).

### Added

* Compatibility with LSL data streams
* Functionalities for cardiac activity (acquisition and feature extraction)
* Implementation of general logic
