using System.ComponentModel.DataAnnotations;

namespace Project.Api;

public class RegisterModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Required(ErrorMessage = "User Name is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    public string ConfirmPassword { get; set; }

    public string FirmId { get; set; }
    public string FirmName { get; set; }

    public string? Status { get; set; } = "Active";
    
    public List<ApplicationRole> Roles { get; set; }

}
