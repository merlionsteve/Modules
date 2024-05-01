using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.Customers.Entities
{
    [Solid]
    public record Customer
    {
        [Comment("Code")]
        [Key()]
        public string Code { get; set; }
        [Comment("Name")]
        public string Name { get; set; }
        [Comment("Address")]
        public string? Address { get; set; }

        [Comment("Contacter")]
        public string? Contacter { get; set; }

        [Comment("Contact Information")]
        public string? Contact { get; set; }
        [Comment("Remark")]
        public string? Remark { get; set; }
    }

    public record VisualCustomer
    {
        [VisualStyle(Title = "Code")]
        [LocalizationRequired(ErrorMessage = "The code cannot be empty!")]
        [LocalizationMaxLength(20, ErrorMessage = "The length is too long!")]
        public string Code { get; set; }

        [LocalizationRequired(ErrorMessage = "The Name cannot be empty!")]
        [LocalizationMaxLength(100, ErrorMessage = "The length is too long!")]
        [VisualStyle(Title = "Name")]
        public string Name { get; set; }

        [VisualStyle(Title = "Address")]
        [LocalizationMaxLength(100, ErrorMessage = "The length is too long!")]
        public string? Address { get; set; }


        [VisualStyle(Title = "Contacter")]
        [LocalizationMaxLength(100, ErrorMessage = "The length is too long!")]
        public string? Contacter { get; set; }

        [VisualStyle(Title = "Contact Information")]
        [LocalizationMaxLength(100, ErrorMessage = "The length is too long!")]
        public string? Contact { get; set; }

        [VisualStyle(Title = "Remark")]
        [LocalizationMaxLength(100, ErrorMessage = "The length is too long!")]
        public string? Remark { get; set; }
    }
}
