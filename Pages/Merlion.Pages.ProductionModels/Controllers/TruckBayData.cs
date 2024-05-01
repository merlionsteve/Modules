using Merlion.Base.ServiceAnnotations;
using Merlion.Database;
using Merlion.Pages.ProductionModels.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Merlion.Pages.ProductionModels.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiJwtAuthorize]
    public class TruckBayData : Controller
    {
        [Microsoft.AspNetCore.Components.Inject]
        public IPartialDbContextFactory _pdbFactory { get; set; }
        public TruckBayData(IPartialDbContextFactory pdb)
        {
            this._pdbFactory = pdb;
        }

        [HttpGet, HttpPost]
        public async Task<IEnumerable<BillOfMaterial>> GetAll()
        {
            using var context = await this._pdbFactory.CreateDbContextAsync(this);
            return null;// context.Set<TruckArm>().Where(x => (x.ValidTo == null || x.ValidTo > DateTime.Now)).ToList();
        }
    }
}
