# Detection Input Images - Developer Documentation

## Overview  

- The Detection Input Images component manages the upload, storage, and retrieval of images used for dump site detection. - Handling image processing, thumbnail generation, and metadata management.

## Image Processing Pipeline  

### 1. Upload Process  

```mermaid
    A[File Upload] --> B[Validation]
    B --> C[Save Physical File]
    C --> D[Generate Thumbnail]
    D --> E[Create DB Entry]
    E --> F[Return Result]
```

### 2. Thumbnail Generation  

```csharp
// Using GDAL for image processing
BufferedCommandResult result = await Cli.Wrap("gdal_translate")
    .WithArguments($"-of JPEG -outsize 500 0 \"{absGeoTiffPath}\" \"{thumbnailPath}\"")
    .WithValidation(CommandResultValidation.None)
    .ExecuteBufferedAsync();
```

## Configuration Requirements  

### File Storage  

```json
{
    "AppSettings": {
        "DetectionInputImagesFolder": "Uploads\\DetectionUploads\\InputImages",
        "DetectionInputImageThumbnailsFolder": "Uploads\\DetectionUploads\\InputImageThumbnails"
    }
}
```

### Security Configuration  

```csharp
// File upload limits
[RequestSizeLimit(int.MaxValue)]
[RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
```

## Integration Points  

### Frontend Integration  

- Uses Select2 for image selection  
- Implements custom preview modals  
- Handles progress tracking during upload  

### GDAL Integration  

```csharp
public class GdalConfiguration
{
    // GDAL initialization
    Gdal.AllRegister();
    GdalConfiguration.ConfigureGdal();
    
    // Image operations
    using var gdalDataset = Gdal.Open(imagePath, Access.GA_ReadOnly);
}
```

## Files Involved

```mermaid
Controllers/
├── DetectionController.cs
    - Manages input image operations
    - Handles file uploads and processing
    - Controls image display and management

Views/
├── Detection/
    ├── ViewAllImages.cshtml
    └── Shared/
        └── Components/
            └── DetectionInputImagesToolViewComponent/
                └── Default.cshtml

Services/
├── DetectionRunService.cs
    - Contains image management methods
    - Handles image processing logic
    - Manages thumbnail generation

Entities/
└── DetectionInputImage.cs
    - Defines input image data structure
    - Contains image metadata properties

DTOs/
└── DetectionInputImageDTO.cs
    - Data transfer object for image operations

ViewModels/
└── DetectionInputImageViewModel.cs
    - View model for image management UI
```
