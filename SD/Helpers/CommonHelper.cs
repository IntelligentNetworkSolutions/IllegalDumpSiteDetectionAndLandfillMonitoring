using System.ComponentModel;
using System.Globalization;

namespace SD.Helpers
{
    public static class CommonHelper
    {
        public static object? CastTo<ReturnType>(object? inputValue)
        {
            if(inputValue is null) 
                return null;

            try
            {
                // TODO: If setting is JSON serialize
                object? castValueAsObj = CastToType(inputValue, typeof(ReturnType));

                if (castValueAsObj is null)
                    return null;

                ReturnType castValueAsReturnType = (ReturnType)castValueAsObj;

                return castValueAsReturnType;
            }
            catch(Exception ex)
            {
                // TODO: Handle
                return null;
            }
        }

        private static object? CastToType(object inputValue, Type outputType) 
        {
            if (inputValue is null)
                return null;

            Type? inputType = inputValue.GetType();
            if (inputType == outputType)
                return inputValue;

            CultureInfo culture = CultureInfo.InvariantCulture;

            TypeConverter outputTypeConverter = TypeDescriptor.GetConverter(outputType);
            if (outputTypeConverter.CanConvertFrom(inputValue.GetType())) 
                return outputTypeConverter.ConvertFrom(null, culture, inputValue);

            TypeConverter inputTypeConverter = TypeDescriptor.GetConverter(inputType);
            if (inputTypeConverter.CanConvertTo(outputType))
                return inputTypeConverter.ConvertTo(null, culture, inputValue, outputType);

            if (!outputType.IsInstanceOfType(inputValue))
                return Convert.ChangeType(inputValue, outputType, culture);

            return inputValue;
        }
    }
}
