using MySqlConnector;

namespace lab3Rwastay.Data;

public static class DbConnection
{
    private const string ConnectionString =
        "Server=localhost;Port=3306;Database=rwastaydb;User ID=root;Password=;SslMode=None;Allow User Variables=True;";

    public static MySqlConnection Create() => new(ConnectionString);

    public static async Task TestAsync()
    {
        await using var connection = Create();
        await connection.OpenAsync();
    }
}
