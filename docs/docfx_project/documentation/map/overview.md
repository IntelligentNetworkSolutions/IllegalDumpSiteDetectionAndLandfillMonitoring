## General info

- The application uses <a href="https://openlayers.org/">OpenLayers</a> library to render a dynamic map. 
- The map is rendered using a view component.
```csharp
 public async Task<IViewComponentResult> InvokeAsync(string mapDivId, string mapToLoad)
 {
     var mapConf = await _mapConfigurationService.GetMapConfigurationByName(mapToLoad);

     if (mapConf.Id == Guid.Empty)
     {
         return View("Error");
     }

     var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
     var olMapHelper = new OpenLayersMapHelper(mapConf, language, _configuration);
     var mapOverviewUrl = await _applicationSettingsService.GetApplicationSettingByKey("MapOverviewUrl");

     var model = new MapModel
     {
         MapConfiguration = mapConf,
         LayersJavaScript = olMapHelper.GetLayersJavaScript(),
         MapDivId = mapDivId,
         MapOverviewUrl = mapOverviewUrl?.Value ?? ""
     };

     return View(model);
 }
```
- The map contains several map tools to make map usage interactive and more dynamic.
- Every map tool is a separate view component added to the map view.
```csharp
 <div id="map" class="map turbosidebar-map" style="height:700px; width:100%; position:relative;">
     @await Component.InvokeAsync("Map", new { mapDivId = "map", mapToLoad = "mainmap" })
     @await Component.InvokeAsync("LastExtentTool")
     @await Component.InvokeAsync("ZoomToExtentTool")
     @await Component.InvokeAsync("MeasureLengthTool")
     @await Component.InvokeAsync("MeasureAreaTool")
     @await Component.InvokeAsync("GoToCoordinatesTool")
     @await Component.InvokeAsync("HistoricData", new { detectionRunId = Model})
     @await Component.InvokeAsync("DetectionIgnoreZonesTool")     
     @await Component.InvokeAsync("DetectionInputImagesTool")     
 </div>
```  

## Map Tools

### Last extent tool
- This map tool enables users to navigate back to the previous view extent on the map. 
- This functionality is implemented using JavaScript, integrated into the map's event lifecycle, and connected to a custom tool interface.
##### Implementation (Script Details)
1. Tracking extent: The moveend event listener updates the lastExtent and newExtent variables whenever the map view changes
```javascript
mapVars.map.on('moveend', lastExtentOnMoveEnd);

function lastExtentOnMoveEnd(evt) {
    var map = evt.map;
    lastExtent = newExtent;
    newExtent = map.getView().calculateExtent(map.getSize());
}
```
2. Tool extent: The activateLastExtentTool function uses the lastExtent variable to adjust the map view back to the previous extent with animation
```javascript
function activateLastExtentTool() {
    mapVars.deactivateTools();
    if (typeof lastExtent !== 'undefined') {
        mapVars.map.getView().fit(lastExtent, { nearest: true, size: mapVars.map.getSize(), duration: 500 });
    }
}
```
3. Tool registration: The LastExtentTool is registered in the map tools list and linked to the activation function
```javascript
var lastExtentTool = new MapToolBaseClass("LastExtentTool");
lastExtentTool.activateTool = activateLastExtentTool;
mapVars.mapTools.push(lastExtentTool);
```
4. User interface: A tab is added to the map sidebar for easy access, and a tooltip provides additional context
```javascript
var tabHtml = `<li>
                    <a id="map-sidebar-lastextenttool-tool-btn" role="tab" data-toggle="tooltip" data-trigger="hover" data-placement="left" title="" data-original-title="@DbResHtml.T("Go to Last Extent", "Resources")">
                        <i class="fa fa-sort"></i>
                    </a>
                </li>`;
var tab = $(tabHtml).appendTo("#map-sidebar-upper-tabs-list")[0];
```
5. Event binding: Clicking the sidebar button activates the Last Extent Tool
```javascript
$(tab).on("click", function (evt) {
    lastExtentTool.activateTool();
});
```

### Zoom to extent tool
- The Zoom to Extent Tool zoom into a specific area of the map by drawing a rectangle. 
- It provides an intuitive way to focus on a particular region, enhancing the map navigation experience.
##### Implementation (Script Details)
1. Activate Tool: Initializes a Vector source for drawing interactions, configures a "box" geometry for the Draw interaction to allow rectangular selection and adds the interaction to the map and zooms to the selected area upon the drawend event
```javascript
function activateZoomTool() {
    mapVars.deactivateTools();
    var zoomToExtentSource = new ol.source.Vector({ wrapX: false });

    this.drawInteraction = new ol.interaction.Draw({
        source: zoomToExtentSource,
        type: 'Circle',
        geometryFunction: ol.interaction.Draw.createBox(),
    });

    this.drawInteraction.on('drawend', function (e) {
        var extent = e.feature.getGeometry().getExtent();
        mapVars.map.getView().fit(extent, {
            size: mapVars.map.getSize(),
            duration: 500
        });
    });

    mapVars.map.addInteraction(this.drawInteraction);
    this.isActive = true;
    $("#map-sidebar-zoomtoextent-tool-btn").addClass("active");
}
```
2. Tool registration: The tool is registered in the map tools list and linked to activation/deactivation methods
```javascript
var zoomTool = new MapToolBaseClass("ZoomToExtentTool");
zoomTool.activateTool = activateZoomTool;
zoomTool.deactivateTool = deactivateZoomTool;
mapVars.mapTools.push(zoomTool);
```
3. UI: A sidebar button is added with a tooltip and an icon for easy access
```javascript
var tabHtml = `<li id="map-sidebar-zoomtoextent-tool-btn">
                    <a role="tab" data-toggle="tooltip" data-trigger="hover" data-placement="left" title="" data-original-title="@DbResHtml.T("Zoom to area", "Resources")">
                        <i class="fa fa-expand"></i>
                    </a>
                </li>`;
var tab = $(tabHtml).appendTo("#map-sidebar-upper-tabs-list")[0];
```
4. Event binding: Clicking the button toggles the tool's activation state
```javascript
$(tab).on("click", function (evt) {
    if ($(this).hasClass("active")) {
        zoomTool.deactivateTool();
    } else {
        zoomTool.activateTool();
    }
});
```
### Measure length tool
- The Measure Length Tool measures distances on the map. 
- It integrates with OpenLayers (ol) for interactive drawing and measurement features.
##### Implementation (Script Details)
1. Tool initialization: The tool is based on MapToolBaseClass, it registers itself with mapVars.mapTools for easy activation and deactivation. Also there is a button in the sidebar for user interaction
```javascript
var measureLengthTool = new MapToolBaseClass("MeasureLengthTool");
measureLengthTool.activateTool = activateMeasureLengthTool;
measureLengthTool.deactivateTool = deactivateMeasureLengthTool;

mapVars.mapTools.push(measureLengthTool);
var tabHtml = `<li id="map-sidebar-measurelength-tool-btn">
                   <a role="tab" data-toggle="tooltip" ...>
                       <i class="fa fa-ruler"></i>
                   </a>
               </li>`;
```
2. Tool activation: adds a drawInteraction for drawing lines, adds a vector layer (measureLengthLayer) to the map for rendering the measurement and displays the measurement dynamically
```javascript
function activateMeasureLengthTool() {
    mapVars.deactivateTools();
    mapVars.map.addLayer(this.measureLengthLayer);
    createMeasureTooltip(this);
    createHelpTooltip(this);
    mapVars.map.addInteraction(this.drawInteraction);
    this.isActive = true;
}
```
3. Drawing: Uses OpenLayers ol.interaction.Draw for drawing, calculates distance using ol.sphere.getLength() and provides real-time updates in a floating tooltip during the drawing
```javascript
this.drawInteraction.on('drawstart', function (evt) {
    sketch = evt.feature;
    listener = sketch.getGeometry().on('change', function (evt) {
        var geom = evt.target;
        var output = formatLength(geom);
        objRef.measureTooltipElement.innerHTML = output;
        objRef.measureTooltip.setPosition(geom.getLastCoordinate());
    });
});
```
4. Distance formatting: Distances below 100 meters are shown in meters, distances above 100 meters are converted to kilometers
```javascript
var formatLength = function (line) {
    var length = ol.sphere.getLength(line, { projection: mapVars.projection });
    return length > 100
        ? Math.round((length / 1000) * 100) / 100 + ' km'
        : Math.round(length * 100) / 100 + ' m';
};

```
### Measure area tool
- This map tool measures areas by drawing polygons on a map. 
- The tool integrates with OpenLayers (ol) and provides an interactive way to calculate areas in square meters or square kilometers.

##### Implementation (Script Details)
1. Tool initialization: The tool extends MapToolBaseClass and registers itself in the mapVars.mapTools array for centralized management. Also there is a button in the sidebar for user interaction
```javascript
var measureAreaTool = new MapToolBaseClass("MeasureAreaTool");
measureAreaTool.activateTool = activateMeasureAreaTool;
measureAreaTool.deactivateTool = deactivateMeasureAreaTool;
mapVars.mapTools.push(measureAreaTool);

var tabHtml = `<li id="map-sidebar-measurearea-tool-btn">
                  <a role="tab" data-toggle="tooltip" data-trigger="hover" data-placement="left" 
                     title="@DbResHtml.T("Measure area", "Resources")">
                     <i class="far fa-square"></i>
                  </a>
              </li>`;
$(tabHtml).appendTo("#map-sidebar-upper-tabs-list");
```
2. Tool activation: Enables the polygon drawing interaction and creates a vector layer for displaying polygons
```javascript
function activateMeasureAreaTool() {
    mapVars.deactivateTools();
    mapVars.map.addLayer(this.measureAreaLayer);
    createMeasureTooltip(this);
    createHelpTooltip(this);
    mapVars.map.addInteraction(this.drawInteraction);
    this.isActive = true;
    $("#map-sidebar-measurearea-tool-btn").addClass("active");
}
```
3. Tool deactivation: Cleans up the map by removing the vector layer, tooltips, and drawing interaction, also resets the tool state and deactivates the sidebar button.
```javascript
function deactivateMeasureAreaTool() {
    mapVars.map.removeInteraction(this.drawInteraction);
    mapVars.map.removeLayer(this.measureAreaLayer);
    mapVars.map.removeOverlay(this.helpTooltip);

    this.mapOverlays.forEach(function (overlay) {
        mapVars.map.removeOverlay(overlay);
    });
    this.mapOverlays = [];
    this.isActive = false;

    $("#map-sidebar-measurearea-tool-btn").removeClass("active");
}
```

4. Drawing: Uses ol.interaction.Draw with type: "Polygon" for area measurements and formats area output dynamically
```javascript
this.drawInteraction.on('drawstart', function (evt) {
    sketch = evt.feature;
    listener = sketch.getGeometry().on('change', function (evt) {
        var geom = evt.target;
        var output = formatArea(geom);
        objRef.measureTooltipElement.innerHTML = output;
        objRef.measureTooltip.setPosition(geom.getInteriorPoint().getCoordinates());
    });
});
```

### Go to coordinates tool
- The tool is implemented as part of the map sidebar, activate the tool, input coordinates, and navigate to the specified location on the map.

##### Implementation (Script Details)
1. Tool Initialization: Defines and registers the tool in the mapVars.mapTools array
```javascript
var goToCoordinatesTool = new MapToolBaseClass("GoToCoordinatesTool");
goToCoordinatesTool.activateTool = activateGoToCoordinatesTool;
goToCoordinatesTool.deactivateTool = deactivateGoToCoordinatesTool;
goToCoordinatesTool.defaultFitPadding = [30, 580, 30, 30];
mapVars.mapTools.push(goToCoordinatesTool);
```
2. Activate and Deactivate Tool: Manages tool state and sidebar visibility
```javascript
function activateGoToCoordinatesTool() {
    mapVars.deactivateTools(); // Deactivate all other tools
    $("#map-sidebar-gotocoordinatestool-pane").addClass("active");
}

function deactivateGoToCoordinatesTool() {
    this.isActive = false;
    $("#map-sidebar-gotocoordinatestool-pane").removeClass("active");
    return true;
}
```
3. Navigate to Coordinates: Validates inputs and fits the map view to the provided coordinates
```javascript
$("#gotocoordinatestool-button").on("click", function (e) {
    var lat = $("#gotocoordinatestool-latitude-input").val();
    var long = $("#gotocoordinatestool-longitude-input").val();
    if (isNaN(parseFloat(lat)) || isNaN(parseFloat(long))) {
        notifyAlerts("danger", "Please enter valid decimal values for latitude and longitude!");
    } else if (lat < -90 || lat > 90 || long < -180 || long > 180) {
        notifyAlerts("danger", "Latitude must be between -90 and 90, and Longitude between -180 and 180.");
    } else {
        var point = new ol.geom.Point([parseFloat(long), parseFloat(lat)]).transform('EPSG:4326', mapVars.projection);
        mapVars.map.getView().fit(point.getExtent(), { duration: 1500, padding: goToCoordinatesTool.defaultFitPadding });
    }
    e.preventDefault();
});
```
### Detection input images tool
- This tool interacts with detection input images on the map.
- Activate the tool, view a list of available detection input images, select images to display them on the map and finally show selected images on the map as map layers.
##### Implementation (Script Details)
1. Tool Initialized as a new instance of MapToolBaseClass, also there is a button in the sidebar for user interaction
```javascript
var detectionInputImagesTool = new MapToolBaseClass("DetectionInputImagesTool");
$("#map-sidebar-upper-tabs-list").append(tabHtml);
```
2. Tool activation and deactivation: set-up and reset the UI state 
```javascript
 function activateDetectionInputImagesTool() {
     mapVars.deactivateTools();
     $("#map-sidebar-detectioninputimagestool-pane").addClass("active");
     var groupLayer = mapVars.findLayerByName('DetectionRunImages');
     if (groupLayer) {
         var layers = groupLayer.getLayers().getArray();
         layers.forEach(function (layer) {
             var layerId = layer.get('imgId');
             selectCheckboxByImgId(layerId);
         });
     }
 }

 function deactivateDetectionInputImagesTool() {
     this.isActive = false;
     $("#map-sidebar-detectioninputimagestool-pane").removeClass("active");
     return true;
 }
```
3. Populates the table with input images data using an AJAX GET request to the backend
```javascript
 $.ajax({
     type: "GET",
     url: "@Url.Action("GetAllDetectionInputImages", "Detection", new { Area = "IntranetPortal" })",
 })
function drawTableRowsForEachInputImage(inputImagesList, detectionInputImagesTable) {
    inputImagesList.forEach(function (item) {
        detectionInputImagesTable += `<tr> ... </tr>`;
    });
    return detectionInputImagesTable;
}
```
4. Map integration: Finds DetectionRunImages group layer, adds layers dynamically for selected input images and adjusts map extent to fit all layers.
```javascript
var groupLayer = mapVars.findLayerByName('DetectionRunImages');    
 groupLayer.getLayers().clear();
 var combinedExtent = ol.extent.createEmpty();

 var inputImageList = data.data;
 inputImageList.forEach(function (inputImage, index) {
     const source = new ol.source.GeoTIFF({
         sources: [
             {
                 url: inputImage.imagePath
             },
         ],
     });
     
     var newLayer = new ol.layer.WebGLTile({
         source: source,
         imgId: inputImage.id,
         name: inputImage.name,
         projection: mapVars.projection,
         title: inputImage.name,
         order: index + 1,
         visible: true
     })
     
     groupLayer.getLayers().push(newLayer);                                   
          
     source.on('change', () => {
         if (source.getState() === 'ready') {
             const extent = source.getTileGrid().getExtent();
             ol.extent.extend(combinedExtent, extent);
             mapVars.map.getView().fit(combinedExtent, mapVars.map.getSize());
         }
     });
 });
 groupLayer.set('displayInLayerSwitcher', true);
```

### Historic data tool
- This tool interacts with detection runs and detection input images on the map.
- Activate the tool, view a list of successfully completed detection runs, choose confidence rate percentage, select detection runs to display them on the map and finally show selected detected dump sites on the map.
- Also this tool provides report for area comparison of the selected and detected dump sites.
##### Implementation (Script Details)
1. Tool Initialized as a new instance of MapToolBaseClass, also there is a button in the sidebar for user interaction
```javascript
var historicData = new MapToolBaseClass("HistoricData");
$("#map-sidebar-upper-tabs-list").append(tabHtml);
```
2. Tool activation and deactivation: set-up and reset the UI state 
```javascript
   function activateHistoricDataTool() {
      mapVars.deactivateTools();
      $("#map-sidebar-historicdata-pane").addClass("active");
  }

  function deactivateHistoricDataTool() {
      this.isActive = false;
      $("#map-sidebar-historicdata-pane").removeClass("active");
      return true;
  }
```
3. Populates the table with detection runs data using an AJAX GET request to the backend
```javascript
 $.ajax({
     type: "GET",
     url: "@Url.Action("GetAllDetectionRuns", "Detection", new { Area = "IntranetPortal" })",
     success: function (data) {
        data.forEach(function (item) {
        var detectionRunsTable = '';
        var checked = item.id === '@Model' ? "checked" : "";
        detectionRunsTable +=
            `<tr>
            <td hidden>${item.id}</td>
                <td style="max-width: 200px; word-wrap: break-word; white-space: normal;">${item.name}</td>
            <td>
                <input class="conf_rate_slider" id="conf_rate_range_${item.id}" type="text" name="conf_rate_range_${item.id}" value="">
            </td>
                    <td>${new Date(item.createdOn).toLocaleDateString('en-GB').replace(/\//g, '.')}</td>
            <td>
                <div class="form-group clearfix">
                    <div class="icheck-dark mt-3 text-bold">
                        <input type="checkbox" id="editDatasetImageEnabledInput_${item.id}" name="editDatasetImageEnabledInput_${item.id}" ${checked}>
                        <label for="editDatasetImageEnabledInput_${item.id}"></label>
                    </div>
                </div>
            </td>
        </tr>
        `
        });
    }
 }) 
```
4. Map integration: Creates group layer historic data, gets the suitable deteciton input image layer, add layers dynamically for selected detection runs above the selected confidance rate  and adjusts map extent to fit all layers.
```javascript
var historicDataLayerGroup = new ol.layer.Group({
    title: 'Historic Data',
    name: 'Historic Data',
    order: 3,
    opacity: 1,
    visible: true,
    openInLayerSwitcher: true,
    fold: 'open',
    layers: new ol.Collection([])
});
var detectionRunImagesGroupLayer = mapVars.findLayerByName('DetectionRunImages');
var newLayer = new ol.layer.WebGLTile({
    source: source,
    name: detectionRun.detectionInputImage.name,
    imgId: detectionRun.detectionInputImage.id,
    projection: mapVars.projection,
    title: detectionRun.detectionInputImage.name,
    order: index + 1,
    visible: true
})

detectionRunImagesGroupLayer.getLayers().push(newLayer);

 var vectorSource = new ol.source.Vector(); 

 detectionRun.detectedDumpSites.forEach(function (dumpSite) {
     var geoJsonObj = JSON.parse(dumpSite.geoJson);
     var olGeoJson = new ol.format.GeoJSON();
     var feature = olGeoJson.readFeature(geoJsonObj, {
         dataProjection: mapVars.projection,
         featureProjection: mapVars.projection
     });
     var confRate = dumpSite.confidenceRate * 100;
     vectorSource.addFeature(feature);
     if(dumpSite.isInsideIgnoreZone === true){
         feature.setStyle(new ol.style.Style({
         fill: new ol.style.Fill({
             color: 'rgba(82, 78, 183, 0.5)'
         }),
         stroke: new ol.style.Stroke({
             color: 'blue',
             width: 2 
         }),
         text: new ol.style.Text({
             font: '14px Calibri,sans-serif',
             fill: new ol.style.Fill({ color: '#000' }),
             stroke: new ol.style.Stroke({
                 color: '#fff', width: 2
             }),
             text: "Confidence rate:\n" + confRate.toFixed(0).toString() + "%"
         })
     }));
     }else{
         feature.setStyle(new ol.style.Style({
         fill: new ol.style.Fill({
             color: 'rgba(255, 0, 0, 0.5)'
         }),
         stroke: new ol.style.Stroke({
             color: 'red',
             width: 2 
         }),
         text: new ol.style.Text({
             font: '14px Calibri,sans-serif',
             fill: new ol.style.Fill({ color: '#000' }),
             stroke: new ol.style.Stroke({
                 color: '#fff', width: 2
             }),
             text: "Confidence rate:\n" + confRate.toFixed(0).toString() + "%"
         })
     }));
     }
 });

 var vectorLayer = new ol.layer.Vector({
     source: vectorSource,
     title: detectionRun.name,
     order: index + 1,
     visible: true                               
 });

 historicDataLayerGroup.getLayers().push(vectorLayer);
 ol.extent.extend(extent, vectorSource.getExtent());
```
5. Area comparison report: Gets the selected detection runs, choose confidance rate and provide numeric data for area in/out of detection ignore zone and total area
```javascript
  $.ajax({
      type: "POST",
      url: "@Url.Action("GenerateAreaComparisonAvgConfidenceRateReport", "Detection", new { Area = "IntranetPortal" })",
      data: {
          selectedDetectionRunsIds: selectedDetectionRunsIds,
          selectedConfidenceRate: selectedConfidenceRate
      },
  })

row.groupedDumpSitesList.forEach(info => {
    tableHtml += `<th>${info.className} area in ignore zone (m²)</th> <th>${info.className} area out of ignore zone (m²)</th>`;
});
tableHtml += `<th>Total Area (m²)</th>`;
tableHtml += `
            </tr>
        </thead>
        <tbody>
            <tr>
`;
row.groupedDumpSitesList.forEach(info => {
    let totalGroupAreaInIgnoreZone = info.totalGroupAreaInIgnoreZone != null ? info.totalGroupAreaInIgnoreZone.toFixed(2) : "No dump sites above the selected confidence rate";
    let totalGroupAreaOutOfIgnoreZone = info.totalGroupAreaOutOfIgnoreZone != null ? info.totalGroupAreaOutOfIgnoreZone.toFixed(2) : "No dump sites above the selected confidence rate";

    tableHtml += `<td>${totalGroupAreaInIgnoreZone}</td> <td>${totalGroupAreaOutOfIgnoreZone}</td>`;
});

tableHtml += `<td>${row.totalAreaOfDetectionRun != null ? row.totalAreaOfDetectionRun.toFixed(2) : 'No dump sites above the selected confidence rate'}</td>`;
tableHtml += `
            </tr>
        </tbody>
    </table>
```
### Detection ignore zones tool
- This tool interacts with detection ignore zones on the map.
- Activate the tool, view a list of available detection ignore zones, focus on specific zone, add new zone, edit zone and delete zone options are available.
##### Implementation (Script Details) 
1. Tool Initialized as a new instance of MapToolBaseClass, also there is a button in the sidebar for user interaction
```javascript
 var ignoreZones = new MapToolBaseClass("IgnoreZones");
$("#map-sidebar-upper-tabs-list").append(tabHtml);
```
2. Tool activation and deactivation: set-up and reset the UI state 
```javascript
 function activateIgnoreZonesTool() {
     mapVars.deactivateTools();
     $("#map-sidebar-ignorezones-pane").addClass("active");
     this.isActive = true;
 }

 function deactivateIgnoreZonesTool() {
     cleanupDrawInteraction();
     if (currentFocusedPolygon) {
         let zoneName = currentFocusedPolygon.get('zoneName');   
         let zoneIsEnabled = currentFocusedPolygon.get('isEnabled');
         if (originalGeometry) {
             currentFocusedPolygon.setGeometry(originalGeometry.clone());
         }               
         currentFocusedPolygon.setStyle(defaultStyle(zoneName, zoneIsEnabled));
         currentFocusedPolygon = null;
         originalGeometry = null;
     }

     cleanUpEditZoneMode();
     $('.modal').modal('hide');
     Swal.close();

     this.isActive = false;
     $("#map-sidebar-ignorezones-pane").removeClass("active");

     if (!$("#editNotify").hasClass("d-none")) {
         $("#editNotify").addClass("d-none");
     }

     if (!$("#addNotify").hasClass("d-none")) {
         $("#addNotify").addClass("d-none");
     }

     resetAllFeaturesToDefaultStyle();
     return true;
 }
```
3. Populates the table with detection ingnore zones data using an AJAX GET request to the backend
```javascript 
 function generateDetectionIgnoreZonesTable() {
     var originalZIndex = $('#map-sidebar').css('z-index');
     $.ajax({
         type: "GET",
         url: "@Url.Action("GetAlllIgnoreZones", "DetectionIgnoreZones", new { Area = "IntranetPortal" })",
         ...
     })
 }
 function showTableOfAllIgnoreZones(detectionIgnoreZonesTable, ignoreZonesList) {
     detectionIgnoreZonesTable +=
         `<table class='table table-striped table-bordered table-hover mt-4' id='detectionIgnoreZonesTable'>
                                     <thead>
                                         <tr>
                                             <th>@DbResHtml.T("Name", "Resources")</th>
                                             <th>@DbResHtml.T("Description", "Resources")</th>
                                             <th>@DbResHtml.T("Is Enabled", "Resources")</th>
                                             <th></th>
                                         </tr>
                                     </thead>
                                     <tbody>
                              `;

     detectionIgnoreZonesTable = drawTableRowsForEachIgnoreZone(ignoreZonesList, detectionIgnoreZonesTable);
     detectionIgnoreZonesTable += `</tbody></table>`;
     $("#map-sidebar-ignorezonestable-tab-container").html(detectionIgnoreZonesTable);
 }
```
4. Focus on detection ignore zone means selecting the zone, adjust the extent to fit the polygon and change the polygon style
```csharp
 [HttpPost]
 [HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
 public async Task<ResultDTO<DetectionIgnoreZoneDTO?>> GetIgnoreZoneById(Guid id)
 {           
     if (id == Guid.Empty)
     {
         return ResultDTO<DetectionIgnoreZoneDTO?>.Fail("Invalid zone id");
     }
    
    
     ResultDTO<DetectionIgnoreZoneDTO?> resultGetEntity = await _detectionIgnoreZoneService.GetIgnoreZoneById(id);
     if (resultGetEntity.IsSuccess == false && resultGetEntity.HandleError())
     {
         return ResultDTO<DetectionIgnoreZoneDTO?>.Fail(resultGetEntity.ErrMsg!);
     }

     return ResultDTO<DetectionIgnoreZoneDTO?>.Ok(resultGetEntity.Data);
 }
```
```javascript
function proceedFocusSelectedZone(ignoreZone) {
    
    if (ignoreZone && ignoreZone.geoJson) {
        var ignoreZonesLayer = mapVars.map.getLayers().getArray().find(layer =>
            layer instanceof ol.layer.Vector && layer.get('title') === 'Detection Ignore Zones'
        );

        if (ignoreZonesLayer) {
            var features = ignoreZonesLayer.getSource().getFeatures();

            features.forEach(function (feature) {
                feature.setStyle(defaultStyle(feature.get('zoneName'), feature.get('isEnabled')));
            });

            var selectedFeature = features.find(function (feature) {
                return feature.get('id') === ignoreZone.id;
            });

            if (selectedFeature) {
                selectedFeature.setStyle(createFocusedStyle(ignoreZone.name));

                var extent = selectedFeature.getGeometry().getExtent();
                mapVars.map.getView().fit(extent, {
                    padding: [150, 150, 150, 150],
                    duration: 1000
                });

                currentFocusedPolygon = selectedFeature;
            } 
        }  
    }      
}
```
5. Add new zone includes more separate functions:
- Trigger when the button is clicked start adding new ignore zone, after that check if a zone is already being drawn, if not, starts drawing a new zone, otherwise prompts the user to finish the current drawing first
```javascript
 function addNewZone() {
     var originalZIndex = $('#map-sidebar').css('z-index');
     if (!draw) {
         drawZonePolygonOnMap();
     } 
     ...
}
```
- Initialize the drawing interaction for a polygon on the map, add the drawing interaction and handles the completion of the drawing, which triggers a modal to input zone details
```javascript
function drawZonePolygonOnMap() {
    cleanupDrawInteraction();
    
    $("#addNotify").removeClass("d-none").fadeIn(300);
    draw = new ol.interaction.Draw({
        source: source,
        type: 'Polygon',
        style: createFocusedStyle(),
        projection: mapVars.projection              
    });

    mapVars.map.addInteraction(draw);
    $("#cancelAddingZoneBtn").off('click').on('click', function () {
        cancelAddingZone();
    });
           
    draw.on('drawend', function (event) {     
        var geoJsonFormat = new ol.format.GeoJSON();
        var geoJson = geoJsonFormat.writeFeature(event.feature);
        cleanupDrawInteraction();
        $("#addNotify").fadeOut(300, function () {
            $(this).addClass("d-none");
        });
        openAddPolygonModal(geoJson);
    });
}
```
- Display a modal for the user to enter details about the newly drawn polygon, including name, description, and enabled status
- Cancel the zone creation process, hides the notification, and removes the drawing interaction from the map
- Clear the drawn polygon from the map and removes the drawing interaction
- Reset the input fields in the modal
```javascript
    function cleanupDrawInteraction() {
        if (draw) {
            source.clear();
            mapVars.map.removeInteraction(draw);
            draw = null;
        }
    }

    function resetAddModalInputValues() {
         $("#zoneName").val('');
         $("#zoneDescription").val('');
         $("#isZoneEnabled").prop("checked", false);
         $("#polygonCoordinates").val('');
    }
```
- Validate form and sends an AJAX request to save the ignore zone, and shows a success or error message. Upon success, updates the ignore zone list on the map
```javascript
function savePolygon() {       
    ...
    $.ajax({
        type: "POST",
        url: "@Url.Action("AddIgnoreZone", "DetectionIgnoreZones", new { Area = "IntranetPortal" })", 
        ...
    });
}
```
```csharp
[HttpPost]
[HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
public async Task<ResultDTO> AddIgnoreZone([FromBody] DetectionIgnoreZoneDTO dto)
{
    if (!ModelState.IsValid)
    {
        var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        return ResultDTO.Fail(error);
    }
    if (string.IsNullOrEmpty(dto.EnteredZonePolygon))
    {
        return ResultDTO.Fail("Invalid entered polygon");
    }
    var userId = User.FindFirstValue("UserId");
    if (string.IsNullOrEmpty(userId))
    {
        return ResultDTO.Fail("User not found");
    }

    dto.CreatedById = userId;
    dto.CreatedOn = DateTime.UtcNow;
    dto.Geom = (Polygon)DTOs.Helpers.GeoJsonHelpers.GeoJsonFeatureToGeometry(dto.EnteredZonePolygon);
    ResultDTO resultCreate = await _detectionIgnoreZoneService.CreateDetectionIgnoreZoneFromDTO(dto);
    if (resultCreate.IsSuccess == false && resultCreate.HandleError())
    {
        return ResultDTO.Fail(resultCreate.ErrMsg!);
    }

    return ResultDTO.Ok();
}
```
6. Edit zone includes more separate functions:
- Prompt a confirmation dialog to edit the selected ignore zone. If confirmed, it allows to modify the geometry of the selected zone, including visual focus and style.
- Search through the Detection Ignore Zones layer and returns the corresponding zone feature.
```javascript
function getZoneById(zoneId) {
    var ignoreZonesLayer = mapVars.map.getLayers().getArray().find(layer =>
        layer instanceof ol.layer.Vector && layer.get('title') === 'Detection Ignore Zones'
    );       

    var features = ignoreZonesLayer.getSource().getFeatures();
    var selectedZone = features.find(function (feature) {
        return feature.get('id') === zoneId;
    });
    return selectedZone;
}
```
- Focus the map view on the extent of the selected zone by adjusting the map view to fit the zone's geometry. If the extent is invalid, it shows an error message.
```javascript
 function focusOnSelectedZone(zone) {
     var extent = zone.getGeometry().getExtent();
     var padding = [50, 50, 50, 50];
     mapVars.map.getView().fit(extent, {
         padding: padding,
         duration: 1000
     });
 }

```
- Enable the edit mode for the specified zone. Thezone's geometry on the map can be modified. It adds the modify interaction to the map and sets up event handlers for finishing or canceling the edit.
```javascript
function enableEditZoneMode(zone, zoneId) {
    cleanUpEditZoneMode();
    originalGeometry = zone.getGeometry().clone();

    if (currentFocusedPolygon === null) {
        currentFocusedPolygon = zone;
        let zoneName = zone.get('zoneName');
        zone.setStyle(createFocusedStyle(zoneName));
    }

    if (currentFocusedPolygon !== zone) {
        let zoneName = currentFocusedPolygon.get('zoneName');
        let zoneIsEnabled = currentFocusedPolygon.get('isEnabled');
        currentFocusedPolygon.setStyle(defaultStyle(zoneName, zoneIsEnabled));
        currentFocusedPolygon = zone;
    }

    modifyInteraction = new ol.interaction.Modify({
        features: new ol.Collection([zone])
    });
    mapVars.map.addInteraction(modifyInteraction);

    let zoneName = zone.get('zoneName');
    zone.setStyle(createFocusedStyle(zoneName));

    $("#editNotify").removeClass("d-none").fadeIn(300);
    $("#finishEditingButton").off('click').on('click', function () {
        handleFinishEditingButtonClick(zone, zoneId);
    });
    $("#cancelEditingButton").off('click').on('click', function () {
        handleCancelEditingButtonClick(zone);
    });
}
```
- Cancel the current editing session and restores the zone to its original geometry. It also resets the zone's style and removes the edit mode interface.
- Finalize the editing of a zone and sends the updated geometry in GeoJSON format for further processing, usually by opening a modal to save or submit the changes.
- Open a modal to allow modifing the attributes of the selected ignore zone.
- Close the edit zone modal and resets the map state, ensuring that any changes to the current zone’s geometry are discarded.
- Saves the updated ignore zone data, including its geometry and attributes, to the server via an AJAX POST request. Validates that all required fields are filled before saving the data.
```javascript
 function saveEditZone() {
     ...
     $.ajax({
         type: "POST",
         url: "@Url.Action("UpdateIgnoreZone", "DetectionIgnoreZones", new { Area = "IntranetPortal" })",
         data: JSON.stringify({
             Id: id,
             Name: name,
             Description: description,
             EnteredZonePolygon: formattedGeoJson,
             IsEnabled: isEnabled
         }),
         ...
     });
 } 
```
```csharp
 [HttpPost]
 [HasAuthClaim(nameof(SD.AuthClaims.ManageDetectionIgnoreZones))]
 public async Task<ResultDTO> UpdateIgnoreZone([FromBody] DetectionIgnoreZoneDTO dto)
 {
     if (!ModelState.IsValid)
     {
         var error = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
         return ResultDTO.Fail(error);
     }
     if (!string.IsNullOrEmpty(dto.EnteredZonePolygon))
     {
         dto.Geom = (Polygon)DTOs.Helpers.GeoJsonHelpers.GeoJsonFeatureToGeometry(dto.EnteredZonePolygon);
     }

     ResultDTO resultUpdate = await _detectionIgnoreZoneService.UpdateDetectionIgnoreZoneFromDTO(dto);
     if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())
     {
         return ResultDTO.Fail(resultUpdate.ErrMsg!);
     }

     return ResultDTO.Ok();
 }
```