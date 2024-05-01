using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.Materials.Entities
{
    [Solid]
    public class MaterialMaster
    {
        [Key] public string Code { get; set; }
        [Comment("Short Description")] public string ShortDescription { get; set; }
        public string CategoryCode { get; set; }

        [Comment("Base Unit of Meassure")] public string? BaseUnit { get; set; }            //L, KG, m3, ton...
        [Comment("Sales Unit")] public string? SalesUnit { get; set; }          //box, bucket
        [Comment("Unit Specification")] public string? UnitSpec { get; set; }           //60盒*10只*20ml
        [Comment("Factor(sales unit to base unit)")] public float? UnitFactor { get; set; }          //10 = 50*10*20/1000, factor for converting the sales unit to base unit
        [Comment("Validity Period(M)")] public int? ValidityPeriod { get; set; } = 6;

        [Comment("Valid From")] public DateTime? ValidFrom { get; set; }
        [Comment("Valid To")] public DateTime? ValidTo { get; set; }
        [Comment("Storage Conditions")] public string? StorageConditions { get; set; }
        public string? Plant { get; set; }

        //public int CreatedBy { get; set; }
        //public DateTime CreatedTime { get; set; }
        //public int? UpdatedBy { get; set;}
        //public DateTime? UpdatedTime { get; set;}
        public string? Remark { get; set; }
    }

    public class VisualMaterialMaster
    {
        [VisualStyle]
        [LocalizationRequired(ErrorMessage = "The Code cannot be empty!")]
        public string Code { get; set; }
        [VisualStyle]
        [LocalizationRequired(ErrorMessage = "The description cannot be empty!")]
        public string ShortDescription { get; set; }

        [LocalizationRequired(ErrorMessage = "The category cannot be empty!")]
        public string CategoryCode { get; set; }

        [VisualStyle]
        public string CategoryName { get; set; }

        //[VisualStyle(Title ="Base Unit")]
        public string? BaseUnit { get; set; }            //L, KG, m3, ton...
        //[VisualStyle(Title = "Sales Unit")]
        public string? SalesUnit { get; set; }          //box, bucket
        [VisualStyle(Title = "Unit Specification")]
        public string? UnitSpec { get; set; }           //60盒*10只*20ml
        //[VisualStyle(Title = "Factor(sales unit to base unit)")]
        public float? UnitFactor { get; set; }          //10 = 50*10*20/1000, factor for converting the sales unit to base unit
        [VisualStyle(Title = "Validity Period(M)")]
        public int? ValidityPeriod { get; set; } = 6;

        //[VisualStyle(Title = "Valid From")]
        public DateTime? ValidFrom { get; set; }
        //[VisualStyle(Title = "Valid To")]
        public DateTime? ValidTo { get; set; }
        public string? StorageConditions { get; set; }

        [LocalizationMaxLength(100, ErrorMessage = "The length is too long!")]
        public string? Plant { get; set; }
        //public int CreatedBy { get; set; }
        //public DateTime CreatedTime { get; set; }
        //public int? UpdatedBy { get; set; }
        //public DateTime? UpdatedTime { get; set; }
        //[VisualStyle(Title = "Remark")]
        [LocalizationMaxLength(100, ErrorMessage = "The length is too long!")]
        public string? Remark { get; set; }
    }
}
