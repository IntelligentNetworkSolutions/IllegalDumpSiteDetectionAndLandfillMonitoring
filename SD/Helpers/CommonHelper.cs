﻿using System.ComponentModel;
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

        public static bool EnumerableHasDuplicatesByProperty<T>(IEnumerable<T> collection, Func<T, object>? property = null)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            if (collection.Count() < 2)
                return false;

            if (property is null)
                return collection.Count() != collection.Distinct().Count();

            return collection.GroupBy(property).Any(g => g.Count() > 1);
        }

        public static string ConvertWindowsPathToLinuxPathReplaceAllDashes(string windowsPath)
        {
            if (windowsPath.Contains("\\\\"))
                windowsPath = windowsPath.Replace("\\\\", "/");

            if (windowsPath.Contains("\\"))
                windowsPath = windowsPath.Replace("\\", "/");

            return windowsPath;
        }
    }
}
