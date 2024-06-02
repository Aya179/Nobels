using System.ComponentModel.DataAnnotations;

namespace Nobels.Models
{
    public class Login
    {
        ////  [Required]
        // // public string Email { get; set; }
        // public string Username { get; set; }

        // [Required]
        // public string Password { get; set; }

        // public string ReturnUrl { get; set; }
        // [Display(Name = "Remember Me")]
        // public bool RememberMe { get; set; }

        //2
       
       // [EmailAddress]
      //  public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        [Required]
        public string number { get; set; }


    }
}
