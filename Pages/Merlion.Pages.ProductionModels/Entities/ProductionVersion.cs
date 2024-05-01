using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.ProductionModels.Entities
{
    //[Solid]
    [PrimaryKey(nameof(MaterialCode), nameof(VersionCode))]
    public class ProductionVersion
    {
        [Comment("Material Code")] public string MaterialCode { get; set; }
        [Comment("Version Code")] public string VersionCode { get; set; }

        [Comment("BOM Alternative")]
        public string? BOMAlternative { get; set; }

        [Comment("Master Recipe Code")]
        public string? MasterRecipeCode { get; set; }


    }


}
