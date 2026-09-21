using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FreeTrack.Infrastructure.Persistence;

// Npgsql refuses DateTime with Kind=Unspecified (what date-picker form input produces)
// for timestamptz columns. Every DateTime in this model is UTC, so label them as such both ways.
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)) { }
}

public class NullableUtcDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter() : base(
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v) { }
}
