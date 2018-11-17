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
        [Range(0, 100)]
        public string Title { get; set; }

        [Required]
        [Range(0, 7000)]
        public string Content { get; set; }

        [Required]
        [Range(0, 35)]
        public string Slug { get; set; }

        public string ImageUri { get; set; }

        [Required]
        public User Author { get; set; }

        [Required]
        public bool Publish { get; set; }

        public DateTime? PublishDate { get; set; }

        public virtual IEnumerable<PostCategory> PostCategories { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }

    namespace Eager
    {
        public class Post
        {
            // Required only for PUT and DELETE operations
            public Guid? Id { get; set; }

            public string Title { get; set; }

            public string Content { get; set; }

            public string Slug { get; set; }

            public string ImageUri { get; set; }

            public Guid Author { get; set; }

            public bool Publish { get; set; }

            public DateTime? PublishDate { get; set; }

            public IEnumerable<Guid> Categories { get; set; }

            public DateTime CreatedOn { get; internal set; }

            public DateTime? ModifiedOn { get; internal set; }
        }
    }
}