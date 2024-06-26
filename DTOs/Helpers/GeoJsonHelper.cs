﻿using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Helpers
{
    public class GeoJsonHelper
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


        public static List<string> GeometryListToGeoJson(List<Polygon> geom)
        {
            List<string> geoJsons = new();

            foreach (var item in geom)
            {
                string geoJson;
                var serializer = GeoJsonSerializer.Create();
                using (var stringWriter = new StringWriter())
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    serializer.Serialize(jsonWriter, item);
                    geoJson = stringWriter.ToString();
                }
                geoJsons.Add(geoJson);
            }
           
            return geoJsons;
        }
    }
}
