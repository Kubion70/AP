using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AP.Entities.Models;

namespace AP.Entities
{
    public class Entity
    {
        #region Ctor

        public Entity() {

        }
        
        public Entity(Guid id) {
            if(id.Equals(Guid.Empty)){
                throw new ArgumentNullException("Entity constructor can't instantiatie with empty Guid");
            }
            Id = id;
        }

        #endregion Ctor

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; internal set; }
    }
}
