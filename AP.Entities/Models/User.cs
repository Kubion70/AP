using System;
using System.ComponentModel.DataAnnotations;

namespace AP.Entities.Models
{
    public class User : Entity
    {
        #region Ctor

        public User()
        {
        }

        public User(Guid id) : base(id)
        {
        }

        #endregion Ctor

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    namespace Eager
    {
        public class User
        {
            public Guid Id { get; set; }

            public string Username { get; set; }

            public string Password { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}