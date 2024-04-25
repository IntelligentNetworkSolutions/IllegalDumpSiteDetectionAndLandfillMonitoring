using NetTopologySuite.Features;
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

        /// <summary>
        /// Returns BBox coordinates in (top left, bottom right) format
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public static Dictionary<int, int>? GeometryBBoxToTopLeftBottomRight(Geometry geom)
        {
            if (geom is null)
            {
                return null;
            }

            var envelope = geom.EnvelopeInternal;

            int top = Convert.ToInt32(envelope.MinX);
            int left = Convert.ToInt32(envelope.MinY);
            int bottom = Convert.ToInt32(envelope.MaxX);
            int right = Convert.ToInt32(envelope.MaxY);

            return new Dictionary<int, int> {{ top, left }, { bottom, right }};
        }

        /// <summary>
        /// Returns BBox coordinates (top left, width, height) format
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public static Dictionary<int, int>? GeometryBBoxToTopLeftWidthHeight(Geometry geom)
        {
            if (geom is null)
            {
                return null;
            }

            var envelope = geom.EnvelopeInternal;

            int top = Convert.ToInt32(envelope.MinX);
            int left = Convert.ToInt32(envelope.MaxY);            
            int width = Convert.ToInt32(envelope.Width);
            int height = Convert.ToInt32(envelope.Height);

            return new Dictionary<int, int> { { top, left }, { width, height } };
        }
    }
}
