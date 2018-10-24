using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.Entities.Models
{
    public class Post : Entity
    {
        #region Ctor

        public Post()
        {
        }

        public Post(Guid id) : base(id)
        {
        }

        #endregion Ctor

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        [Required]
        public string Slug { get; set; }

        public SystemUser Author { get; set; }

        [Required]
        public bool Publish { get; set; }

        public DateTime PublishDate { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedOn { get; }

        [Required]
        public SystemUser CreatedBy { get; internal set; }

        public DateTime? ModifiedOn { get; set; }

        public SystemUser ModifiedBy { get; set; }
    }
}