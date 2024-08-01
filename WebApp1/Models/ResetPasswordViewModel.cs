using System.ComponentModel.DataAnnotations;

namespace ResetPassword.Models
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Code { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
