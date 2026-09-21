using System.Net.Http.Headers;
using System.Net.Http.Json;
using FreeTrack.Application.Interfaces;
using FreeTrack.Domain.Entities;
using FreeTrack.Domain.Enums;
using FreeTrack.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FreeTrack.Infrastructure.Services;

/// <summary>
/// Always writes an AdminNotification row (so the dashboard works out of the box).
/// Additionally sends an email through Resend's HTTPS API when Email:ApiKey is configured
/// (Render's free plan blocks SMTP). Email failures are logged, never thrown, so they
/// can't break the booking flow itself.
/// </summary>
public class BookingNotificationService : IBookingNotificationService
{
    private readonly IAdminNotificationRepository _notifications;
    private readonly HttpClient _http;
    private readonly EmailOptions _email;
    private readonly ILogger<BookingNotificationService> _logger;

    public BookingNotificationService(
        IAdminNotificationRepository notifications,
        HttpClient http,
        IOptions<EmailOptions> email,
        ILogger<BookingNotificationService> logger)
    {
        _notifications = notifications;
        _http = http;
        _email = email.Value;
        _logger = logger;
    }

    public async Task NotifyAdminNewRequestAsync(Booking booking, CancellationToken ct = default)
    {
        var roomName = booking.Room?.Name ?? "een ruimte";
        var summary = $"Nieuwe boekingsaanvraag: {roomName}, {booking.StartUtc:dd/MM/yyyy} \u2013 {booking.EndUtc:dd/MM/yyyy}, van {booking.CustomerName}.";

        await _notifications.AddAsync(new AdminNotification { BookingId = booking.Id, Message = summary }, ct);
        await _notifications.SaveChangesAsync(ct);

        await TrySendEmailAsync(
            to: _email.AdminEmail,
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
        if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(_email.ApiKey))
        {
            _logger.LogInformation("Email not configured \u2014 skipping email. Would have sent to {To}: {Subject}", to, subject);
            return;
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "emails")
            {
                Content = JsonContent.Create(new { from = _email.FromAddress, to = new[] { to }, subject, text = body })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _email.ApiKey);

            using var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Email to {To} was rejected by Resend: {Status} {Body}",
                    to, (int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send booking notification email to {To}", to);
        }
    }
}
