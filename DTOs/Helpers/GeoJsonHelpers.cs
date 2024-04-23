﻿using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Converters;
using Newtonsoft.Json;
using System.IO;

namespace DTOs.Helpers
{
    public class GeoJsonHelpers
    {
        public static string GeometryToGeoJson(Geometry geom)
        {
            string geoJson;

            var serializer = GeoJsonSerializer.Create();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                serializer.Serialize(jsonWriter, geom);
                geoJson = stringWriter.ToString();
            }
            return geoJson;
        }

        public static Geometry GeoJsonFeatureToGeometry(string geoJson)
        {
            GeoJsonReader reader = new GeoJsonReader();            
            Feature feature = reader.Read<Feature>(geoJson);
            Geometry geometry = feature.Geometry;
            return geometry;
        }
    }
}
