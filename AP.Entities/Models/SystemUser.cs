using System;
using System.ComponentModel.DataAnnotations;

namespace AP.Entities.Models
{
    public class SystemUser : Entity
    {
        #region Ctor

        public SystemUser()
        {
        }

        public SystemUser(Guid id) : base(id)
        {
        }

        #endregion Ctor

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}