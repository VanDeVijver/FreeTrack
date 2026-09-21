using System.ComponentModel.DataAnnotations;

namespace FreeTrack.Web.Models;

public class LoginViewModel
{
    [Required, Display(Name = "Gebruikersnaam")]
    public string Username { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Wachtwoord")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
