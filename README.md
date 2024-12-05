<img style="" src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/raven-scan-logo-blue-text-gray-nobg-1080x360-svg.svg" alt="Raven Scan Logo"/>

[![Raven Scan](https://img.shields.io/badge/Raven-Scan-darkgray)](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring)
[![.Net](https://img.shields.io/badge/.NET-5C2D91?style=flat&logo=.net&logoColor=white)](https://dotnet.microsoft.com/en-us/)
[![Download Dataset](https://app.roboflow.com/images/download-dataset-badge.svg)](https://universe.roboflow.com/igor-dimitrovski-qowpq/dumpsite-detection)
[![codecov](https://codecov.io/gh/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/branch/master/graph/badge.svg?token=44SYRYP1H7)](https://codecov.io/gh/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring)
[![Open Source](https://badgen.net/badge/Open%20Source/Permissive?icon=github)](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring?tab=Apache-2.0-1-ov-file)
[![Apache 2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://www.apache.org/licenses/LICENSE-2.0.html)

# Welcome to the [ __Raven Scan__ ] platform documentation

## Overview  

The Raven Scan platform leverages advanced drone and satellite imagery to enhance waste management and environmental monitoring through cutting-edge technology.  

Utilizing high-resolution images combined with sophisticated image annotation, object detection models, and geospatial analysis, our system offers robust tools to identify illegal dump sites and effectively manage regulated landfills.  

## User Guides and Documentation

### Guides  

Explore each feature through our [User Guides](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/user-guides.html)

### Feature Documentation  

Learn more from our detailed [Feature Documentation](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/dev-docs-overview.html)

## 📝Table of Contents

| [Overview](#overview)                                                                                                | [Getting Started](#getting-started)        | [Licensing](#licensing)              |  
| -------------------------------------------------------------------------------------------------------------------- | ------------------------------------------ | ------------------------------------ |  
| [Guides and Docs](#user-guides-and-documentation)                                                                    | [Dependencies](#dependencies)              | [Open-Sourced](#open-sourced)       |  
| [Key Features](#key-features)                                                                                        | [Raven Scan Showcase](raven-scan-showcase) | [Acknowledgments](#acknowledgments) |  
| [Map](#map)  \| [Datasets](#dataset-management) \| [Annotation](#image-annotation)                                   | [Development](#development)                | [Venture Fund](#unicef-venture-fund)|  
| [Orthophoto Inputs](#detection-images) \| [Landfill Management](#landfill) \| [3D Point-Cloud Scans](#3d-point-cloud) | [Code of Conduct](#code-of-conduct)        | [MMDetection](#mmdetection)         |  
| [AI Model Training](#ai-model-training) \| [Waste Detection and Monitoring](#waste-detection)                        | [Contributing](#contributing)              | [Third-Party](#third-party-notices) |  

## Key Features  

<table>
  
  <tr>
  <!-- Map -->
  <td align="center">
    <h4 id="map">Map</h4>
    <h6>View Detected Dumpsites on Map</h6>
    <h6>Open-Layers, layer switcher, view terrain, adjust coloring</h6>
    <h6>Direct GeoTiff Injection, review historical scans of areas</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/clipart-map-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/map/map-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/map/overview.html"><h6>Docs</h6></a>
    <h6>
  </td>

  <!-- Dataset Management -->
  <td align="center">
    <h4 id="dataset-management">Dataset Management</h4>
    <h6>Manage extensive Datasets of Drone and Satellite images</h6>
    <h6>Tools for uploading, categorizing, and maintaining image data</h6>
    <h6>Features include tagging, filtering, and robust data integrity checks</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/dataset-manage-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/dataset-management/dataset-management-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/dataset-management/overview.html"><h6>Docs</h6></a>
    </h6>
  </td>
  </tr>

  <tr>
  <!-- Image Annotation -->
  <td align="center">
    <h4 id="image-annotation" >Image Annotation</h4>
    <h6>Annotate High-Resolution imagery</h6>
    <h6>Draw, adjust, enable and disable bounding boxes</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/annotate-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/dataset-management/image-annotation-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/dataset-management/image-annotation.html"><h6>Docs</h6></a>
    </h6>
  </td>

  <!-- Detection Input Images -->
  <td align="center">
    <h4 id="detection-images">Detection Input Images</h4>
    <h6>Reuse Input Images and save space</h6>
    <h6>View input images as Map Layers</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/detection-input-images-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/detection-process/detection-input-images-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/detection-process/detection-input-images.html"><h6>Docs</h6></a>
    </h6>
  </td>
  </tr>

  <tr>
  <!-- Landfill Management -->
  <td align="center">
    <h4 id="landfill">Landfill Management</h4>
    <h6>Advanced tools for legal landfill management</h6>
    <h6>Waste Form Submission integration, types, trucks, imports</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/truck-waste-fly-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/landfill-management/landfill-management-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/landfill-management/overview.html"><h6>Docs</h6></a>
    </h6>
  </td>

  <!-- 3D Point-Cloud -->
  <td align="center">
    <h4 id="3d-point-cloud">3D Point-Cloud</h4>
    <h6>3D Point-Cloud scan integration</h6>
    <h6>View and Compare, Measure Height, Distance, Volume</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/3D-point-cloud-upload-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/landfill-management/3d-point-cloud-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/landfill-management/3d-point-cloud.html"><h6>Docs</h6></a>
    </h6>
  </td>
  </tr>

  <tr>
  <!-- AI Model Training -->
  <td align="center">
    <h4 id="ai-model-training">AI Model Training</h4>
    <h6>Train proven AI model architectures with your custom datasets</h6>
    <h6>Reinforce your custom-trained models as more data comes in</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/robot-train-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/training-process/training-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/training-process/overview.html"><h6>Docs</h6></a>
    </h6>
  </td>

  <!-- Waste Detection and Monitoring of Dumpsites -->
  <td align="center">
    <h4 id="waste-detection">Waste Detection and Monitoring of Dumpsites</h4>
    <h6>Detect using Custom-Trained AI Models</h6>
    <h6>Visualize Results on Georeferenced Map</h6>
    <img width=150 src="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/object-detection-500.png" alt=""/>
    <h6 align="center">
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/guides/detection-process/detection-guide.html">Guide</a>
      <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/documentation/detection-process/overview.html"><h6>Docs</h6></a>
    </h6>
  </td>
  </tr>
</table>

This repository aims to equip researchers, environmental agencies, and policymakers with the tools needed to monitor and respond to environmental challenges efficiently.  

Join us in leveraging these capabilities to maintain ecological integrity and promote sustainable practices in waste management.  

Our complete [Project Charter](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/about/project-charter.html).

Our official [Documentation Page](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/index.html).

## Getting Started  

We provide a guide that will help you set up and run the __[ Raven Scan ]__ platform on your local development environment.  

- <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 512 512"><path d="M288 32c0-17.7-14.3-32-32-32s-32 14.3-32 32l0 242.7-73.4-73.4c-12.5-12.5-32.8-12.5-45.3 0s-12.5 32.8 0 45.3l128 128c12.5 12.5 32.8 12.5 45.3 0l128-128c12.5-12.5 12.5-32.8 0-45.3s-32.8-12.5-45.3 0L288 274.7 288 32zM64 352c-35.3 0-64 28.7-64 64l0 32c0 35.3 28.7 64 64 64l384 0c35.3 0 64-28.7 64-64l0-32c0-35.3-28.7-64-64-64l-101.5 0-45.3 45.3c-25 25-65.5 25-90.5 0L165.5 352 64 352zm368 56a24 24 0 1 1 0 48 24 24 0 1 1 0-48z"/></svg> <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 512 512"><path d="M464 256A208 208 0 1 0 48 256a208 208 0 1 0 416 0zM0 256a256 256 0 1 1 512 0A256 256 0 1 1 0 256zM188.3 147.1c7.6-4.2 16.8-4.1 24.3 .5l144 88c7.1 4.4 11.5 12.1 11.5 20.5s-4.4 16.1-11.5 20.5l-144 88c-7.4 4.5-16.7 4.7-24.3 .5s-12.3-12.2-12.3-20.9l0-176c0-8.7 4.7-16.7 12.3-20.9z"/></svg> The complete [installation guide](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/development/set-up.html) can be found on our official [documentation page](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/index.html)  

### Dependencies  

| __Dependency Type__     | __Technology / Tool__    | __Dependency Type__     | __Technology / Tool__    |  
|-------------------------|--------------------------|-------------------------|--------------------------|  
| __MVC Dependencies__    |                          | __AI Dependencies__     |                          |  
| Web Framework           | .NET 8                   | AI Platform             | MMDetection 3.3          |  
| ORM                     | Entity Framework         | AI Processing Library   | Pytorch (_CUDA_)         |  
| Package Manager         | NuGet                    | Package Manager         | Miniconda 3              |  
| Package Manager         | npm                      | Programming Language    | Python 3.8               |  
| Frontend Library        | Open Layers              | Programming Language    | C++ 14                   |  
| Geographic Library      | NetTopologySuite         |                         |                          |  
| Geographic Library      | GDAL                     |                         |                          |  
| 3D Point-Cloud Library  | PoTree                   |                         |                          |  
| Database                | PostgreSQL 16            |                         |                          |  
| Database Extension      | PostGIS                  |                         |                          |  
| Scheduling Library      | Hangfire                 |                         |                          |  

- <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 512 512"><path d="M0 96C0 78.3 14.3 64 32 64l384 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L32 128C14.3 128 0 113.7 0 96zM64 256c0-17.7 14.3-32 32-32l384 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L96 288c-17.7 0-32-14.3-32-32zM448 416c0 17.7-14.3 32-32 32L32 448c-17.7 0-32-14.3-32-32s14.3-32 32-32l384 0c17.7 0 32 14.3 32 32z"/></svg> <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 640 512"><path d="M272.2 64.6l-51.1 51.1c-15.3 4.2-29.5 11.9-41.5 22.5L153 161.9C142.8 171 129.5 176 115.8 176L96 176l0 128c20.4 .6 39.8 8.9 54.3 23.4l35.6 35.6 7 7c0 0 0 0 0 0L219.9 397c6.2 6.2 16.4 6.2 22.6 0c1.7-1.7 3-3.7 3.7-5.8c2.8-7.7 9.3-13.5 17.3-15.3s16.4 .6 22.2 6.5L296.5 393c11.6 11.6 30.4 11.6 41.9 0c5.4-5.4 8.3-12.3 8.6-19.4c.4-8.8 5.6-16.6 13.6-20.4s17.3-3 24.4 2.1c9.4 6.7 22.5 5.8 30.9-2.6c9.4-9.4 9.4-24.6 0-33.9L340.1 243l-35.8 33c-27.3 25.2-69.2 25.6-97 .9c-31.7-28.2-32.4-77.4-1.6-106.5l70.1-66.2C303.2 78.4 339.4 64 377.1 64c36.1 0 71 13.3 97.9 37.2L505.1 128l38.9 0 40 0 40 0c8.8 0 16 7.2 16 16l0 208c0 17.7-14.3 32-32 32l-32 0c-11.8 0-22.2-6.4-27.7-16l-84.9 0c-3.4 6.7-7.9 13.1-13.5 18.7c-17.1 17.1-40.8 23.8-63 20.1c-3.6 7.3-8.5 14.1-14.6 20.2c-27.3 27.3-70 30-100.4 8.1c-25.1 20.8-62.5 19.5-86-4.1L159 404l-7-7-35.6-35.6c-5.5-5.5-12.7-8.7-20.4-9.3C96 369.7 81.6 384 64 384l-32 0c-17.7 0-32-14.3-32-32L0 144c0-8.8 7.2-16 16-16l40 0 40 0 19.8 0c2 0 3.9-.7 5.3-2l26.5-23.6C175.5 77.7 211.4 64 248.7 64L259 64c4.4 0 8.9 .2 13.2 .6zM544 320l0-144-48 0c-5.9 0-11.6-2.2-15.9-6.1l-36.9-32.8c-18.2-16.2-41.7-25.1-66.1-25.1c-25.4 0-49.8 9.7-68.3 27.1l-70.1 66.2c-10.3 9.8-10.1 26.3 .5 35.7c9.3 8.3 23.4 8.1 32.5-.3l71.9-66.4c9.7-9 24.9-8.4 33.9 1.4s8.4 24.9-1.4 33.9l-.8 .8 74.4 74.4c10 10 16.5 22.3 19.4 35.1l74.8 0zM64 336a16 16 0 1 0 -32 0 16 16 0 1 0 32 0zm528 16a16 16 0 1 0 0-32 16 16 0 1 0 0 32z"/></svg> See all [Project Dependencies](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/development/dependencies/dependencies.html)  

## Raven Scan Showcase  

Below are screenshots demonstrating Raven Scan's key capabilities in action:  

![Dataset Management Overview of Dataset Images and their statuses](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/screen-shot-dataset-manage-designed.png)  
_Dataset Management Overview of Dataset Images and their statuses_

![Detected Dumpsites show on Map](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/screen-shot-detection-volkovo-best-historic.png)  
_Detected Dumpsites show on Map_

![3D Point-Cloud Volume Calculation of a Landfill's Waste Heap](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/ins-template/public/images/screen-shot-3d-point-cloud.png)  
_3D Point-Cloud Volume Calculation of a Landfill's Waste Heap_

## Development  

Support & Issues  

Check the detailed documentation for troubleshooting  
Create an issue in the GitHub repository  

If you are ready to contribute, experiencing a bug, or just curious  

- <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 640 512"><path d="M392.8 1.2c-17-4.9-34.7 5-39.6 22l-128 448c-4.9 17 5 34.7 22 39.6s34.7-5 39.6-22l128-448c4.9-17-5-34.7-22-39.6zm80.6 120.1c-12.5 12.5-12.5 32.8 0 45.3L562.7 256l-89.4 89.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l112-112c12.5-12.5 12.5-32.8 0-45.3l-112-112c-12.5-12.5-32.8-12.5-45.3 0zm-306.7 0c-12.5-12.5-32.8-12.5-45.3 0l-112 112c-12.5 12.5-12.5 32.8 0 45.3l112 112c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L77.3 256l89.4-89.4c12.5-12.5 12.5-32.8 0-45.3z"/></svg> <svg xmlns="http://www.w3.org/2000/svg"  width=14 viewBox="0 0 384 512"><path d="M64 464c-8.8 0-16-7.2-16-16L48 64c0-8.8 7.2-16 16-16l160 0 0 80c0 17.7 14.3 32 32 32l80 0 0 288c0 8.8-7.2 16-16 16L64 464zM64 0C28.7 0 0 28.7 0 64L0 448c0 35.3 28.7 64 64 64l256 0c35.3 0 64-28.7 64-64l0-293.5c0-17-6.7-33.3-18.7-45.3L274.7 18.7C262.7 6.7 246.5 0 229.5 0L64 0zm97 289c9.4-9.4 9.4-24.6 0-33.9s-24.6-9.4-33.9 0L79 303c-9.4 9.4-9.4 24.6 0 33.9l48 48c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-31-31 31-31zM257 255c-9.4-9.4-24.6-9.4-33.9 0s-9.4 24.6 0 33.9l31 31-31 31c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l48-48c9.4-9.4 9.4-24.6 0-33.9l-48-48z"/></svg> Check out the [Development Documentation](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/development/intro.html)  

## Code of Conduct

We are committed to fostering a welcoming and inclusive community.  

Our project adheres to a Code of Conduct that outlines expectations for participation and community standards for behavior.  

We encourage all contributors and participants to review and adhere to these guidelines.

By participating in this project, you agree to abide by its terms.  

- <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 640 512"><path d="M392.8 1.2c-17-4.9-34.7 5-39.6 22l-128 448c-4.9 17 5 34.7 22 39.6s34.7-5 39.6-22l128-448c4.9-17-5-34.7-22-39.6zm80.6 120.1c-12.5 12.5-12.5 32.8 0 45.3L562.7 256l-89.4 89.4c-12.5 12.5-12.5 32.8 0 45.3s32.8 12.5 45.3 0l112-112c12.5-12.5 12.5-32.8 0-45.3l-112-112c-12.5-12.5-32.8-12.5-45.3 0zm-306.7 0c-12.5-12.5-32.8-12.5-45.3 0l-112 112c-12.5 12.5-12.5 32.8 0 45.3l112 112c12.5 12.5 32.8 12.5 45.3 0s12.5-32.8 0-45.3L77.3 256l89.4-89.4c12.5-12.5 12.5-32.8 0-45.3z"/></svg> <svg width=18  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512"><path d="M434.7 64h-85.9c-8 0-15.7 3-21.6 8.4l-98.3 90c-.1 .1-.2 .3-.3 .4-16.6 15.6-16.3 40.5-2.1 56 12.7 13.9 39.4 17.6 56.1 2.7 .1-.1 .3-.1 .4-.2l79.9-73.2c6.5-5.9 16.7-5.5 22.6 1 6 6.5 5.5 16.6-1 22.6l-26.1 23.9L504 313.8c2.9 2.4 5.5 5 7.9 7.7V128l-54.6-54.6c-5.9-6-14.1-9.4-22.6-9.4zM544 128.2v223.9c0 17.7 14.3 32 32 32h64V128.2h-96zm48 223.9c-8.8 0-16-7.2-16-16s7.2-16 16-16 16 7.2 16 16-7.2 16-16 16zM0 384h64c17.7 0 32-14.3 32-32V128.2H0V384zm48-63.9c8.8 0 16 7.2 16 16s-7.2 16-16 16-16-7.2-16-16c0-8.9 7.2-16 16-16zm435.9 18.6L334.6 217.5l-30 27.5c-29.7 27.1-75.2 24.5-101.7-4.4-26.9-29.4-24.8-74.9 4.4-101.7L289.1 64h-83.8c-8.5 0-16.6 3.4-22.6 9.4L128 128v223.9h18.3l90.5 81.9c27.4 22.3 67.7 18.1 90-9.3l.2-.2 17.9 15.5c15.9 13 39.4 10.5 52.3-5.4l31.4-38.6 5.4 4.4c13.7 11.1 33.9 9.1 45-4.7l9.5-11.7c11.2-13.8 9.1-33.9-4.6-45.1z"/></svg> Full [Code of Conduct](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/blob/master/CODE_OF_CONDUCT.md)  

## Contributing

We welcome contributions from the community.  

Whether you're fixing bugs, adding new features, or improving documentation, your help is greatly appreciated.  

For detailed instructions on how to contribute, please see our

- <svg width=18  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512"><path d="M565.6 36.2C572.1 40.7 576 48.1 576 56l0 336c0 10-6.2 18.9-15.5 22.4l-168 64c-5.2 2-10.9 2.1-16.1 .3L192.5 417.5l-160 61c-7.4 2.8-15.7 1.8-22.2-2.7S0 463.9 0 456L0 120c0-10 6.1-18.9 15.5-22.4l168-64c5.2-2 10.9-2.1 16.1-.3L383.5 94.5l160-61c7.4-2.8 15.7-1.8 22.2 2.7zM48 136.5l0 284.6 120-45.7 0-284.6L48 136.5zM360 422.7l0-285.4-144-48 0 285.4 144 48zm48-1.5l120-45.7 0-284.6L408 136.5l0 284.6z"/></svg> <svg width=18  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512"><path d="M159.9 175.8h64v64a16 16 0 0 0 16 16h64a16 16 0 0 0 16-16v-64h64a16 16 0 0 0 16-16v-64a16 16 0 0 0 -16-16h-64v-64a16 16 0 0 0 -16-16h-64a16 16 0 0 0 -16 16v64h-64a16 16 0 0 0 -16 16v64A16 16 0 0 0 159.9 175.8zM568.1 336.1a39.9 39.9 0 0 0 -55.9-8.5L392.5 415.8H271.9a16 16 0 0 1 0-32H350.1c16 0 30.8-10.9 33.4-26.6a32.1 32.1 0 0 0 -31.6-37.4h-160a117.7 117.7 0 0 0 -74.1 26.3l-46.5 37.7H15.9a16.1 16.1 0 0 0 -16 16v96a16.1 16.1 0 0 0 16 16h347a104.8 104.8 0 0 0 61.7-20.3L559.6 392A40 40 0 0 0 568.1 336.1z"/></svg> Complete [Contributing Guidelines](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/blob/master/CONTRIBUTING.md)  

## Licensing  

This project is licensed under the __Apache License 2.0__.  
This license allows for a great deal of freedom in both academic and commercial use of this software.  

- <svg width=18  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 576 512"><path d="M528 160l0 256c0 8.8-7.2 16-16 16l-192 0c0-44.2-35.8-80-80-80l-64 0c-44.2 0-80 35.8-80 80l-32 0c-8.8 0-16-7.2-16-16l0-256 480 0zM64 32C28.7 32 0 60.7 0 96L0 416c0 35.3 28.7 64 64 64l448 0c35.3 0 64-28.7 64-64l0-320c0-35.3-28.7-64-64-64L64 32zM272 256a64 64 0 1 0 -128 0 64 64 0 1 0 128 0zm104-48c-13.3 0-24 10.7-24 24s10.7 24 24 24l80 0c13.3 0 24-10.7 24-24s-10.7-24-24-24l-80 0zm0 96c-13.3 0-24 10.7-24 24s10.7 24 24 24l80 0c13.3 0 24-10.7 24-24s-10.7-24-24-24l-80 0z"/></svg> <svg width=18  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 640 512"><path d="M256 336h0c0-16.2 1.3-8.7-85.1-181.5-17.7-35.3-68.2-35.4-85.9 0C-2.1 328.8 0 320.3 0 336H0c0 44.2 57.3 80 128 80s128-35.8 128-80zM128 176l72 144h66l72-144zm512 160c0-16.2 1.3-8.7-85.1-181.5-17.7-35.3-68.2-35.4-85.9 0-87.1 174.3-85 165.8-85 181.5H384c0 44.2 57.3 80 128 80s128-35.8 128-80h0zM440 320l72-144 72 144H440zm88 128H352V153.3c23.5-10.3 41.2-31.5 46.4-57.3h628c8.8 0 16-7.2 16-16V48c0-8.8-7.2-16-16-16H383.6C369 12.7 346.1 0 320 0s-49 12.7-63.6 32H112c-8.8 0-16 7.2-16 16v32c0 8.8 7.2 16 16 16h129.6c5.2 25.8 22.9 47 46.4 57.3V448H112c-8.8 0-16 7.2-16 16v32c0 8.8 7.2 16 16 16h416c8.8 0 16-7.2 16-16v-32c0-8.8-7.2-16-16-16z"/></svg> See the Complete [License Text](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring?tab=Apache-2.0-1-ov-file#readme).  

## Open-Sourced

### Collected Dataset  

- <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 576 512"><path d="M384 480l48 0c11.4 0 21.9-6 27.6-15.9l112-192c5.8-9.9 5.8-22.1 .1-32.1S555.5 224 544 224l-400 0c-11.4 0-21.9 6-27.6 15.9L48 357.1 48 96c0-8.8 7.2-16 16-16l117.5 0c4.2 0 8.3 1.7 11.3 4.7l26.5 26.5c21 21 49.5 32.8 79.2 32.8L416 144c8.8 0 16 7.2 16 16l0 32 48 0 0-32c0-35.3-28.7-64-64-64L298.5 96c-17 0-33.3-6.7-45.3-18.7L226.7 50.7c-12-12-28.3-18.7-45.3-18.7L64 32C28.7 32 0 60.7 0 96L0 416c0 35.3 28.7 64 64 64l23.7 0L384 480z"/></svg> <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 576 512"><path d="M160 80l352 0c8.8 0 16 7.2 16 16l0 224c0 8.8-7.2 16-16 16l-21.2 0L388.1 178.9c-4.4-6.8-12-10.9-20.1-10.9s-15.7 4.1-20.1 10.9l-52.2 79.8-12.4-16.9c-4.5-6.2-11.7-9.8-19.4-9.8s-14.8 3.6-19.4 9.8L175.6 336 160 336c-8.8 0-16-7.2-16-16l0-224c0-8.8 7.2-16 16-16zM96 96l0 224c0 35.3 28.7 64 64 64l352 0c35.3 0 64-28.7 64-64l0-224c0-35.3-28.7-64-64-64L160 32c-35.3 0-64 28.7-64 64zM48 120c0-13.3-10.7-24-24-24S0 106.7 0 120L0 344c0 75.1 60.9 136 136 136l320 0c13.3 0 24-10.7 24-24s-10.7-24-24-24l-320 0c-48.6 0-88-39.4-88-88l0-224zm208 24a32 32 0 1 0 -64 0 32 32 0 1 0 64 0z"/></svg> [Hugging Face Dataset](https://huggingface.co/datasets/INS-IntelligentNetworkSolutions/Waste-Dumpsites-DroneImagery)  

### Trained Models  

- <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 640 512"><path d="M96 64c0-17.7 14.3-32 32-32l32 0c17.7 0 32 14.3 32 32l0 160 0 64 0 160c0 17.7-14.3 32-32 32l-32 0c-17.7 0-32-14.3-32-32l0-64-32 0c-17.7 0-32-14.3-32-32l0-64c-17.7 0-32-14.3-32-32s14.3-32 32-32l0-64c0-17.7 14.3-32 32-32l32 0 0-64zm448 0l0 64 32 0c17.7 0 32 14.3 32 32l0 64c17.7 0 32 14.3 32 32s-14.3 32-32 32l0 64c0 17.7-14.3 32-32 32l-32 0 0 64c0 17.7-14.3 32-32 32l-32 0c-17.7 0-32-14.3-32-32l0-160 0-64 0-160c0-17.7 14.3-32 32-32l32 0c17.7 0 32 14.3 32 32zM416 224l0 64-192 0 0-64 192 0z"/></svg> <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 640 512"><path d="M320 0c17.7 0 32 14.3 32 32l0 64 120 0c39.8 0 72 32.2 72 72l0 272c0 39.8-32.2 72-72 72l-304 0c-39.8 0-72-32.2-72-72l0-272c0-39.8 32.2-72 72-72l120 0 0-64c0-17.7 14.3-32 32-32zM208 384c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zm96 0c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zm96 0c-8.8 0-16 7.2-16 16s7.2 16 16 16l32 0c8.8 0 16-7.2 16-16s-7.2-16-16-16l-32 0zM264 256a40 40 0 1 0 -80 0 40 40 0 1 0 80 0zm152 40a40 40 0 1 0 0-80 40 40 0 1 0 0 80zM48 224l16 0 0 192-16 0c-26.5 0-48-21.5-48-48l0-96c0-26.5 21.5-48 48-48zm544 0c26.5 0 48 21.5 48 48l0 96c0 26.5-21.5 48-48 48l-16 0 0-192 16 0z"/></svg> [Hugging Face Models](https://huggingface.co/INS-IntelligentNetworkSolutions/waste-detection-faster_rcnn-mmdetection)

## Acknowledgments  

<svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 512 512"><path d="M336 16l0 64c0 8.8-7.2 16-16 16s-16-7.2-16-16l0-64c0-8.8 7.2-16 16-16s16 7.2 16 16zm-98.7 7.1l32 48c4.9 7.4 2.9 17.3-4.4 22.2s-17.3 2.9-22.2-4.4l-32-48c-4.9-7.4-2.9-17.3 4.4-22.2s17.3-2.9 22.2 4.4zM135 119c9.4-9.4 24.6-9.4 33.9 0L292.7 242.7c10.1 10.1 27.3 2.9 27.3-11.3l0-39.4c0-17.7 14.3-32 32-32s32 14.3 32 32l0 153.6c0 57.1-30 110-78.9 139.4c-64 38.4-145.8 28.3-198.5-24.4L7 361c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0l53 53c6.1 6.1 16 6.1 22.1 0s6.1-16 0-22.1L23 265c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0l93 93c6.1 6.1 16 6.1 22.1 0s6.1-16 0-22.1L55 185c-9.4-9.4-9.4-24.6 0-33.9s24.6-9.4 33.9 0l117 117c6.1 6.1 16 6.1 22.1 0s6.1-16 0-22.1l-93-93c-9.4-9.4-9.4-24.6 0-33.9zM433.1 484.9c-24.2 14.5-50.9 22.1-77.7 23.1c48.1-39.6 76.6-99 76.6-162.4l0-98.1c8.2-.1 16-6.4 16-16l0-39.4c0-17.7 14.3-32 32-32s32 14.3 32 32l0 153.6c0 57.1-30 110-78.9 139.4zM424.9 18.7c7.4 4.9 9.3 14.8 4.4 22.2l-32 48c-4.9 7.4-14.8 9.3-22.2 4.4s-9.3-14.8-4.4-22.2l32-48c4.9-7.4 14.8-9.3 22.2-4.4z"/></svg> We would like to extend our deepest gratitude to the following organizations and platforms for their invaluable support

### [UNICEF Venture Fund](https://www.unicef.org/innovation/venturefund)  

We express our profound gratitude to the UNICEF Venture Fund for their generous support of our project. Their commitment to fostering innovation and sponsoring projects that utilize frontier technology is truly commendable and instrumental in driving positive change.

### [MMDetection](https://github.com/open-mmlab/mmdetection)  

A special thanks to the open-source AI training platform MMDetection. Your robust tools and extensive resources have significantly accelerated our development process.

### Third Party Notices  

Our project would not have been possible without the myriad of libraries and frameworks that have empowered us along the way. We owe a great debt of gratitude to all the contributors and maintainers of these projects.

Thank you to everyone who has made this project possible. We couldn't have done it without you!

_Raven Scan uses third-party libraries or other resources that may be distributed under licenses different than the Raven Scan software._  

_In the event that we accidentally failed to list a required notice, please bring it to our attention by posting an issue on out GitHub Page._  

_Each team member has played a pivotal role in bringing this project to fruition, and we are immensely thankful for their hard work and dedication._  
  
- <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 512 512"><path d="M0 96C0 78.3 14.3 64 32 64l384 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L32 128C14.3 128 0 113.7 0 96zM64 256c0-17.7 14.3-32 32-32l384 0c17.7 0 32 14.3 32 32s-14.3 32-32 32L96 288c-17.7 0-32-14.3-32-32zM448 416c0 17.7-14.3 32-32 32L32 448c-17.7 0-32-14.3-32-32s14.3-32 32-32l384 0c17.7 0 32 14.3 32 32z"/></svg> <svg xmlns="http://www.w3.org/2000/svg"  width=18 viewBox="0 0 640 512"><path d="M272.2 64.6l-51.1 51.1c-15.3 4.2-29.5 11.9-41.5 22.5L153 161.9C142.8 171 129.5 176 115.8 176L96 176l0 128c20.4 .6 39.8 8.9 54.3 23.4l35.6 35.6 7 7c0 0 0 0 0 0L219.9 397c6.2 6.2 16.4 6.2 22.6 0c1.7-1.7 3-3.7 3.7-5.8c2.8-7.7 9.3-13.5 17.3-15.3s16.4 .6 22.2 6.5L296.5 393c11.6 11.6 30.4 11.6 41.9 0c5.4-5.4 8.3-12.3 8.6-19.4c.4-8.8 5.6-16.6 13.6-20.4s17.3-3 24.4 2.1c9.4 6.7 22.5 5.8 30.9-2.6c9.4-9.4 9.4-24.6 0-33.9L340.1 243l-35.8 33c-27.3 25.2-69.2 25.6-97 .9c-31.7-28.2-32.4-77.4-1.6-106.5l70.1-66.2C303.2 78.4 339.4 64 377.1 64c36.1 0 71 13.3 97.9 37.2L505.1 128l38.9 0 40 0 40 0c8.8 0 16 7.2 16 16l0 208c0 17.7-14.3 32-32 32l-32 0c-11.8 0-22.2-6.4-27.7-16l-84.9 0c-3.4 6.7-7.9 13.1-13.5 18.7c-17.1 17.1-40.8 23.8-63 20.1c-3.6 7.3-8.5 14.1-14.6 20.2c-27.3 27.3-70 30-100.4 8.1c-25.1 20.8-62.5 19.5-86-4.1L159 404l-7-7-35.6-35.6c-5.5-5.5-12.7-8.7-20.4-9.3C96 369.7 81.6 384 64 384l-32 0c-17.7 0-32-14.3-32-32L0 144c0-8.8 7.2-16 16-16l40 0 40 0 19.8 0c2 0 3.9-.7 5.3-2l26.5-23.6C175.5 77.7 211.4 64 248.7 64L259 64c4.4 0 8.9 .2 13.2 .6zM544 320l0-144-48 0c-5.9 0-11.6-2.2-15.9-6.1l-36.9-32.8c-18.2-16.2-41.7-25.1-66.1-25.1c-25.4 0-49.8 9.7-68.3 27.1l-70.1 66.2c-10.3 9.8-10.1 26.3 .5 35.7c9.3 8.3 23.4 8.1 32.5-.3l71.9-66.4c9.7-9 24.9-8.4 33.9 1.4s8.4 24.9-1.4 33.9l-.8 .8 74.4 74.4c10 10 16.5 22.3 19.4 35.1l74.8 0zM64 336a16 16 0 1 0 -32 0 16 16 0 1 0 32 0zm528 16a16 16 0 1 0 0-32 16 16 0 1 0 0 32z"/></svg> All [Project Dependencies](https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/development/dependencies/dependencies.html)  
