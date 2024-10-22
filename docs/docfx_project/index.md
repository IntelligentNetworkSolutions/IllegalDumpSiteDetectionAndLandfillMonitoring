<h1 align="center">
    Illegal Dump Site Detection and Landfill Monitoring <img align="center" width="250" src="/images/logo-robot-formal.png" alt="">  
</h1>

## Overview

Welcome to the Illegal Dump Site Detection and Landfill Monitoring platform documentation. This system leverages advanced drone and satellite imagery to enhance waste management and environmental monitoring through cutting-edge technology.

Our platform utilizes high-resolution images combined with sophisticated image annotation, object detection models, and geospatial analysis to offer robust tools for identifying illegal dump sites and effectively managing regulated landfills.

### Key Features

#### Dataset Management

- [Dataset Management Overview](/docfx_project/documentation/dataset-management/overview.md)
- Manage extensive datasets of drone and satellite images
- Tools for uploading, categorizing, and maintaining image data
- Features include tagging, filtering, and robust data integrity checks

#### Image Annotation

- Part of [Dataset Management Process](/docfx_project/documentation/dataset-management/overview.md)
- Annotate high-resolution drone and satellite imagery
- Train object detection models for precise waste detection

#### Object Detection Model Training

- [Training Process Guide](/docfx_project/guides/training-process/training-user-guide.md)
- [Training Process Detailed Overview](/docfx_project/documentation/training-process/overview.md)
- Train sophisticated models with diverse image datasets
- Enhanced detection accuracy across varied environmental conditions

#### Detection and Monitoring

- [Detection Process Documentation](/docfx_project/documentation/detection-process/overview.md)
- Deploy pre-trained and custom-trained models
- Visualize results on georeferenced maps

#### Landfill Management

- [Landfill Management Documentation](/docfx_project/documentation/landfill-management/overview.md)
- Advanced tools for legal landfill management
- Waste form submission integration
- 3D point cloud scan integration

### System Architecture

Our platform integrates several key technologies:

#### MVC App Components

| Component | Technology/Tool |
|-----------|----------------|
| Web Framework | .NET 8 |
| ORM | Entity Framework |
| Package Manager | npm |
| Frontend Library | Open Layers |
| Geographic Library | NetTopologySuite |
| Database | PostgreSQL 16 |
| Database Extension | PostGIS |
| GIS Server | Geoserver |

#### Object Detection Environment

| Component | Requirement | Version | Optional |
|-----------|------------|----------|-----------|
| MMDetection | Miniconda | 3 | |
| | Python | 3.8 | |
| | C++ | 14 | |
| | Pytorch | *depends* | *CUDA* |

### Documentation Structure

- [Training Process](/docfx_project/documentation/training-process/overview.md)
  - Model training workflow
  - Configuration management
  - Performance monitoring
  
- [Detection Process](/docfx_project/documentation/detection-process/overview.md)
  - Detection pipeline
  - Result visualization
  - Performance optimization

- [Dataset Management](/docfx_project/documentation/dataset-management/overview.md)
  - Data organization
  - Annotation workflow
  - Quality control

- [Landfill Management](/docfx_project/documentation/landfill-management/overview.md)
  - Waste tracking
  - 3D scanning
  - Reporting systems

### Getting Started

- [User Guide Home](/docfx_project/guides/overview.md)
- [Creating Your First Training Run](/docfx_project/guides/training-process/training-guide.md#creating-training-runs.md)
- [Managing Models](/docfx_project/guides/training-process/training-guide.md#managing-models)
- [Troubleshooting Guide](/docfx_project/guides/training-process/training-guide.md#troubleshooting)

### Development Resources

- [Development Setup Guide](/docfx_project/documentation/getting-started.md)
- [Contributing Guidelines](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/blob/master/CONTRIBUTING.md)
- [Code of Conduct](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/blob/master/CODE_OF_CONDUCT.md)

## License

This project is licensed under the __Apache License 2.0__. For more details, see the [full license text](https://github.com/IntelligentNetworkSolutions/IllegalDumpSiteDetectionAndLandfillMonitoring/blob/master/LICENSE.md).

## Acknowledgments

- Sponsor: **UNICEF Venture Fund**

We express our profound gratitude to the UNICEF Venture Fund for their generous support of our project. Their commitment to fostering innovation and sponsoring projects that utilize frontier technology is truly commendable and instrumental in driving positive change.

---

*This documentation is maintained as part of the Illegal Dump Site Detection and Landfill Monitoring project.*
