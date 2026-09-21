using Npgsql;

namespace FreeTrack.Infrastructure.Persistence;

public static class PostgresConnectionString
{
    /// <summary>Accepts Npgsql key=value strings as-is and converts the postgresql:// URI Neon hands out.</summary>
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException(
                "ConnectionStrings:Default is not set. Provide the Neon (PostgreSQL) connection string via " +
                "'dotnet user-secrets' locally or the ConnectionStrings__Default environment variable in production.");

        if (!value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
            !value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            return value;

        var uri = new Uri(value);
        var credentials = uri.UserInfo.Split(':', 2);

        return new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(credentials[0]),
            Password = credentials.Length > 1 ? Uri.UnescapeDataString(credentials[1]) : null,
            SslMode = SslMode.Require
        }.ConnectionString;
    }
}
