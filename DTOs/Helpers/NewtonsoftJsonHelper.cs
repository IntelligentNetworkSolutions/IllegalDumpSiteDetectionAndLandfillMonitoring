using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static async Task<ResultDTO<TJson>> ReadFromFileAsDeserializedJson<TJson>(string filePath) where TJson : class
        {
            try
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);

                if (NewtonsoftJsonHelper.IsValidJson(jsonContent) == false)
                    return ResultDTO<TJson>.Fail("JSON is not valid");

                TJson? response = JsonConvert.DeserializeObject<TJson>(jsonContent);
                if (response == null)
                    return ResultDTO<TJson>.Fail("Failed Deserialization");

                return ResultDTO<TJson>.Ok(response);
            }
            catch (Exception ex)
            {
                return ResultDTO<TJson>.ExceptionFail(ex.Message, ex);
            }
        }
    }
}
