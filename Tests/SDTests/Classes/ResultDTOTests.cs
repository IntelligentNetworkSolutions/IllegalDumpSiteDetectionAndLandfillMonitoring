using SD;

namespace Tests.SDTests.Classes
{
    public class ResultDTO_Tests
    {
        #region ResultDTO
        [Fact]
        public void ResultDTO_Ok_ReturnsTrue()
        {
            var resultDTO = ResultDTO.Ok();
            Assert.True(resultDTO.IsSuccess);
            Assert.Null(resultDTO.ErrMsg);
            Assert.Null(resultDTO.ExObj);
        }

        [Fact]
        public void ResultDTO_Fail_ReturnsFalseAndNullExceptionObj()
        {
            var resultDTO = ResultDTO.Fail("error message");
            Assert.False(resultDTO.IsSuccess);
            Assert.Equal("error message", resultDTO.ErrMsg);
            Assert.Null(resultDTO.ExObj);
        }

        [Fact]
        public void ResultDTO_ExceptionFail_ReturnsFalseAndNotNullExceptionObj()
        {
            var exception = new Exception("exception message");
            var resultDTO = ResultDTO.ExceptionFail("error message", exception);
            Assert.False(resultDTO.IsSuccess);
            Assert.Equal("error message", resultDTO.ErrMsg);
            Assert.Same(exception, resultDTO.ExObj);
        }

        [Fact]
        public void ResultDTO_HandleError_ThrowsException()
        {
            var resultDTO = ResultDTO.ExceptionFail("error message", new Exception("exception message"));
            Assert.Throws<Exception>(() => ResultDTO.HandleError(resultDTO));
        }

        [Fact]
        public void ResultDTO_HandleError_ReturnsTrue()
        {
            var resultDTO = ResultDTO.Ok();
            Assert.True(ResultDTO.HandleError(resultDTO));
        }
        #endregion

        #region ResultDTO<DataType>
        [Fact]
        public void ResultDtoDataTypeInt_Ok_ReturnsResultDTOWithCorrectType()
        {
            int val = 42;
            Type typeVal = val.GetType();
            // Arrange
            var resultDTO = ResultDTO<int>.Ok(val);

            // Act
            var actualType = resultDTO.Data.GetType();

            // Assert
            Assert.Equal(typeVal, actualType);
        }

        [Fact]
        public void ResultDtoDataTypeInt_Ok_ReturnsResultDTOWithCorrectData()
        {
            // Arrange
            var expectedData = 42;
            var resultDTO = ResultDTO<int>.Ok(expectedData);

            // Act
            var actualData = resultDTO.Data;

            // Assert
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public void ResultDtoDataTypeInt_Ok_ReturnsTrue()
        {
            var resultDTO = ResultDTO<int>.Ok(default(int));
            Assert.True(resultDTO.IsSuccess);
            Assert.Null(resultDTO.ErrMsg);
            Assert.Null(resultDTO.ExObj);
        }

        [Fact]
        public void ResultDtoDataType_Fail_ReturnsFalseErrMsgNullExObj()
        {
            var resultDTO = ResultDTO<int>.Fail("error message");
            Assert.False(resultDTO.IsSuccess);
            Assert.Equal("error message", resultDTO.ErrMsg);
            Assert.Null(resultDTO.ExObj);
        }

        [Fact]
        public void ResultDtoDataType_ExceptionFail_ReturnsFalseErrMsgSameExObj()
        {
            var exception = new Exception("exception message");
            var resultDTO = ResultDTO<int>.ExceptionFail("error message", exception);
            Assert.False(resultDTO.IsSuccess);
            Assert.Equal("error message", resultDTO.ErrMsg);
            Assert.Same(exception, resultDTO.ExObj);
        }

        [Fact]
        public void ResultDtoDataType_HandleError_ThrowsException()
        {
            var resultDTO = ResultDTO<int>.ExceptionFail("error message", new Exception("exception message"));
            Assert.Throws<Exception>(() => ResultDTO<int>.HandleError(resultDTO));
        }

        [Fact]
        public void ResultDtoDataType_HandleError_ReturnsTrue()
        {
            var resultDTO = ResultDTO<int>.Ok(default(int));
            Assert.True(ResultDTO<int>.HandleError(resultDTO));
        }
        #endregion
    }
}
