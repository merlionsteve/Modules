using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.ProductionModels.Entities
{
    [Reference]
    public class MaterialMaster
    {
        [Key] public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string CategoryCode { get; set; }
        public string? BaseUnit { get; set; }            //L, KG, m3, ton...
        public string? Plant { get; set; }
    }
}
