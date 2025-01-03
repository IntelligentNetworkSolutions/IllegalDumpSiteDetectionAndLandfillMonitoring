﻿using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helpers
{
    public class GeoJsonHelpers
    {
        public static string GeometryToGeoJson(Geometry geom)
        {
            if(geom == null) 
                throw new ArgumentNullException(nameof(geom));

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
    }
}
