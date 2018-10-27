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

        [Required]
        public string Content { get; set; }

        [Required]
        public string Slug { get; set; }

        [Required]
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

    namespace Eager
    {
        public class Post
        {
            public Guid Id { get; set; }

            [Required]
            public string Title { get; set; }

            [Required]
            public string Content { get; set; }

            [Required]
            public string Slug { get; set; }

            [Required]
            public Guid Author { get; set; }

            [Required]
            public bool Publish { get; set; }

            public DateTime PublishDate { get; set; }

            public IEnumerable<Guid> Categories { get; set; }

            public DateTime CreatedOn { get; internal set; }

            public Guid CreatedBy { get; internal set; }

            public DateTime? ModifiedOn { get; internal set; }

            public Guid ModifiedBy { get; internal set; }
        }
    }
}