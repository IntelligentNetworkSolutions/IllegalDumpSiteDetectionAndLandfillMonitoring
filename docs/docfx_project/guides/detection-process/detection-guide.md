# Detection Process User Guide  

## Table of Contents  

1. [Overview](#overview)  
2. [Managing Input Images](#managing-input-images)  
3. [Creating Detection Runs](#creating-detection-runs)  
4. [Managing Ignore Zones](#managing-ignore-zones)  
5. [Viewing Detection Results](#viewing-detection-results)  
6. [Troubleshooting](#troubleshooting)  

## Overview  

- The Detection Process system helps you analyze aerial imagery to identify potential illegal dump sites.  
- This guide walks you through the complete workflow from uploading images to viewing detection results.  

## Managing Input Images  

### Accessing the Input Images Page  

1. Navigate to the Detection section in the main menu  
2. Click on "Detection Input Images"  
3. The system displays a list of all available input images  

### Uploading New Images  

1. Click the "Add Input Image" button  
2. Fill in the required information:  
   - **Name**: Enter a descriptive name for the image  
   - **Description**: Add relevant details about the image  
   - **Date Taken**: Select when the image was captured  
   - **Image File**: Click "Choose File" to select your image  
3. Click "Upload" to start the upload process  
4. Wait for the progress bar to complete  
5. The system will automatically generate a thumbnail  

### Managing Existing Images

- **Preview**: Click the preview button (eye icon) to view the image  
- **Edit**: Use the edit button (pencil icon) to modify image details  
- **Delete**: Click the delete button (trash icon) to remove an image  
  > Note: You cannot delete images that are associated with existing detection runs  

## Creating Detection Runs  

### Starting a New Detection Run  

1. Navigate to "Create Detection Run" in the Detection section  
2. Fill in the detection run details:  
   - **Name**: Enter a descriptive name for the detection run  
   - **Description**: (Optional) Add additional information  
   - **Select Trained Model**: Choose from available trained models  
   - **Select Input Image**: Choose the image to analyze  

### Choosing Detection Parameters  

1. Select a trained model:  
   - Models are listed with their training dates  
   - Published models appear at the top of the list  
2. Select an input image:  
   - Hover over image names to view details  
   - Preview thumbnails are available  
3. Click "Schedule Detection" to start the process  

### Monitoring Detection Progress  

1. The system will display the current status:  
   - **Waiting**: Detection run is queued  
   - **Processing**: Analysis is in progress  
   - **Success**: Detection completed successfully  
   - **Error**: Detection encountered problems  
2. You can track progress in the Map Interface  

## Managing Ignore Zones  

### Creating Ignore Zones

1. Open the Ignore Zones tool in the map interface  
2. Click "Add Ignore Zone"  
3. Draw the zone on the map:  
   - Click to start drawing  
   - Click for each corner point  
   - Double-click to finish  
4. Fill in zone details:  
   - **Name**: Enter a descriptive name  
   - **Description**: Add purpose or notes  
   - **Enabled**: Toggle zone activation  

### Using Ignore Zones  

- Active ignore zones automatically filter detection results  
- Areas within ignore zones are marked differently in results  
- Toggle zones on/off to adjust detection visibility  

## Viewing Detection Results  

### Accessing Results  

You can view results in two ways:  

1. **Historic Data View**:  
   - Click the History tool in the map interface  
   - Select detection runs to display  
   - Adjust confidence thresholds using sliders  
   - Compare multiple detection runs  

2. **Detection Zones List**:  
   - Navigate to "Detected Zones" in the menu  
   - View all completed detection runs  
   - Click to show individual run results  

### Understanding Result Display  

Results are color-coded:  

- **Red polygons**: Detected dump sites outside ignore zones  
- **Blue polygons**: Detected sites within ignore zones  
- Confidence rates are displayed on each detection  
- Hover over detections for additional details  

### Filtering and Analysis  

1. Use confidence rate sliders to filter results  
2. Compare multiple detection runs:  
   - Select runs to compare  
   - Use the "Area Comparison" report  
   - View statistics and metrics  

### Exporting Results  

1. Select detection runs to export  
2. Choose export format (if available)  
3. Download results for external use  

## Troubleshooting  

### Common Issues  

1. **Upload Failures**  
   - Check file format compatibility  
   - Verify file size limits  
   - Ensure proper georeferencing  

2. **Detection Errors**  
   - Verify input image quality  
   - Check trained model compatibility  
   - Review error messages in logs  

3. **Display Problems**  
   - Refresh the map view  
   - Clear browser cache  
   - Check layer visibility settings  

### Getting Help  

- Check error messages for specific guidance  
- Contact system administrators for persistent issues  
- Document steps that led to any errors  

### Best Practices  

1. **Input Images**  
   - Use high-resolution imagery  
   - Ensure proper georeferencing  
   - Keep file sizes manageable  

2. **Detection Runs**  
   - Use descriptive names  
   - Document run parameters  
   - Monitor progress regularly  

3. **Result Analysis**  
   - Start with default confidence thresholds  
   - Compare multiple runs for accuracy  
   - Document significant findings  

4. **System Performance**  
   - Limit concurrent detection runs  
   - Clean up unused data  
   - Regular system maintenance  
