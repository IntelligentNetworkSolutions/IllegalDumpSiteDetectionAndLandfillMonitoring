# Project Charter  

## Mission  

Our mission is to provide an efficient and accurate way to identify and locate illegal waste dumpsites, to help in the management and reduction of waste.  

The proposed system utilizes the powers of machine learning (ML) and artificial inteligence (AI) through photogrammetric analysis to analyze satellite or drone images in order to identify illegal waste.  

Through computer vision the platform will detect illegal dumpsites as a set of vector polygons that mark all the detected illegal landfills.  

## Vision  

Creating a robust, accurate, and user-friendly tool to visualize, interpret, and act upon detected waste information.  

Contributing to more sustainable and environmentally friendly practices, and in doing so encouraging collaboration between different stakeholders to collectively address and mitigate the issue of polution.

Our strategic vision involves expanding the current usage to other geographic areas with similar environmental challenges, through localization and customization.  

## Community Statement  

We are committed to fostering collaboration and community engagement in the fight against waste pollution.  

By providing optional open access to aggregated, anonymized data and promoting information sharing among stakeholders, we aim to build a global community dedicated to sustainable waste management practices.  

Through educational initiatives and partnerships, we aspire to raise awareness and inspire collective action for a cleaner and healthier planet.  

## Licensing Strategy  

The waste detection software will adopt an open-source licensing strategy to encourage widespread adoption, collaboration, and continuous improvement.  

The core software components and algorithms will be made available under a permissive open-source license, fostering innovation and enabling developers, researchers, and organizations to contribute to the evolution of the technology.  

Additionally, we may offer premium services, support, and customization options for commercial entities and advanced users.  

## Deliverables  

1. [__.NET MVC Web App__](#mvc-app-mainapp)  
    1. [Dataset Management Module](#dataset-management-module)  
    2. [AI Model Training Module](#ai-model-training-module)
    3. [Waste Detection Module](#waste-detection-module)  
    4. [Legal Landfill Management Module](#legal-landfill-management-module)  
    5. [User Management Module](#user-management-module)  
    ...
2. [__.NET API AI Object-Detection Training and Detection__](#api-mlagent)  
    1. Model Training Endpoints  
    2. Object Detection Endpoints  
    3. Reporting Endpoints  
3. [__Python AI Object-Detection Training and Detection Scripts__](#python-scripts)  
    1. [MMDetection Platform Scripts](#mmdetection-platform-scripts)
    2. [Dataset Preparation Scripts](#dataset-preparation-scripts)  
    3. [AI Model Training Scripts](#model-training-scripts)  
    4. [Object Detection Scripts](#model-inference-detection-scripts)  

</br>

## Main components  

### MVC App (MainApp)

#### Dataset Management Module

- Uploading images  
- Pre-processing images  
- Annotating images  
- Reviewing images  

#### AI Model Training Module  

- Train a new Model  
- Publish a Trained Model
- Reinforce Published(Trained) Model  
- Trained Models Management  

#### Waste Detection Module  

- Detect waste on an input image
- Review detections on map  

#### Legal Landfill Management Module  

- Registering Legal Landfills  
- Incoming Waste Tracking  
- Waste Heaps Registration and Tracking
- Input and View for Point-Cloud 3D Scans of Waste Heaps  
- Reporting on Lanfill and Waste Heaps reduction, incoming waste amound, type ...

#### User Management Module  

- Manage Users  
- Manage Roles  

</br>

### API (MLAgent)  

#### Model Training Endpoints  

#### Object Detection Endpoints  

#### Reporting Endpoints  

</br>

### Python Scripts  

#### MMDetection Platform Scripts  

- MMDetection Platform Pre-Defined Scripts  
- Python, PyTorch, libraries  

#### Dataset Preparation Scripts  

- Compatibility Checks  
- Image Checks  
- Annotation Checks  

#### Model Training Scripts  

- CPU/GPU options  
- Faster-R-CNN algorithm  
- Automatic Batch Size and Sampling Rate Calculation  
- Standardized training settings  
- Optional training settings  

#### Model Inference (Detection) Scripts

- Trained Model Inference  
- Large Image Inference  

</br>

### Database  

- annotated image datasets  
- trained object-detection AI models and training runs  
- detected waste results and detection runs(inference)  
- legal landillfs, waste heaps, point-cloud scans  
- users, roles, settings  
...  

</br>

## Project team  

### Sponsor  

- UNICEF Innovation Fund  

### Team  

- Technical Team lead  
- Senior Software developer  
- Intermediate Software developer  
- Project manager  
- General Manager  
- Quality assurance tester  
- GIS expert  
- Marketing Specialist  
