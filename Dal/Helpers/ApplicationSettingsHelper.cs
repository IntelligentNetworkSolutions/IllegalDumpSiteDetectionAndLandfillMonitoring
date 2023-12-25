using Dal.Repositories;
using Entities;
//using Services.Interfaces.Repositories;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Helpers
{
    public class ApplicationSettingsHelper
    {

        private readonly ApplicationSettingsDa _applicationSettingsDa;
        private readonly ConcurrentDictionary<string, ApplicationSettings> _cache;
        public ApplicationSettingsHelper(ApplicationSettingsDa applicationSettingsDa)
        {
            _applicationSettingsDa = applicationSettingsDa;
            _cache = new ConcurrentDictionary<string, ApplicationSettings>();
        }

        // TODO: This should be private
        public string GetApplicationSetting(string configurationName, string defaultValue = null)
        {
            // TODO: Needs try catch , if try catch here it can be removed from all following parametarized get methods
            var appSetting = 
                _cache.GetOrAdd(configurationName, 
                                i => _applicationSettingsDa.GetApplicationConfiguration(configurationName));

            if (appSetting != null)
                return appSetting.Value;
            
            if (appSetting == null && defaultValue != null)
                return defaultValue;
            
            throw new Exception("Application setings error: The settings for " + configurationName + " does not exist");
        }

        public string GetApplicationSettingString(string configurationName, string defaultValue = null)
        {
            try
            {
                var value = GetApplicationSetting(configurationName, defaultValue);
                return (string)value;
            }
            catch (Exception e)
            {
                // TODO: Review -> Higher probablity of setting not found than invalid cast to string
                throw new Exception("Application setings error: Invalid cast to string " + configurationName, e);
            }
        }

        public int GetApplicationSettingInteger(string configurationName, string defaultValue = null)
        {
            // TODO: Review -> Higher probablity of setting not found than invalid cast to string, add try parse to know if not found or parse error
            try
            {
                var value = GetApplicationSetting(configurationName, defaultValue);
                return int.Parse(value);
            }
            catch (Exception e)
            {
                throw new Exception("Application setings error: Invalid cast to integer " + configurationName, e);
            }
        }

        public DateTime GetApplicationSettingDateTime(string configurationName, string defaultValue = null)
        {
            // TODO: Review -> Higher probablity of setting not found than invalid cast to string, add try parse to know if not found or parse error
            try
            {
                var value = GetApplicationSetting(configurationName, defaultValue);
                return DateTime.Parse(value);
            }
            catch (Exception e)
            {
                throw new Exception("Application setings error: Invalid cast to DateTime " + configurationName, e);
            }
        }

        public decimal GetApplicationSettingDecimal(string configurationName, string defaultValue = null)
        {
            // TODO: Review -> Higher probablity of setting not found than invalid cast to string, add try parse to know if not found or parse error
            try
            {
                var value = GetApplicationSetting(configurationName, defaultValue);
                return decimal.Parse(value);
            }
            catch (Exception e)
            {
                throw new Exception("Application setings error: Invalid cast to decimal " + configurationName, e);
            }
        }

        public bool GetApplicationSettingBool(string configurationName, string defaultValue = null)
        {
            // TODO: Review -> Higher probablity of setting not found than invalid cast to string, add try parse to know if not found or parse error
            try
            {
                var value = GetApplicationSetting(configurationName, defaultValue);
                return bool.Parse(value);
            }
            catch (Exception e)
            {
                throw new Exception("Application setings error: Invalid cast to bool " + configurationName, e);
            }
        }
    }
}
