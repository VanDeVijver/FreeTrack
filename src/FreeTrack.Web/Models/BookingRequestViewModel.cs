using System.ComponentModel.DataAnnotations;

namespace FreeTrack.Web.Models;

public class BookingRequestViewModel
{
    public int RoomId { get; set; }
    public string RoomSlug { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vul je naam in.")]
    [Display(Name = "Naam")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vul je e-mailadres in.")]
    [EmailAddress(ErrorMessage = "Dit lijkt geen geldig e-mailadres.")]
    [Display(Name = "E-mail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Dit lijkt geen geldig telefoonnummer.")]
    [Display(Name = "Telefoon")]
    public string? CustomerPhone { get; set; }

    [StringLength(2000)]
    [Display(Name = "Bericht (optioneel)")]
    public string? Message { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Startdatum")]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Einddatum")]
    public DateTime EndDate { get; set; }
}
