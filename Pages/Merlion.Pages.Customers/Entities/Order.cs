using Merlion.Database.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.Customers.Entities
{
    [Reference]
    public class Order
    {
        [Key]
        public string Code { get; set; }

        public string ContractCode { get; set; }

    }
}
