using System;
using System.Collections.Generic;
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

        [Required]
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual IEnumerable<PostCategory> PostCategories { get; set; }
    }

    namespace Eager
    {
        public class Category
        {
            public Guid? Id { get; set; }

            public string Name { get; set; }

            public DateTime CreatedOn { get; }

            public DateTime? ModifiedOn { get; set; }
        }
    }
}