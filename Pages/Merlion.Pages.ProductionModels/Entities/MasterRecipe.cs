using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.ProductionModels.Entities
{
    //[Solid]
    public class MasterRecipe
    {
        [Key] public string Code { get; set; }

        [Comment("Product Code")] public string ProductCode { get; set; }
        public float? QuanlityFrom { get; set; }
        public float? QuantityTo { get; set; }
        [Comment("Group Name")] public string GroupName { get; set; }
        [Comment("Group Counter")] public int Counter { get; set; }
        [Comment("Is Active")] public bool IsActive { get; set; }
        public string? Description { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }

        public string? ApprovedBy { get; set; }
        public DateTime ApprovedTime { get; set; }
    }

   // [Solid]
    public class MasterRecipeOperation
    {
        [Key]
        [Comment("Operation Code")] public string PhaseCode { get; set; }
        [Comment("Operation Name")] public string PhaseName { get; set; }
        [Comment("Recipe Code")] public string RecipeCode { get; set; }
        [Comment("Parent Code")] public string ParentCode { get; set; }
        public bool IsPhase { get; set; }  //sequence control,predecessor & successor
        public int StandardValue { get; set; }
        public string? Unit { get; set; }
    }
   // [Solid]
    public class RecipePhaseMaterial
    {
        [Key]
        [Comment("Material Code")] public string MaterialCode { get; set; }
        [Comment("Phase")] public string PhaseCode { get; set; }
        public float Quantity { get; set; }
        public string? Unit { get; set; }


        [Comment("IsPostGoodRequired")] public bool IsPostGoodRequired { get; set; }

    }
}
