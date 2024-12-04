# Dataset Management  

_The initial step in detecting Your Kind of Waste_  

### Image preprocessing  

...  

### Create Dataset  

Create a new Dataset  

#### Import Dataset  

##### Requirements  

- The dataset must be in a `.zip` file format.  
- Annotation files must be named `annotations_coco.json`.  

> [!WARNING]  
> MMDetection COCO Format  
> same as the standard COCO Format but without the **NULL Class**.  
> [Official format documentation](https://mmdetection.readthedocs.io/en/latest/user_guides/dataset_prepare.html)  

---

You can import datasets in two ways: **Split Dataset** or **Non-Split Dataset**.  

##### Split Dataset  

For a split dataset, the `.zip` file must contain the following structure:  

- Three folders named exactly:  
- `train`  
- `valid`  
- `test`  
- Each folder should include:  
- The images for the respective split.  
- An `_annotations.coco.json` file.  

##### Non-Split Dataset  

For a non-split dataset, the `.zip` file should contain:  

- All images in the root directory.  
- A single `_annotations.coco.json` file in the root directory.  

#### Manual Image Upload  

- Upload images one by one  
- Through the Edit Dataset page  
  - Click on the Add new dataset image  
  - Choose an image on your device  
  - Choose the part of the image that you want to use  

>[!IMPORTANT]  
> Images must be at least 1280x1280 pixels _for now, will become a dataset setting_  

### Prepare and Annotate Procedure Best Practices  

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

### Publish Dataset  

Preconditions for Dataset Publishing  

- All Images Must Be Enabled  
  - All images associated with the dataset must be enabled. _(Optionally: Enable all images at once)_  
- All Enabled Images Must Have Annotations _(Planned Change)_  
  - In the current implementation, all enabled images in the dataset must have at least one annotation associated with them.  
- Minimum Number of Images Required  
  - The dataset must contain at least 100 images to be eligible for publishing.  
- Minimum Number of Classes Required  
  - The dataset must include at least 1 dataset class linked to it.  

### Delete Image  

- Through the Edit Dataset page  
  - Click on the Edit Image button while hovering over an image  
  - Press the Delete button  
  - Press the Delete button again in the new pop-up  

### Export Dataset  

- Export Dataset for further use  
  - Press the Export Annotations _(will be changed)_ to COCO Format button  
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

> [!TIP]
>
>For more information, check the [**Dataset Management Documentation**](../../documentation/dataset-management/overview.md) here.
