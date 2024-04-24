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

    public record ResultDataDTO(bool IsSuccess, object? Data, string? ErrMsg, object? ExObj)
    {
        public static ResultDataDTO Ok() => new ResultDataDTO(true, null, null, null);
        public static ResultDataDTO Ok<DataType>(DataType? Data) => new ResultDataDTO(true, Data, null, null);
        public static ResultDataDTO Fail(string errorMsg) => new ResultDataDTO(false, null, errorMsg, null);
        public static ResultDataDTO ExceptionFail(string errorMsg, object? exceptionObj) => new ResultDataDTO(false, null, errorMsg, exceptionObj);

        public bool IsOk() => this.IsSuccess;
        public bool IsFailNoExceptionHandling() => this.IsSuccess == false;
        public bool IsFailThrowsExceptionFail()
        {
            if (this.ExObj is not null)
                throw (Exception)this.ExObj;

            if(this.IsSuccess == false)
                return true;

            return false;
        }

        public static ResultDataDTO? ResultDataDTOFromResultDTO(ResultDTO resultDTO)
        {
            if (resultDTO is null)
                return null;
            
            return new ResultDataDTO(resultDTO.IsSuccess, null, resultDTO.ErrMsg, resultDTO.ExObj);
        }

        public static ResultDataDTO? ResultDataDTOFromResultDTO<DataType>(ResultDTO<DataType> resultDTO)
        {
            if (resultDTO is null)
                return null;

            return new ResultDataDTO(resultDTO.IsSuccess, resultDTO.Data, resultDTO.ErrMsg, resultDTO.ExObj);
        }
    }

    public record ResultDTO<DataType>(bool IsSuccess, DataType? Data, string? ErrMsg, object? ExObj)
    {
        public static ResultDTO<DataType> Ok(DataType Data) => new ResultDTO<DataType>(true, Data, null, null);

        public static ResultDTO<DataType> Fail(string errorMessage) => new ResultDTO<DataType>(false, default(DataType?), errorMessage, null);

        public static ResultDTO<DataType> ExceptionFail(string errorMessage, object? exceptionObj) => new ResultDTO<DataType>(false, default(DataType?), errorMessage, exceptionObj);

        public static bool HandleError(ResultDTO<DataType> resultDTO)
        {
            if (resultDTO.ExObj is not null)
                throw (Exception)resultDTO.ExObj;

            return true;
        }
    }

    public static class ResultDTOExtensions
    {
        public static bool HandleError(this ResultDTO resultDTO)
        {
            if (resultDTO.ExObj is not null)
                throw (Exception)resultDTO.ExObj;

            return true;
        }

        public static ResultDTO<DataType> Ok<DataType>(this DataType data)
        {
            return new ResultDTO<DataType>(true, data, null, null);
        }

        public static ResultDTO<DataType> Fail<DataType>(this string errorMessage)
        {
            return new ResultDTO<DataType>(false, default(DataType), errorMessage, null);
        }

        public static ResultDTO<DataType> ExceptionFail<DataType>(this string errorMessage, object? exceptionObj)
        {
            return new ResultDTO<DataType>(false, default(DataType), errorMessage, exceptionObj);
        }

        public static bool HandleError<DataType>(this ResultDTO<DataType> resultDTO)
        {
            if (resultDTO.ExObj is not null)
                throw (Exception)resultDTO.ExObj;

            return true;
        }
    }
}
