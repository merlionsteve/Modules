using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.Materials.Entities
{
    [Solid]
    public class MaterialCategory
    {
        [Key] public string Code { get; set; }

        public string Name { get; set; }

        [Comment("Is Active")]
        public bool IsActive { get; set; }
        public string? Description { get; set; }
    }

    public class VisualMaterialCategory
    {
        [VisualStyle]
        [LocalizationRequired(ErrorMessage ="The Code cannot be empty!")]
        public string Code { get; set; }

        [VisualStyle]
        [LocalizationRequired(ErrorMessage = "The Name cannot be empty!")]
        [LocalizationMaxLength(50, ErrorMessage = "The length is too long!")]
        public string Name { get; set; }

        [VisualStyle(Width = "50px")]
        public bool IsActive { get; set; } = true;
        [VisualStyle(Width = "300px")]
        [LocalizationMaxLength(200, ErrorMessage = "The length is too long!")]
        public string? Description { get; set; }
    }
}
