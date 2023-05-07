using System.ComponentModel.DataAnnotations;

namespace PizzaStore.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email adresiniz ")]
        public string Email { get; set; }

        [Required]
        [StringLength(16, ErrorMessage = "{0} alanı  en az {2} karakter uzunluğunda olmalıdır..", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifreniz")]
        public string Password { get; set; }

        [Display(Name = "Beni hatırla")]
        public bool IsRememberMe { get; set; }

    }
}
