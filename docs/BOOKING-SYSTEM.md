# Booking calendar + admin approval — how it works

## The flow
1. Visitor opens `/Bookings/Calendar/{roomSlug}` (linked from each room card on
   the home page) and sees a month grid: **Available** / **In aanvraag**
   (pending) / **Bezet** (booked, i.e. approved) / greyed-out past days.
2. Clicking an available day goes to `/Bookings/Request/{roomSlug}?date=...`,
   a short form (name, email, phone, dates, message).
3. Submitting **never** creates a confirmed booking — it always lands as
   `BookingStatus.Pending`. At that point:
   - An `AdminNotification` row is written (so `/Admin` shows it even with no
     email configured).
   - `BookingNotificationService` best-effort emails `Smtp:AdminEmail` if
     `Smtp:Host` is set in config. If sending fails, it's logged, not thrown —
     the booking still gets created.
4. Admin logs in at `/Account/Login` and reviews requests at `/Admin`
   (defaults to the "In afwachting" / pending tab).
5. **Approve** or **Reject** — either way `BookingNotificationService` emails
   the customer with the decision (again, best-effort).
6. Only `Approved` bookings block the calendar for other visitors. A
   `Pending` request does **not** block the slot — someone else can still
   request the same dates; the admin decides who actually gets it. This was
   a deliberate choice given "band/manager are just examples of user types" —
   there's no reservation-holding/locking behavior, just visibility.

## Admin login
`AccountController` now uses ASP.NET Core Identity (`SignInManager<IdentityUser>`),
backed by the same `AppDbContext` (Identity's `AspNetUsers`/`AspNetRoles` tables
live alongside the app's own tables). A single admin account is seeded in
`Program.cs` on startup, in development only:

- Username: `admin`
- Password: `test`
- Role: `Admin` (checked via `[Authorize(Roles = "Admin")]` on `AdminController`)

The password policy in `Program.cs` (`AddIdentity<...>(options => ...)`) is
deliberately relaxed — no digit/uppercase/symbol required, minimum length 4 —
specifically so `"test"` passes validation. **Change the seeded password and
tighten that policy back up** before this is anything but a local dev login;
Identity's lockout-after-failed-attempts is on by default, which helps, but a
4-character password is still a 4-character password.

To change the seeded credentials, edit the constants at the top of
`FreeTrack.Infrastructure/Persistence/Seed/IdentitySeeder.cs`
(`SeedAdminUsername` / `SeedAdminPassword`) — they only take effect for a
*new* database; if `admin` already exists, use the admin dashboard (once you
build a "change password" flow) or `UserManager.RemovePasswordAsync` +
`AddPasswordAsync` directly.

 (`Smtp:Host` empty). Until you set
  real SMTP credentials, notifications only show up in `/Admin` — nothing is
  silently lost, but nobody gets emailed either.
- **No migration has been generated yet** for `Booking` / `AdminNotification`
  — see the README's "Database" section; run
  `dotnet ef migrations add AddBookings -p src/FreeTrack.Infrastructure -s src/FreeTrack.Web`
  before running the app.

## Extending it later
- Multiple admins / roles → create more `IdentityUser`s and assign the
  `Admin` role via `RoleManager`/`UserManager` (e.g. an "invite admin" flow) —
  `[Authorize(Roles = "Admin")]` already covers any number of them.
- Real-time slot locking (so two people can't both get approved for
  overlapping dates) → the conflict check already exists in
  `BookingService.RequestBookingAsync` and `ApproveAsync`; you'd add a second
  overlap check inside `ApproveAsync` that rejects the approval (with a
  clear error) if another `Approved` booking already claimed the slot
  between request and decision.
- Hourly/time-of-day granularity instead of whole days → `Booking.StartUtc`/
  `EndUtc` are already full `DateTime`s, so this is a calendar-UI change
  (finer grid), not a data-model change.
