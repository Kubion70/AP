using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.Entities.Models
{
    public class Category : Entity
    {
        #region Ctor

        public Category()
        {
        }

        public Category(Guid id) : base(id)
        {
        }

        #endregion Ctor

        [Required]
        public string Name { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedOn { get; }

        [Required]
        public SystemUser CreatedBy { get; internal set; }

        public DateTime? ModifiedOn { get; set; }

        public SystemUser ModifiedBy { get; set; }
    }
}