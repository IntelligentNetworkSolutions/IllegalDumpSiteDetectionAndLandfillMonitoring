# 3D Point Cloud Files - Developer documentation

## Overview 

-  A point cloud is a set of data points in a 3D coordinate system—commonly known as the XYZ axes. Each point represents a single spatial measurement   on the object's surface. Taken together, a point cloud represents the entire external surface of an object.
- To load and view the point cloud file the platform uses a compatible 3D viewer called <a href="https://github.com/potree/potree?tab=readme-ov-file">Potree</a>.
- For filtering, transforming and analyzing the point cloud data the platform uses <a href="https://pdal.io/en/2.8.1/">PDAL</a> - Point Data Abstraction Library and <a href="https://gdal.org/en/stable">GDAL</a> - translator library for raster and vector geospatial data formats.
> [!WARNING]
> - To use this section functionalities the user must have successfully completed all steps from the <a href="https://intelligentnetworksolutions.github.io/IllegalDumpSiteDetectionAndLandfillMonitoring/development/set-up.html">Development set-up</a> , especially the part for <b>Waste comparison analysis</b> otherwise it will not work properly.

## Upload/Convert Process
> [!NOTE]
> - First of all, the following application settings must be stored as the table below  

<table style="width: 100%; border-collapse: collapse; table-layout: fixed;">
    <thead>
        <tr>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Key</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Value</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Description</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Data Type</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Module</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">SupportedPointCloudFileExtensions</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">.las, .laz</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Specifies the allowed file extensions for point cloud files.</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">LegalLandfillPointCloudFileUploads</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Uploads\LegalLandfillUploads\PointCloudUploads</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Legal Landfill PointCloud File Uploads</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">LegalLandfillPointCloudFileConverts</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Uploads\LegalLandfillUploads\PointCloudConverts</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Legal Landfill PointCloud File Converts</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">PotreeConverterFilePath</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Should contains the absolute path to potree converter exe (ex. C:\PotreeConverter\PotreeConverter.exe)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Potree Converter File Path</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">PdalExePath</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Should contains the absolute path to pdal exe (ex. C:\miniconda3\envs\waste_comparison_env\Library\bin\pdal.exe)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Pdal exe path</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">PdalPipelineTemplate</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">[ "{INPUT_FILE}", { "filename": "{OUTPUT_FILE}", "gdaldriver": "GTiff", "output_type": "max", "resolution": "0.5", "nodata": -9999, "type": "writers.gdal" } ]</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Pdal Pipeline Template</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>        
    </tbody>
</table>

### The uploading process contains several steps:
1. <i>Checking supported file formats (.las, .laz).</i>
```csharp
//check file support
var fileUploadExtension = Path.GetExtension(viewModel.FileUpload.FileName);
ResultDTO resultCheckSuportingFiles = await _legalLandfillPointCloudFileService.CheckSupportingFiles(fileUploadExtension);
if (!resultCheckSuportingFiles.IsSuccess && resultCheckSuportingFiles.HandleError())
{
    return ResultDTO.Fail(resultCheckSuportingFiles.ErrMsg!);
}
```
2. <i>Storing the uploaded file in database</i>
```csharp
//create the file in database
var resultCreateEntity = await _legalLandfillPointCloudFileService.CreateLegalLandfillPointCloudFile(dto);
if (!resultCreateEntity.IsSuccess && resultCreateEntity.HandleError())
{
    return ResultDTO.Fail(resultCreateEntity.ErrMsg!);
}
```
3. <i>Storing the uploaded file in folder so it could be available for further usage</i>
```csharp
//upload the file in folder                
var fileName = string.Format("{0}{1}", resultCreateEntity.Data.Id.ToString(), fileUploadExtension);
var filePath = Path.Combine(uploadsFolder, fileName);
var uploadResult = await _legalLandfillPointCloudFileService.UploadFile(viewModel.FileUpload, uploadsFolder, resultCreateEntity.Data.Id.ToString() + fileUploadExtension);
if (!uploadResult.IsSuccess && uploadResult.HandleError())
{
    return ResultDTO.Fail("File uploading failed");
}
```
4. <i>Converting and storing the uploaded point cloud file to relevant Potree file format using <a href="https://github.com/potree/PotreeConverter">Potree converter</a>. In the background it transforms the supported/uploaded point cloud file and creates several files on a different location like <i>hierarchy.bin, octree.bin, log.txt, and metadata.json</i>. Later these files are used by the platform so the user can view the potree cloud file/s.</i>
```csharp
//convert file to potree format
var convertsFolder = Path.Combine(_webHostEnvironment.WebRootPath, pointCloudConvertFolder.Data, resultGetEntity.Data.Id.ToString(), resultCreateEntity.Data.Id.ToString());
var convertResult = await _legalLandfillPointCloudFileService.ConvertToPointCloud(potreeConverterFilePath.Data, uploadResult.Data, convertsFolder, filePath);
if (!convertResult.IsSuccess && convertResult.HandleError())
{
    return ResultDTO.Fail("Converting to point cloud failed");
}
//execute command line
 var result = await Cli.Wrap(potreeConverterFilePath)
 .WithArguments($"{filePath} -o {convertsFolder}")
 .WithValidation(CommandResultValidation.None)
 .ExecuteAsync();
```
5. <i>Creating and storing a tif file using PDAL for the needs of further waste differential analysis.</i>
```csharp
//create tif file                
var tifFileName = string.Format("{0}{1}", resultCreateEntity.Data.Id.ToString(), "_dsm.tif");
var tifFilePath = Path.Combine(uploadsFolder, tifFileName);
var tiffResult = await _legalLandfillPointCloudFileService.CreateTifFile(pipelineJsonTemplate.Data, pdalAbsPath.Data, filePath, tifFilePath);
if (!tiffResult.IsSuccess)
{
    return ResultDTO.Fail("Creating TIF file failed");
}
//execute command line
var res = await Cli.Wrap(pdalAbsPath)
    .WithArguments("pipeline --stdin")
    .WithStandardInputPipe(PipeSource.FromString(pipelineJson))
    .WithValidation(CommandResultValidation.None)
    .ExecuteBufferedAsync();
```

## View point cloud/s
### 3D Visualization
1. <i>Select some of the previously uploaded point cloud/s to open up in Potree 3D viewer.</i>
```csharp
//Preview the selected point cloud files - key method 
[HttpGet]
[HasAuthClaim(nameof(SD.AuthClaims.PreviewLegalLandfillPointCloudFiles))]
public async Task<IActionResult> Preview(List<string> selectedFiles)
{
    if (selectedFiles == null || selectedFiles.Count == 0)
    {
        var errorPath = _configuration["ErrorViewsPath:Error"];
        if (errorPath == null)
        {
            return BadRequest();
        }
        return Redirect(errorPath);
    }

    try
    {
        List<Guid> depryptedGuids = new List<Guid>();
        foreach (var item in selectedFiles)
        {
            Guid decryptedGuid = GuidEncryptionHelper.DecryptGuid(item);
            depryptedGuids.Add(decryptedGuid);
        }

        List<PreviewViewModel> listVM = new();
        foreach (var item in depryptedGuids)
        {
            var resultGetEntity = await _legalLandfillPointCloudFileService.GetLegalLandfillPointCloudFilesById(item);
            if (!resultGetEntity.IsSuccess && resultGetEntity.HandleError())
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }
            if (resultGetEntity.Data == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (errorPath == null)
                {
                    return NotFound();
                }
                return Redirect(errorPath);
            }

            string? fileNameWithoutExtension = Path.GetFileNameWithoutExtension(resultGetEntity.Data.FileName);

            PreviewViewModel vm = new()
            {
                FileId = resultGetEntity.Data.Id,
                LandfillId = resultGetEntity.Data.LegalLandfillId,
                FileName = fileNameWithoutExtension ?? resultGetEntity.Data.FileName
            };

            listVM.Add(vm);
        }

        return View(listVM);
    }
    catch (Exception)
    {
        var errorPath = _configuration["ErrorViewsPath:Error"];
        if (errorPath == null)
        {
            return BadRequest();
        }
        return Redirect(errorPath);
    }
}
```
```javascript
//show the selected point cloud in the Potree sidebar
for (let i = 0; i < metadatas.length; i++) {
    Potree.loadPointCloud(metadatas[i].FileUrl, metadatas[i].FileName, e => {
        let scene = viewer.scene;
        let pointcloud = e.pointcloud;

        let material = pointcloud.material;
        material.size = 1;
        material.pointSizeType = Potree.PointSizeType.ADAPTIVE;
        material.shape = Potree.PointShape.SQUARE;

        scene.addPointCloud(pointcloud);

        viewer.fitToScreen();       
    });
}
```

2. <i>Potree 3D viewer sidebar contains additional options for measuring, clipping, navigating, maintaining the background etc.</i>
```javascript
//Potree sidebar adjustment
 viewer.loadGUI(() => {
      viewer.setLanguage('en');
     let section = $(`
                 <h3 id="menu_meta" class="accordion-header ui-widget"><span>Additional actions</span></h3>
                 <div class="accordion-content ui-widget pv-menu-list"></div>
             `);
     let content = section.last();
     content.html(`
             <div class="pv-menu-list">
                  <a href='@Url.Action("ViewPointCloudFiles", "LegalLandfillsManagement", new { Area = "IntranetPortal" })' class="btn btn-xs m-0 bg-gradient-gray text-white">Back to files list</a>
             </div>
             `);
     section.first().click(() => content.slideToggle());
     section.insertAfter($('#menu_about'));
     $("#menu_about").hide();
     $("#potree_languages").hide();
     $("#sidebar_header").hide();
     $("#menu_tools").next().show();           
     viewer.toggleSidebar();

 }); 
```
## Compare Point Clouds
> [!NOTE]
> - First of all, the following application settings must be stored as the table below  
<table style="width: 100%; border-collapse: collapse; table-layout: fixed;">
    <thead>
        <tr>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Key</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Value</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Description</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Data Type</th>
            <th style="background-color: #3170aa; color: white; word-wrap: break-word;">Module</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">PythonExeAbsPath</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Should contains the absolute path to python exe (ex. C:\miniconda3\envs\waste_comparison_env\python.exe)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Python Exe From Environment Abs Path</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">GdalCalcAbsPath</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Should contains the absolute path to gdal_calc (ex. C:\miniconda3\envs\waste_comparison_env\Scripts\gdal_calc.py)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Gdal Calc From Environment Abs Path</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>
        <tr>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">OutputDiffFolderPath</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Uploads\LegalLandfillUploads\TempDiffWasteVolumeComparisonUploads</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Output Diff Folder Path</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">Integer (0)</td>
            <td style="background-color: #f2f2f2; word-wrap: break-word;">null</td>
        </tr>    
    </tbody>
</table>

### The comparing process contains several steps:
1. <i>Select only two point clouds so the application gets the matching tif files for the selected point clouds.</i>
```csharp
//get selected point cloud files
var resultGetEntities = await _legalLandfillPointCloudFileService.GetFilteredLegalLandfillPointCloudFiles(selectedFiles);
if (!resultGetEntities.IsSuccess && resultGetEntities.HandleError())
{
    return ResultDTO<WasteVolumeComparisonDTO>.Fail(resultGetEntities.ErrMsg!);
}
if (resultGetEntities.Data == null || resultGetEntities.Data.Count < 2 || resultGetEntities.Data.Count > 2)
{
    return ResultDTO<WasteVolumeComparisonDTO>.Fail("Expected data is null or does not have required number of elements.");
}
//get tif files for the selected point cloud files
var inputA = Path.Combine(webRootPath, firstElement.FilePath, firstElement.Id.ToString() + "_dsm.tif");
var inputB = Path.Combine(webRootPath, secondElement.FilePath, secondElement.Id.ToString() + "_dsm.tif");
```
2. <i>In the background the application always orders the two selected point cloud files by scan date time so the older file is always first.</i>
```csharp
var orderedList = resultGetEntities.Data.OrderBy(x => x.ScanDateTime).ToList();
```
3. <i>After this procedure creates temporary differential tif file which contains necessary data for calculating the volume difference between two point clouds.</i>
```csharp
 var gdalRes = await Cli.Wrap(pythonExeAbsPath.Data)
     .WithArguments($"\"{gdalCalcAbsPath.Data}\" -A \"{inputA}\" -B \"{inputB}\" --outfile=\"{outputDiffFilePath}\" --calc=\"A-B\" --extent=intersect --overwrite")
     .WithValidation(CommandResultValidation.None)
     .ExecuteBufferedAsync();
```
4. <i>The application read the created differential tif file, uses a formula to calculate the data.</i>
```csharp
//read the file
using (Dataset dataset = Gdal.Open(outputDiffFilePath, Access.GA_ReadOnly))
{
    if (dataset == null)
    {
        return ResultDTO<double?>.Fail("Could not open the TIF file");
    }

    Band band = dataset.GetRasterBand(1);

    double min, max, mean, stdDev;
    band.ComputeStatistics(false, out min, out max, out mean, out stdDev, null, null);

    string statisticsValidPercent = band.GetMetadataItem("STATISTICS_VALID_PERCENT", null);
    string statisticsMean = band.GetMetadataItem("STATISTICS_MEAN", null);

    if (string.IsNullOrEmpty(statisticsValidPercent) || string.IsNullOrEmpty(statisticsMean))
    {
        return ResultDTO<double?>.Fail("Required statistics are missing from the dataset");
    }

    double statisticsValidPercentValue = double.Parse(statisticsValidPercent);
    double statisticsMeanValue = double.Parse(statisticsMean);

    int width = dataset.RasterXSize;
    int height = dataset.RasterYSize;

    double[] geoTransform = new double[6];
    dataset.GetGeoTransform(geoTransform);
    double pixelWidth = geoTransform[1];   
}
```
```csharp
//calculating formula
double wasteVolumeDiffrenceFormula = ((width * pixelWidth * (statisticsValidPercentValue / 100)) *
                                          (height * pixelWidth * (statisticsValidPercentValue / 100)) *
                                          statisticsMeanValue) * -1;
```
5. <i>After all, the temporary differential tif file is deleted to free up space and also because it is no longer needed.</i>
```csharp
 File.Delete(outputDiffFilePath);
```
6. <i>For example, with this functionality the user can view the volume difference between two scanned illegal landfills in cubic meters.</i> 
