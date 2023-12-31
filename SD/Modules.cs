﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SD
{
    public class Module
    {
        /// <summary>
        /// Unique module code (used in DB config for tenants access to modules)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Can be used as a display name/title of the tool
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Module description for internal user (INS only)
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// All module codes
    /// </summary>
    public static class Modules
    {
        public static ICollection<Module> GetAll()
        {
            Type t = typeof(Modules);
            FieldInfo[] fields = t.GetFields();
            var modules = new Collection<Module>();
            foreach (FieldInfo fi in fields)
            {
                modules.Add((Module)fi.GetValue(null));
            }
            return modules;
        }

        public static void CheckModuleValuesForDuplicates()
        {
            var valuesList = GetAll().Select(s => s.Value);
            if (valuesList.Count() != valuesList.Distinct().Count())
            {
                throw new Exception("SD.Modules have value duplicates. Rename the duplicated values!");
            }
        }

        public static readonly Module UserManagement = new Module { Value = "UserManagement", Description = "Management of users" };
        public static readonly Module AuditLog = new Module { Value = "AuditLog", Description = "Audit Log" };
        public static readonly Module Admin = new Module { Value = "Admin", Description = "Admin" };
        public static readonly Module SpecialActions = new Module { Value = "SpecialActions", Description = "Special Actions" };

    }
}
