using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.ProductionModels.Entities
{
    [Solid]
    public class BillOfMaterial
    {
        [Key]
        public string Code { get; set; }

        [Comment("Product Code")] public string ProductCode { get; set; }
        [Comment("Alternative Number")] public int Alternative { get; set; }

        [Comment("Base Quqntity")] public float BaseQuantity { get; set; }
        [Comment("Is Active")] public bool IsActive { get; set; }
    }
    [Solid]
    [PrimaryKey(nameof(ComponentCode), nameof(BOMCode))]
    public class BOMComponent
    {
        [Comment("Material Code")] public string? ComponentCode { get; set; }
        [Comment("BOM Code")] public string BOMCode { get; set; }
        public float Quantity { get; set; }
    }

    public class VisualBOM
    {
        [VisualStyle(Title = "Code")]
        [LocalizationRequired(ErrorMessage ="The Code cannot be empty")]
        public string Code { get; set; }

        //[VisualStyle(Title = "Product Code")] 
        [LocalizationRequired(ErrorMessage = "The Product cannot be empty")]

        public string ProductCode { get; set; }

        [VisualStyle(Title = "Product/Alt", Auto = false)]
        public string Product { get; set; }

        //[VisualStyle(Title = "Alternative")] 
        public int Alternative { get; set; } = 1;

        //[VisualStyle(Title = "Base Quqntity")] 
        [LocalizationRequired(ErrorMessage = "The Base Quantity cannot be empty")]
        public float BaseQuantity { get; set; } = 100.00f;

        //[VisualStyle(Title = "Unit")] 
        public string? Unit { get; set; }  //unit of meassurement in materal master

        //[VisualStyle(Title = "Is Active")] 
        public bool IsActive { get; set; }
    }

    public class VisualBOMComponent
    {
        public string BOMCode { get; set; }
        [VisualStyle(Title = "Component Code")] public string? ComponentCode { get; set; }
        [VisualStyle(Title = "Component")] public string? Component { get; set; }
        [VisualStyle(Title = "Quantity", Format = "0.00")] public float Quantity { get; set; }
        [VisualStyle(Title = "Unit")] public string? Unit { get; set; }
    }
}
