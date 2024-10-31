# Dataset Management  

- The initial step in detecting Your Kind of Waste.  

## Imaging  
  
- Images should have the same resolution  
  - 1280x1280 pixels  
- Images should have the same color settings when taken  
  
## Image Pre Processing  

- ...  

## Create Dataset  

- Create a new Dataset  

## Create a Dataset Class

## Add Dataset Class to Dataset  

## Image Upload  

- Upload images one by one  
- Through the Edit Dataset page  
  - Click on the Add new dataset image  
  - Choose an image on your device  
  - Choose the part of the image that you want to use  
    - images must be at least 1280x1280 pixels _for now, will become a dataset setting_  

## Import Dataset  

- Already have an annotated dataset ?
  - we got you  
  - you just have to convert your annotations file/s to the MMDetection COCO Format  
  - same as the COCO Format without the NULL Class  
- Must be in the .zip file format  
- Directories can be split  
- Annotations files should be named __annotations_coco.json__  

## Enabling Images  

### Best Practices Procedure  

- Through the Edit Dataset page  
  - Click on the Edit Image button while hovering over an image  
  - Check Enable Image checkbox  
  - Press the Edit button  
- Through the Image Annotation page  
  - Click the Change Status button while the Status Radio Button is set to Enabled  
- _Optionally: Enable all images at once_  

- Check the annotations in each image, by going throught each image separately and enabling it if satisfactory.  
- Leave Images with annotations which contain annotations about which you are unsure of as disabled images.  
- Consult team about the annotations in the remaining disabled images  
  - think about end goal (what will be detected)  

## Publish Dataset  

- All Images must be Enabled  
- All Images must have Annotations _(will change)_  
- There must be at lease a 100 images in the dataset  

## Delete Image  

- Through the Edit Dataset page  
  - Click on the Edit Image button while hovering over an image  
  - Press the Delete button  
  - Press the Delete button again in the new pop-up  

## Export Dataset  

- Export Dataset for further use  
  - Press the Export Annotations_(will be changed)_ to COCO Format button  
  - Choose the options that suits your case  
    - Export Options  
      - All Images _default option_  
      - Only Enable Images _toogle toogle_  
    - Export as Split Dataset  
      - Export as Single Directory _default option_  
        - all images are in a single directory  
        - a single annotations_coco.json file for all images  
      - Export as Split Directories _toggle toggle_  
        - Split Directories  
          - train  
          - valid  
          - test  
        - images are split randomly into the three directories  
        - each directory has a separate annotations_coco.json file for the images in that directory  
        - _current implementation works only for single class datasets_  
