using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Merlion.Pages.ProductionModels.Entities
{
   // [Solid]
    [PrimaryKey(nameof(Code), nameof(Version))]
    public class MBR
    {
        [Key]
        public string Code { get; set; }
        [Key] public string Version { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string FileName { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
