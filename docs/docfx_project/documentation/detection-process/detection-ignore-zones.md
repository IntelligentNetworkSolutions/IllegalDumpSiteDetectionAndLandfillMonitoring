# Detection Ignore Zones - Developer Documentation

## Overview  

- The Detection Ignore Zones component manages spatial areas that should be excluded from dump site detection analysis. 
- Handling functionality for creating, editing, and managing these exclusion zones.

## Architecture

### Entity Structure

```csharp
public class DetectionIgnoreZone : BaseEntity<Guid>, ICreatedByUser
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }

    [JsonIgnore]
    [Column(TypeName = "geometry(Polygon)")]
    public Polygon Geom { get; set; }

    [NotMapped]
    public string GeoJson { get; set; }

    public string CreatedById { get; set; }
    public DateTime CreatedOn { get; set; }
}
```

### Service Layer Interface

```csharp
public interface IDetectionIgnoreZoneService
{
    Task<ResultDTO<List<DetectionIgnoreZoneDTO>>> GetAllIgnoreZonesDTOs();
    Task<ResultDTO<DetectionIgnoreZoneDTO?>> GetIgnoreZoneById(Guid id);
    Task<ResultDTO> CreateDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto);
    Task<ResultDTO> UpdateDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto);
    Task<ResultDTO> DeleteDetectionIgnoreZoneFromDTO(DetectionIgnoreZoneDTO dto);
}
```

## Implementation Details

## Spatial Operations  

### GeoJSON Processing  

```csharp
public static class GeoJsonHelpers
{
    public static string GeometryToGeoJson(Geometry geom)
    {
        var serializer = GeoJsonSerializer.Create();
        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Serialize(jsonWriter, geom);
        return stringWriter.ToString();
    }

    public static Geometry GeoJsonFeatureToGeometry(string geoJson)
    {
        GeoJsonReader reader = new GeoJsonReader();
        Feature feature = reader.Read<Feature>(geoJson);
        return feature.Geometry;
    }
}
```

### Spatial Validation  

```csharp
public static Polygon ConvertBoundingBoxToPolygon(List<float> bbox)
{
    if (bbox == null || bbox.Count != 4)
        throw new ArgumentException("Invalid bbox format");

    GeometryFactory factory = new GeometryFactory();
    Coordinate[] coordinates = new Coordinate[]
    {
        new(bbox[0], bbox[1]),
        new(bbox[0] + bbox[2], bbox[1]),
        new(bbox[0] + bbox[2], bbox[1] + bbox[3]),
        new(bbox[0], bbox[1] + bbox[3]),
        new(bbox[0], bbox[1])
    };
    
    return factory.CreatePolygon(coordinates);
}
```

## Frontend Integration  

### Map Interaction  

```javascript
function enableEditZoneMode(zone, zoneId) {
    modifyInteraction = new ol.interaction.Modify({
        features: new ol.Collection([zone])
    });
    mapVars.map.addInteraction(modifyInteraction);
}

function drawZonePolygonOnMap() {
    draw = new ol.interaction.Draw({
        source: source,
        type: 'Polygon',
        style: createFocusedStyle(),
        projection: mapVars.projection
    });
    mapVars.map.addInteraction(draw);
}
```

### Style Configuration  

```javascript
function defaultStyle(text, isEnabled) {
    return new ol.style.Style({
        fill: new ol.style.Fill({
            color: isEnabled ? 'rgba(152, 133, 88, 0.4)' : 'rgba(255, 204, 51, 0.2)'
        }),
        stroke: new ol.style.Stroke({
            color: isEnabled ? '#988558' : '#ffcc33',
            width: 2
        })
    });
}
```

## Database Integration  

### PostGIS Configuration  

```sql
-- Enable PostGIS extension
CREATE EXTENSION IF NOT EXISTS postgis;

-- Index for spatial queries
CREATE INDEX idx_detectionignorezones_geom 
ON public.detectionignorezones USING gist (geom);
```

## Files Involved

```mermaid
Controllers/
└── DetectionIgnoreZonesController.cs
    - Manages ignore zone operations
    - Handles zone creation and updates
    - Processes spatial data

Views/
└── Shared/
    └── Components/
        └── DetectionIgnoreZonesToolViewComponent/
            └── Default.cshtml
            - Implements zone management UI
            - Handles map interactions
            - Manages zone visualization

Services/
└── DetectionIgnoreZoneService.cs
    - Processes zone operations
    - Manages spatial data
    - Handles zone validation

Entities/
└── DetectionIgnoreZone.cs
    - Defines ignore zone structure
    - Contains spatial data properties

Helpers/
└── GeoJsonHelpers.cs
    - Provides spatial data conversion
    - Handles GeoJSON processing
```
