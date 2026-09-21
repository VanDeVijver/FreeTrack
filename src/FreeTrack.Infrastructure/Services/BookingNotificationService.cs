using System.Net;
using System.Net.Mail;
using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Domain.Enums;
using FreeTrack.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FreeTrack.Infrastructure.Services;

/// <summary>
/// Always writes an AdminNotification row (so the dashboard works out of the box).
/// Additionally attempts an SMTP send when Smtp:Host is configured — email failures
/// are logged, never thrown, so they can't break the booking flow itself.
/// </summary>
public class BookingNotificationService : IBookingNotificationService
{
    private readonly IAdminNotificationRepository _notifications;
    private readonly SmtpOptions _smtp;
    private readonly ILogger<BookingNotificationService> _logger;

    public BookingNotificationService(
        IAdminNotificationRepository notifications,
        IOptions<SmtpOptions> smtp,
        ILogger<BookingNotificationService> logger)
    {
        _notifications = notifications;
        _smtp = smtp.Value;
        _logger = logger;
    }

    public async Task NotifyAdminNewRequestAsync(Booking booking, CancellationToken ct = default)
    {
        var roomName = booking.Room?.Name ?? "een ruimte";
        var summary = $"Nieuwe boekingsaanvraag: {roomName}, {booking.StartUtc:dd/MM/yyyy} \u2013 {booking.EndUtc:dd/MM/yyyy}, van {booking.CustomerName}.";

        await _notifications.AddAsync(new AdminNotification { BookingId = booking.Id, Message = summary }, ct);
        await _notifications.SaveChangesAsync(ct);

        await TrySendEmailAsync(
            to: _smtp.AdminEmail,
            subject: "Nieuwe boekingsaanvraag \u2014 FreeTrack",
            body: $"{summary}\n\n" +
                  $"E-mail klant: {booking.CustomerEmail}\n" +
                  $"Telefoon: {booking.CustomerPhone}\n" +
                  $"Bericht: {booking.Message}\n\n" +
                  "Beoordeel deze aanvraag in het admin-dashboard (/Admin).");
    }

    public async Task NotifyCustomerDecisionAsync(Booking booking, CancellationToken ct = default)
    {
        var approved = booking.Status == BookingStatus.Approved;
        var roomName = booking.Room?.Name ?? "de ruimte";

        var subject = approved
            ? "Je boeking is bevestigd \u2014 FreeTrack"
            : "Update over je boekingsaanvraag \u2014 FreeTrack";

        var body = approved
            ? $"Goed nieuws! Je boeking voor {roomName} ({booking.StartUtc:dd/MM/yyyy} \u2013 {booking.EndUtc:dd/MM/yyyy}) is bevestigd."
            : $"Helaas kunnen we je aanvraag voor {roomName} ({booking.StartUtc:dd/MM/yyyy} \u2013 {booking.EndUtc:dd/MM/yyyy}) niet bevestigen."
              + (string.IsNullOrWhiteSpace(booking.AdminNote) ? "" : $" Reden: {booking.AdminNote}");

        // Kept out of AdminNotifications on purpose — that table is for things the
        // admin needs to act on, and a decision they just made isn't one of them.
        await TrySendEmailAsync(booking.CustomerEmail, subject, body);
    }

    private async Task TrySendEmailAsync(string? to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(_smtp.Host))
        {
            _logger.LogInformation("SMTP not configured \u2014 skipping email. Would have sent to {To}: {Subject}", to, subject);
            return;
        }

        try
        {
            using var client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                Credentials = new NetworkCredential(_smtp.Username, _smtp.Password),
                EnableSsl = _smtp.EnableSsl,
                Timeout = 10_000 // default is 100s; a blocked SMTP port must not stall the booking request
            };
            using var mail = new MailMessage(_smtp.FromAddress ?? _smtp.Username ?? "noreply@freetrack.local", to, subject, body);
            await client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send booking notification email to {To}", to);
        }
    }
}
