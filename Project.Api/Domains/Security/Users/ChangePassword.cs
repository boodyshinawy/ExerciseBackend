using System.ComponentModel.DataAnnotations;

public class ChangePassword
{
    [Required]
    public string? NewPassword { get; set; }
    [Required]
    public string? ConfirmPassword { get; set; }
}
