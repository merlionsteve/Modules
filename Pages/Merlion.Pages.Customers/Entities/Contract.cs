using Merlion.Base.DataAnnotations;
using Merlion.Database.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Merlion.Pages.Customers.Entities
{
    [Solid]
    public record Contract
    {
        [Comment("Code")]
        [Key]
        public string Code { get; set; }
       
        public string Name { get; set; }
        [Comment("Customer Code")]
        public string CustomerCode { get; set; }
        
        public DateOnly? SigningDate { get; set; }
        [Comment("Main Content")]
        public string? ContentInBrief { get; set; }
        [Comment("Is Closed")]
        public bool IsClosed { get; set; } = false;
    }

    public record VisualContract
    {
        [VisualStyle(Title = "Code")]
        [LocalizationRequired(ErrorMessage = "The code cannot be empty!")]
        [LocalizationMaxLength(20, ErrorMessage = "The length is too long!")]
        public string Code { get; set; }

        [LocalizationRequired(ErrorMessage = "The name cannot be empty!")]
        [LocalizationMaxLength(50, ErrorMessage = "The length is too long!")]

        [VisualStyle(Title ="Title")]
        public string Name { get; set; }

        [LocalizationRequired(ErrorMessage = "The customer cannot be empty!")]

        public string CustomerCode { get; set; }
        [VisualStyle(Title ="Customer")]
        public string CustomerName { get; set; }

        [VisualStyle(Title ="Signing Date", Format = "yyyy-MM-dd")]
        [LocalizationRequired(ErrorMessage = "The signing date cannot be empty!")]

        public DateTime? SigningDate { get; set; }
        [VisualStyle(Title = "Main Content", Width = "500px")]
        [LocalizationMaxLength(500,ErrorMessage ="error")]
        public string? ContentInBrief { get; set; }

        [VisualStyle(Title = "Is Closed")]
        public bool IsClosed { get; set; } = false;
    }
}
