using System.Text.Json;
using DocumentFormat.OpenXml.Wordprocessing;
using DTOs.MainApp.BL.DetectionDTOs;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using NetTopologySuite.Geometries;
using SD;

namespace MainApp.MVC.Helpers
{
    public static class JsonFileReader
    {
        public static bool IsValidJson(string jsonString)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(jsonString))
                {
                    // If it gets here, the JSON is well-formed
                    return true;
                }
            }
            catch (JsonException)
            {
                // JSON was not in a valid format
                return false;
            }
        }

        public static async Task<ResultDTO<TJson>> ReadFromStringAsDeserializedJson<TJson>(string jsonContent) where TJson : class
        {
            try
            {
                if (string.IsNullOrEmpty(jsonContent))
                    return ResultDTO<TJson>.Fail("JSON is not present or empty");

                if(IsValidJson(jsonContent) == false)
                    return ResultDTO<TJson>.Fail("JSON is not valid");

                TJson? response = JsonSerializer.Deserialize<TJson>(jsonContent);
                if (response is null)
                    return ResultDTO<TJson>.Fail("Failed Deserialization");

                return await Task.FromResult(ResultDTO<TJson>.Ok(response));
            }
            catch (Exception ex)
            {
                return ResultDTO<TJson>.ExceptionFail(ex.Message, ex);
            }
        }

        public static async Task<ResultDTO<TJson>> ReadFromFileAsDeserializedJson<TJson>(string filePath) where TJson : class
        {
            try
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);

                if (IsValidJson(jsonContent) == false)
                    return ResultDTO<TJson>.Fail("JSON is not valid");

                TJson? response = JsonSerializer.Deserialize<TJson>(jsonContent);
                if (response is null)
                    return ResultDTO<TJson>.Fail("Failed Deserialization");

                return ResultDTO<TJson>.Ok(response);
            }
            catch (Exception ex)
            {
                return ResultDTO<TJson>.ExceptionFail(ex.Message, ex);
            }
        }

        public static async Task<ResultDTO> ReceiveDeserializedDetectionRunResponseJson()
        {
            //string jsonFilePath = "";
            //ResultDTO<DetectionRunFinishedResponse> resultReadJson =
            //    await JsonFileReader.ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(jsonFilePath);

            // Example JSON
            string testJson = "{ \"labels\": [1, 2], \"scores\": [0.93, 0.88], \"bboxes\": [ [120.0, 30.0, 320.0, 180.0], [170.0, 45.0, 245.0, 125.0] ] }";
            string testJsonFilePath = "C:\\Users\\INS\\OneDrive\\Desktop\\Vardariste_1.json";
            //var resultJsonDeserialization =
            //var resultJsonDeserializatidon =
            //    await JsonFileReader.ReadFromStringAsDeserializedJson<DetectionRunFinishedResponse>(testJson);

            var resultJsonDeserialization =
                await JsonFileReader.ReadFromFileAsDeserializedJson<DetectionRunFinishedResponse>(testJsonFilePath);

            if (resultJsonDeserialization.IsSuccess == false)
                return ResultDTO.Fail(resultJsonDeserialization.ErrMsg!);

            DetectionRunFinishedResponse detections = resultJsonDeserialization.Data!;

            string detectionRunDatasetIdStr = Guid.NewGuid().ToString();
            Guid detectionRunDatasetId = Guid.Parse(detectionRunDatasetIdStr);

            string detectionRunIdStr = Guid.NewGuid().ToString();
            Guid detectionRunId = Guid.Parse(detectionRunIdStr);
            
            string detectionRunName = "Dummy Test Detection Run";
            
            string detectedZonesDatasetClassIdStr = Guid.NewGuid().ToString();
            Guid detectedZonesDatasetClassId = Guid.Parse(detectedZonesDatasetClassIdStr);

            List<DetectedDumpSiteDTO> detectedDumpSites = new List<DetectedDumpSiteDTO>();

            GeometryFactory factory = new GeometryFactory();
            List<Polygon> polygons = new List<Polygon>();
            for (int i = 0; i < detections.bboxes.Length; i++)
            {
                var box = detections.bboxes[i];
                var lowerLeft = new Coordinate(box[0], box[1]);
                var upperRight = new Coordinate(box[2], box[3]);

                // Create a rectangle polygon from the bounding box
                Coordinate[] coordinates = new Coordinate[] { 
                    lowerLeft, new Coordinate(upperRight.X, lowerLeft.Y), upperRight, new Coordinate(lowerLeft.X, upperRight.Y),
                    lowerLeft  // Closed linear ring 
                };

                //polygons.Add(factory.CreatePolygon(coordinates));
                detectedDumpSites.Add(new DetectedDumpSiteDTO()
                {
                    DetectionRunId = detectionRunId,
                    DatasetClassId = detectedZonesDatasetClassId,
                    ConfidenceRate = detections.scores[i],
                    Geom = factory.CreatePolygon(coordinates)
                });
            }

            return ResultDTO.Ok();
        }
    }
}
