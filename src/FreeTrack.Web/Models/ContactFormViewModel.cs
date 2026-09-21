using System.ComponentModel.DataAnnotations;

namespace FreeTrack.Web.Models;

/// <summary>Backs both "Vertel ons over je project" and "Contact Us" forms.</summary>
public class ContactFormViewModel
{
    [Required(ErrorMessage = "Vul je naam in.")]
    [Display(Name = "Naam")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vul je e-mailadres in.")]
    [EmailAddress(ErrorMessage = "Dit lijkt geen geldig e-mailadres.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Dit lijkt geen geldig telefoonnummer.")]
    [Display(Name = "Telefoon")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Vertel ons iets over je project.")]
    [StringLength(2000)]
    [Display(Name = "Je project")]
    public string Message { get; set; } = string.Empty;

    [Display(Name = "Gewenste ruimte")]
    public int? RoomId { get; set; }

    [Display(Name = "Gewenste dienst")]
    public int? StudioServiceId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Gewenste datum")]
    public DateTime? PreferredDate { get; set; }

    // Populated by the controller to fill the <select> lists; not bound from the form post.
    public List<RoomOption> RoomOptions { get; set; } = new();
    public List<ServiceOption> ServiceOptions { get; set; } = new();

    public record RoomOption(int Id, string Name);
    public record ServiceOption(int Id, string Category);
}
