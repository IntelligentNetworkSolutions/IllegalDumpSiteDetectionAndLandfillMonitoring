namespace DTOs
{
    // TODO: Decide if ErrorMessage or Exception Object
    public record ResultDTO(bool IsSuccess, string? ErrMsg)
    {
        public static ResultDTO Ok() => new ResultDTO(true, null);

        public static ResultDTO Fail(string errorMessage) => new ResultDTO(false, errorMessage);
    }

    public record ResultDTO<DataType>(bool IsSuccess, DataType? data, string? ErrMsg) where DataType : class
    {
        public static ResultDTO<DataType> Ok(DataType data) => new ResultDTO<DataType>(true, data, null);

        public static ResultDTO<DataType> Fail(string errorMessage) => new ResultDTO<DataType>(false, null, errorMessage);
    }
}
