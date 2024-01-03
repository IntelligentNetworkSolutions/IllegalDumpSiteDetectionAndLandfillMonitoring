using Audit.Core;
using Audit.EntityFramework;
using Entities;
using SD.Enums;

namespace MainApp.MVC.Infrastructure.Register
{
    public static class RegisterAudit
    {
        public static IServiceCollection RegisterAuditNet(this IServiceCollection services)
        {
            Audit.Core.Configuration.Setup()
               .UseEntityFramework(_ => _
                   .AuditTypeMapper(t => typeof(AuditLog))
                   .AuditEntityAction<AuditLog>((ev, entry, entity) =>
                   {

                       entity.AuditData = entry.ToJson();
                       entity.EntityType = entry.EntityType.Name;
                       entity.AuditDate = DateTime.Now;
                       entity.TablePk = entry.PrimaryKey.First().Value.ToString();
                       entity.AuditAction = entry.Action;
                       entity.AuditInternalUser = ev.CustomFields["audit_internal_user"] != null ? ev.CustomFields["audit_internal_user"]?.ToString() : "none";
                   })
                   .IgnoreMatchedProperties(true));


            Audit.Core.Configuration.AddOnSavingAction(scope =>
            {
                var efEvent = scope.GetEntityFrameworkEvent();
                if (!efEvent.Success)
                {
                    scope.Discard();
                }
                else
                {
                    var notIncluded = Enum.GetNames(typeof(AuditLogIgnoredTables));
                    efEvent.Entries.RemoveAll(e => notIncluded.Contains(e.Table));

                    if (efEvent.Entries.Count == 0)
                    {
                        scope.Discard();
                    }

                    foreach (var entry in efEvent.Entries)
                    {
                        if (entry.Action == "Update")
                        {

                            foreach (var change in entry.Changes)
                            {
                                if (change.OriginalValue is NetTopologySuite.Geometries.Geometry)
                                {
                                    change.OriginalValue = Entities.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)change.OriginalValue);
                                    change.NewValue = Entities.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)change.NewValue);
                                }
                            }

                            foreach (var columnValue in entry.ColumnValues)
                                entry.ColumnValues.Remove(columnValue);
                        }

                        var tempList = entry.ColumnValues.ToList();

                        foreach (var colValue in tempList)
                        {
                            if (colValue.Value is NetTopologySuite.Geometries.Geometry)
                            {
                                entry.ColumnValues[colValue.Key] = Entities.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)colValue.Value);
                            }
                        }

                    }

                }
            });

            return services;
        }
    }
}
