using System.ComponentModel.DataAnnotations;

namespace Project.Api;

public class LoginModel
{
    [Required(ErrorMessage = "User Name is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    public bool RememberMe{ get; set; } 
}
