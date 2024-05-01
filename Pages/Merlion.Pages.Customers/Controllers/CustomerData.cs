using Merlion.Base.ServiceAnnotations;
using Merlion.Database;
using Merlion.Pages.Customers.Entities;

//using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Merlion.Pages.Customers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [ApiJwtAuthorize]
    public class CustomerData : ControllerBase
    {
        [Microsoft.AspNetCore.Components.Inject]
        public IPartialDbContextFactory _pdbFactory { get; set; }
        public CustomerData(IPartialDbContextFactory pdb)
        {
            this._pdbFactory = pdb;
        }

        [HttpGet, HttpPost]
        public async Task<IEnumerable<Customer>> Get()
        {
            using var context = await this._pdbFactory.CreateDbContextAsync(this);
            return context.Set<Customer>().ToList();
        }

        [HttpGet("{code}")]
        public async Task<Customer?> Get(string code)
        {
            using var context = await this._pdbFactory.CreateDbContextAsync(this);
            return context.Set<Customer>().FirstOrDefault(x => x.Code == code);
        }

        [HttpGet("look"), HttpPost("look")]
        public async Task<Customer?> Get(int category, string code)
        {
            using var context = await this._pdbFactory.CreateDbContextAsync(this);
            return context.Set<Customer>().FirstOrDefault(x => x.Code == category.ToString());
        }
    }

   
}

