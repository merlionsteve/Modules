using Merlion.Base.ServiceAnnotations;
using Merlion.Database;
using Merlion.Pages.Materials.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Merlion.Pages.Materials.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiJwtAuthorize]
    public class MaterialData : Controller
    {
        [Microsoft.AspNetCore.Components.Inject]
        public IPartialDbContextFactory _pdbFactory { get; set; }
        public MaterialData(IPartialDbContextFactory pdb)
        {
            this._pdbFactory = pdb;
        }

        [HttpGet, HttpPost]
        public async Task<IEnumerable<MaterialMaster>> GetAll()
        {
            using var context = await this._pdbFactory.CreateDbContextAsync(this);
            return context.Set<MaterialMaster>().Where(x=>(x.ValidTo==null || x.ValidTo>DateTime.Now)).ToList();
        }

        [HttpGet("{code}")]
        public async Task<MaterialMaster?> Get(string materialCode)   //parameter should be same, here should be code, not materialCode
        {
            using var context = await this._pdbFactory.CreateDbContextAsync(this);
            return context.Set<MaterialMaster>().FirstOrDefault(x => x.Code == materialCode);
        }

        [HttpGet("Category/{catCode}"), HttpPost("Category/{catCode}")] 
        public async Task<IEnumerable<MaterialMaster>> GetAllOfOneCategory(string catCode)  //recommend to place the parameter in function definition, not route definition
        {
            using var context = await this._pdbFactory.CreateDbContextAsync(this);
            return context.Set<MaterialMaster>().Where(x => x.CategoryCode == catCode && (x.ValidTo == null || x.ValidTo > DateTime.Now)).ToList();
        }
    }
    public enum MaterialCategory
    {
        [Description("Raw Materials")]
        RawMaterial = 1,
        [Description("Auxiliary Materials")]
        AuxiliaryMaterial = 2,
        [Description("Packaging Materials")]
        PackagingMaterial = 4,
        [Description("Intermediates")]
        Intermediates = 8,
        [Description("Final Product")]
        FinalProduct = 16,
        [Description("Others")]
        Others = 32
    }
}
