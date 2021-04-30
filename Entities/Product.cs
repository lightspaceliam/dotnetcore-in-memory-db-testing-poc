using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace Entities
{
    [Index(nameof(Code), IsUnique = true, Name = "Unique_Index_Code")]
    public class Product : EntityBase
    {
        [DataMember]
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is required.")]
        [StringLength(50, ErrorMessage = "Code exceeds {1} characters.")]
        public string Code { get; set; }

        [DataMember]
        [Display(Name = "Colour")]
        [Required(ErrorMessage = "Colour is required.")]
        [StringLength(100, ErrorMessage = "Colour exceeds {1} characters.")]
        public string Colour { get; set; }

        /// <summary>
        /// Property with no string length to demonstrate nvarchar(max) incompatibility with SQLite.
        /// </summary>
        [DataMember]
        [Display(Name = "Data")]
        public string Data { get; set; }

        /// <summary>
        /// Debatable if this calculated property should be here as it has nothing to do with the database and could be interpreted as a violation separating concerns.
        /// </summary>
        [NotMapped]
        public decimal? Price
        {
            get
            {
                var firstJuncture = ProductJunctures?
                    .OrderByDescending(p => p.Juncture)
                    .FirstOrDefault();

                return firstJuncture?.Price;
            }
        }

        // Navigation Properties.

        public List<ProductJuncture> ProductJunctures { get; set; }
    }
}
