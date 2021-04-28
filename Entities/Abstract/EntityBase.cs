using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Entities
{
    [DataContract]
    public abstract class EntityBase : IEntity
    {
        [DataMember]
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember]
        [Display(Name = "Name")]
        [CustomValidation(typeof(EntityBase), "NameIsSometimesRequired")]
        [StringLength(150, ErrorMessage = "Name exceeds {1} characters.")]
        public virtual string Name { get; set; }

        [DataMember]
        [Display(Name = "Last Modified")]
        [ConcurrencyCheck]
        public DateTime LastModified { get; set; }

        [DataMember]
        [Display(Name = "Created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// We are marking Name as required for all instances of the BaseEntity class except for those listed here.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ValidationResult NameIsSometimesRequired(string value, ValidationContext context)
        {
            return value != null
                || (context.ObjectInstance is ProductJuncture)
                ? ValidationResult.Success
                : new ValidationResult("Name is required.");
        }
    }
}
