using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class AuthInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }

    public class SignUpResponeModel
    {
        public string Message { get; set; }
    }

    public class LoginResponeModel
    {
        public string Token { get; set; }
    }

    public class ProfileResponeModel
    {
        public string Username { get; set; }
    }

}
