# Detection Process  

The Detection Process helps you analyze aerial imagery to identify potential illegal dump sites.  
This guide walks you through the complete workflow from uploading images to viewing detection results.  

### Creating Detection Runs  

#### Starting a New Detection Run  

1. Navigate to "Create Detection Run" in the Detection section  
2. Fill in the detection run details:  
   - **Name**: Enter a descriptive name for the detection run  
   - **Description**: (Optional) Add additional information  
   - **Select Trained Model**: Choose from available trained models  
   - **Select Input Image**: Choose a [detection input image](detection-input-images-guide.md) to use  

#### Choosing Detection Parameters  

1. Select a trained model:  
   - Models are listed with their training dates  
   - Published models appear at the top of the list  
2. Select an input image:  
   - Hover over image names to view details  
   - Preview thumbnails are available  
3. Click "Schedule Detection" to start the process  

#### Monitoring Detection Progress  

1. The system will display the current status:  
   - **Waiting**: Detection run is queued  
   - **Processing**: Analysis is in progress  
   - **Success**: Detection completed successfully  
   - **Error**: Detection encountered problems  
2. You can track progress in the Scheduled Runs page  

## Detection Ignore Zones

### Managing Ignore Zones  

#### Creating Ignore Zones

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

#### Using Ignore Zones  

- Active ignore zones automatically filter detection results  
- Areas within ignore zones are marked differently in results  
- Toggle zones on/off to adjust detection visibility  

### Viewing Detection Results  

#### Accessing Results  

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

#### Understanding Result Display  

Results are color-coded:  

- **Red polygons**: Detected dump sites outside ignore zones  
- **Blue polygons**: Detected sites within ignore zones  
- Confidence rates are displayed on each detection  
- Hover over detections for additional details  

#### Filtering and Analysis  

1. Use confidence rate sliders to filter results  
2. Compare multiple detection runs:  
   - Select runs to compare  
   - Use the "Area Comparison" report  
   - View statistics and metrics  

#### Exporting Results  

1. Select detection runs to export  
2. Choose export format (if available)  
3. Download results for external use  

### Troubleshooting  

#### Common Issues  

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

> [!TIP]
>
>For more information, check the [**Detection Guide Documentation**](../../documentation/detection-process/overview.md) here.
