using System.ComponentModel.DataAnnotations;

namespace Shelflyx.Models.ViewModels
{
    public class Login
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
