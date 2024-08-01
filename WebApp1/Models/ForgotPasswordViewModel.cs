using System.ComponentModel.DataAnnotations;




namespace ForgotPassword.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
