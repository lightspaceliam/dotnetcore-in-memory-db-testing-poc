using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.Serialization;

/// <summary>
/// Product Juncture. Initially named incorrectly to illustrate the impact Sqlite has on migrations 
/// however, in .NET Core 5.0.5, the Sqlite framework was able to handle translations between MS SQL & SQLite commands.
/// </summary>
namespace Entities
{
    [Index(nameof(ProductId), nameof(Juncture), Name = "Index_ProductJuncture_ProductId_Juncture")]
    public class ProductJuncture : EntityBase
    {
        [NotMapped]
        public override string Name => $"{Product?.Name} {Juncture.Year} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Juncture.Month)}";

        [DataMember]
        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is required.")]
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Date the price changed on an individual product. 
        /// </summary>
        [DataMember]
        [Display(Name = "Juncture")]
        [Required(ErrorMessage = "Juncture is required.")]
        public DateTime Juncture { get; set; }

        // Foreign Key/s.

        [DataMember]
        [Display(Name = "Product")]
        [Required(ErrorMessage = "Product is required.")]
        public Guid ProductId { get; set; }

        // Navigation Properties.

        public Product Product { get; set; }
    }
}
