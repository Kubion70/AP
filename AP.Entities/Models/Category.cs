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

        public DateTime? ModifiedOn { get; set; }
    }

    namespace Eager
    {
        public class Category
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public DateTime CreatedOn { get; }

            public DateTime? ModifiedOn { get; set; }
        }
    }
}