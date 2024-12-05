# 3D Point Cloud Files

### General  

A point cloud is a set of data points in a 3D coordinate system—commonly known as the XYZ axes. Each point represents a single spatial measurement   on the object's surface. Taken together, a point cloud represents the entire external surface of an object.  

- To load and view the point cloud file the platform uses a compatible 3D viewer called [Potree](https://github.com/potree/potree?tab=readme-ov-file).  
- For filtering, transforming and analyzing the point cloud data the platform uses [PDAL - Point Data Abstraction Library](https://pdal.io/en/2.8.1/) and [GDAL - translator library for raster and vector geospatial data formats](https://gdal.org/en/stable).  
- For upload, the software supports .las and .laz file formats (extensions).  
- To use the functionalities that this section contains the user must have successfully completed all steps from the [set-up](../../development/set-up.html), especially the part for "Waste comparison analysis" otherwise it will not work correctly.  

In the 3D Point Cloud and Point Cloud Files section of the waste detection app, you are able to work with 3D data representations of a landfill or waste area. A point cloud is essentially a collection of data points in 3D space, often created by scanning or surveying the area. Each point represents a location on the surface of the waste, and together, they form a detailed 3D model of the area. This model can be used for various analyses, including waste volume measurements and monitoring changes over time.

The app provides several key features to make it easy for you to manage and analyze point cloud data. First, the Upload/Convert File button allows you to add new point cloud files into the system. This could include uploading files directly or converting them from other formats to the ones compatible with the app. Once uploaded, you can use the View Point Clouds button to see and interact with the 3D models, which helps in visualizing the waste and understanding the overall situation.

Additionally, the app offers a Waste Volume Differential Analysis feature. This lets you compare point clouds taken at different times to measure the changes in waste volume, helping track waste accumulation or reduction over time. This can be incredibly useful for monitoring progress in waste management and for planning future efforts.

To make it easier to manage all the point cloud files, there’s a search bar where you can quickly find specific files. You also have the ability to edit, delete, and view any of the point cloud files related to legal landfill data. This ensures that you can maintain an up-to-date and organized database of all the relevant 3D models, ensuring smooth operations when dealing with landfill waste detection.  

### Upload/Convert Point Cloud

The uploading process contains several steps:

- Storing the uploaded file in the database  
- Storing the uploaded file in folder so it could be available for further usage  
- Converting and storing the uploaded point cloud file to relevant Potree file format using [Potree converter](https://github.com/potree/PotreeConverter).  
  - The Potree converter in the background transforms the supported/uploaded point cloud file and creates several files on a different location like hierarchy.bin, octree.bin, log.txt, and metadata.json. Later these files are used by the platform so the user can view the potree cloud file/s.  
- Creating and storing a tif file using PDAL for the needs of further waste differential analysis.

### View Point Clouds  

This is basically the preview of the selected point cloud/s using the Potree 3D viewer.  
On this screen the user can use the options in the right sidebar to take a closer look at the selected point cloud/s.  

- In the Scene option under the Point Clouds object the platform lists all of the selected point clouds for preview, here the user can check to view or uncheck to hide some point cloud from the preview section.  
- The other options contain tools for measuring, clipping, navigating, maintaining the background etc.  

### Compare Point Clouds  

In this section the user is selecting only two point clouds so the platform in the background gets the matching tiff files for the selected point clouds and after this procedure it temporarily creates a tif file which contains necessary data for calculating the volume difference between two point clouds.  
The platform uses a formula to calculate the data from the created tif file and after it is calculated the file is deleted to free up space and also because it is no longer needed.  
In the background the platform always orders the two selected point cloud files by scan date time so the older file is always first.  
For example, with this functionality the user can view the volume difference between two scanned illegal landfills.  

>[!TIP]
>
> For more information, check the [**3D Point Cloud Documentation**](../../documentation/landfill-management/3d-point-cloud.md) here.  
