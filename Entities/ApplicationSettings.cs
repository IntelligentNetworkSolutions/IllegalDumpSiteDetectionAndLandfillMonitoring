using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SD.Enums;

namespace Entities
{
    public partial class ApplicationSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string? Module { get; set; }
        public ApplicationSettingsDataType DataType { get; set; }
    }
}
