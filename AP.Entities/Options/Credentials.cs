using System.ComponentModel.DataAnnotations;

namespace AP.Entities.Options
{
    public class Credentials
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}