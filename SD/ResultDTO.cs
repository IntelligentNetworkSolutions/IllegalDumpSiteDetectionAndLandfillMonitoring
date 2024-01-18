namespace SD
{
    public record ResultDTO(bool IsSuccess, string? ErrMsg, object? ExObj)
    {
        public static ResultDTO Ok() => new ResultDTO(true, null, null);

        public static ResultDTO Fail(string errorMessage) => new ResultDTO(false, errorMessage, null);

        public static ResultDTO ExceptionFail(string errorMessage, object? exceptionObj) => new ResultDTO(false, errorMessage, exceptionObj);

        public static bool HandleError(ResultDTO resultDTO)
        {
            if (resultDTO.ExObj is not null)
                throw (Exception)resultDTO.ExObj;

            return true;
        }
    }

    public record ResultDTO<DataType>(bool IsSuccess, DataType? Data, string? ErrMsg)
    {
        public static ResultDTO<DataType> Ok(DataType Data) => new ResultDTO<DataType>(true, Data, null);

        public static ResultDTO<DataType> Fail(string errorMessage) => new ResultDTO<DataType>(false, default(DataType?), errorMessage);
    }
}
