# Detection Run Process

- [Detection Input Images Docs](/documentation/detection-process/detection-input-images.md)  
- [DetectionRunService and Controller Docs](/documentation/detection-process/detection-runs.md)  
- [Detection Ignore Zones Docs](/documentation/detection-process/detection-ignore-zones.md)  

___Dev Insight___  

## Preparation  

### Add an Input Image  

- the image should be an ortophoto _preferably a __drone image___  
- the input image must be in the __correct projection__ _(currently 6316)_ to view on the map  
- - absolute path to input image is sent as a parameter to MMDetection  

### Choose a suitable Object-Detection Model

- choose a Trained Model.
  - affects number and quality of detected objects  
  - models are enviroment dependent i.e. the quality and correctness of detection is determined by the content of the images on which they were trained  

> [!NOTE]
>
> - example:
>   - the initial seed model "INSWasteDetection" will detect snow as waste, since none of the images in its training set did not contain snow, it recognizes snow as waste.
>    - this is fixed, by taking images of waste while snow is present, afterwhich a dataset would be created which contains snowy images.
>    - in some cases it is enough only to annotate the object of interest while the similar object which is not of interest is present in the image for the model to infer that two different objects are at play.


  - absolute path to the model's config file is sent as a parameter to MMDetection  
    - /mmdetectionroot/ins-development/trainings/traningRunId/traningRunId.py  
  - absolute path to the model's model file is sent as a parameter to MMDetection  
    - /mmdetectionroot/ins-development/trainings/traningRunId/epoch_{best_epoch}.pth  
  - training run directories and files are generated during the training process or the initial seeding process  
  - paths in config must be valid  
  - _might need to check backbone appsettings.json variable, should be addressed in setup_

## Schedule Detection Run  

- __Up Front__
  - Nice Notification Pops-Up  
  - Detection Run status can be checked in the Schedules Runs section  
  
> [!WARNING]
> - _In the Background_
>   - A Detection Run is Scheduled through Hangfire .
>     - There is only one queue for both the Training and the Detection process.
>     - This is due to concurrency errors and resources availablity concerns.
>     - _currently a detection run can be cancelled only if it is in the waiting status_
>     - More queue managmenet features incoming.
>   - MMDetection is called using CLIWrap as a background process  
>     - initially seeded script  

> ## Success  
>
> The Detection Run finished successfully  
> MMDetection outputs the annotations file containing the detected objects  
> The annotations are converted as DetectedDumpSite Entities  
>  - COCO Format  
>  - Output Bounding Boxes: xMin, yMin, xMax, yMax  
>  - /mmdetectionroot/ins-development/detections/detectionRunId/detection_bboxes.json  
>  - Each annotation(bounding-box) is converted as an Entity with a Polygon(bounding-box) Geom  
>  - MMDetection outputs two images with the object detection visualized
>    - Debugging purpouses _will be removed_  

## View Results  

### Detected Zones _Single Detection_  

- From the Detected Zones Page choose the Action view detection run  
- Loads the Input Image and the detected dump sites for the chosen detection run  

### Map Tool _Multiple Overlapping Detections_  

- From the Right Sidebar choose the Historic Data tool  
- Check the Detection Runs you want shown on the map  
- Loads the Input Images and the detected dump sites for the chosen detection runs  

### Manipulate Confidance Rate

- See more certain or less certain detections  
  - Splide the slider while the detection run is checked and show on map  

### Manipulate Opacity  

- Decrease or Increase the Opacity on the Detected Dumpsites _(Polygons / Bounding Boxes)_  
- From the right Sidebar choose the first option "Layers"  
- Click the Options ... for the Historic Data Layer  
- Decrease or Increase the Opacity _immediate effect_  

## Files Involved

```mermaid
Controllers/
├── DetectionController.cs
    - Manages detection run operations
    - Handles run creation and monitoring
    - Processes results display

Views/
├── Detection/
    ├── CreateDetectionRun.cshtml
    ├── DetectedZones.cshtml
    └── Shared/
        └── Components/
            └── HistoricDataViewComponent/
                └── Default.cshtml

Services/
├── DetectionRunService.cs
    - Orchestrates detection process
    - Manages run lifecycle
    - Processes detection results
├── MMDetectionConfigurationService.cs
    - Handles ML model configuration
    - Manages detection parameters

Entities/
├── DetectionRun.cs
    - Defines run structure
    - Tracks process status
└── DetectedDumpSite.cs
    - Stores detection results
    - Contains spatial data

DTOs/
├── DetectionRunDTO.cs
└── DetectedDumpSiteDTO.cs

ViewModels/
└── DetectionRunViewModel.cs

Configuration/
└── MMDetectionConfiguration.cs
    - ML model settings
    - Detection parameters
```
