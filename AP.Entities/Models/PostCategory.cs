using System;

namespace AP.Entities.Models
{
    public class PostCategory
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        /// <summary>
        /// Relational equality checking if related guid exists in relation.
        /// This works because Guids are unique regardless of entity type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return PostId.Equals(obj) ||
                   CategoryId.Equals(obj);
        }
    }
}