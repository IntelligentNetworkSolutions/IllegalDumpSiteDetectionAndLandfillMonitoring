using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace DTOs.Helpers
{
    public class GeoJsonHelpers
    {
        public static string GeometryToGeoJson(Geometry geom)
        {
            if (geom == null)
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

        public static Geometry GeoJsonFeatureToGeometry(string geoJson)
        {
            GeoJsonReader reader = new GeoJsonReader();
            Feature feature = reader.Read<Feature>(geoJson);
            Geometry geometry = feature.Geometry;
            return geometry;
        }

        public static Polygon ConvertBoundingBoxToPolygon(List<float> bbox)
        {
            // Validate that the bounding box list has exactly 4 elements
            if (bbox == null || bbox.Count != 4)
                throw new ArgumentException("Bounding box must contain exactly 4 elements: [x, y, width, height].");

            // Extract bbox values
            float x = bbox[0];
            float y = bbox[1];
            float width = bbox[2];
            float height = bbox[3];

            // Define the four corners of the bounding box
            List<Coordinate> coordinates = new List<Coordinate>
            {
                new Coordinate(x, y),             // Top-left corner (x, y)
                new Coordinate(x + width, y),     // Top-right corner (x + width, y)
                new Coordinate(x + width, y + height), // Bottom-right corner (x + width, y + height)
                new Coordinate(x, y + height),    // Bottom-left corner (x, y + height)
                new Coordinate(x, y)              // Closing the polygon (back to top-left corner)
            };

            // Create and return the polygon using the coordinates
            GeometryFactory geometryFactory = new GeometryFactory();
            Polygon polygon = geometryFactory.CreatePolygon(coordinates.ToArray());

            return polygon;
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

            return new Dictionary<int, int> { { top, left }, { bottom, right } };
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

        /// <summary>
        /// Returns BBox coordinates (top left, width, height) format
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public static List<float>? GeometryBBoxToTopLeftWidthHeightList(Geometry geom)
        {
            if (geom == null)
            {
                return null;
            }

            var envelope = geom.EnvelopeInternal;

            float left = (float)envelope.MinX;
            float top = (float)envelope.MinY;
            float width = (float)envelope.Width;
            float height = (float)envelope.Height;

            return new List<float> { left, top, width, height };
        }
    }
}