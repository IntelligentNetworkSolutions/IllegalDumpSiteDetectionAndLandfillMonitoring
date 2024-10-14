using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.ObjectDetection.API.Responses.DetectionRun;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SD;

namespace DTOs.Helpers
{
    public static class NewtonsoftJsonHelper
    {
        public static bool IsValidJson(string jsonString)
        {
            try
            {
                JToken.Parse(jsonString);
                // If it gets here, the JSON is well-formed
                return true;
            }
            catch (JsonReaderException)
            {
                // JSON was not in a valid format
                return false;
            }
        }

        public static async Task<ResultDTO<DetectionRunFinishedResponse>> ReadFromFileAsDeserializedJson<TJson>(string filePath) where TJson : class
        {
            //try
            //{
            //    string jsonContent = await File.ReadAllTextAsync(filePath);

            //    if (NewtonsoftJsonHelper.IsValidJson(jsonContent) == false)
            //        return ResultDTO<TJson>.Fail("JSON is not valid");

            //    TJson? response = JsonConvert.DeserializeObject<TJson>(jsonContent);
            //    if (response == null)
            //        return ResultDTO<TJson>.Fail("Failed Deserialization");

            //    return ResultDTO<TJson>.Ok(response);
            //}
            //catch (Exception ex)
            //{
            //    return ResultDTO<TJson>.ExceptionFail(ex.Message, ex);
            //}

            try
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                if (NewtonsoftJsonHelper.IsValidJson(jsonContent) == false)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("JSON is not valid");


                var jsonObject = JObject.Parse(jsonContent);
                if(jsonObject == null)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("Parsing failed");

                var detectionsArray = jsonObject["detections"] as JArray;
                if (detectionsArray == null)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("Detections array not found");

                var labels = new List<int>();
                var scores = new List<float>();
                var bboxes = new List<double[]>();

                foreach (var detection in detectionsArray)
                {
                    int label = (int)detection["label"]!;
                    float score = (float)detection["score"]!;
                    var bbox = detection["bbox"]!.ToObject<double[]>();

                    labels.Add(label);
                    scores.Add(score);
                    bboxes.Add(bbox!);
                }

                var response = new DetectionRunFinishedResponse
                {
                    labels = labels.ToArray(),
                    scores = scores.ToArray(),
                    bboxes = bboxes.ToArray()
                };

                if (response == null)
                    return ResultDTO<DetectionRunFinishedResponse>.Fail("Failed Deserialization");

                return ResultDTO<DetectionRunFinishedResponse>.Ok(response);
            }
            catch (Exception ex)
            {
                return ResultDTO<DetectionRunFinishedResponse>.ExceptionFail(ex.Message, ex);
            }
        }
    }
}
