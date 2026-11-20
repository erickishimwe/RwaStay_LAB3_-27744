using lab3Rwastay.Models;
using MySqlConnector;

namespace lab3Rwastay.Data;

public sealed class UserRepository
{
    public User? FindByUsername(string username)
    {
        const string sql = "SELECT u.id, u.username, u.password_hash, u.role, u.host_id, u.guest_id, COALESCE(h.name, g.full_name, u.username) AS display_name FROM users u LEFT JOIN hosts h ON h.id=u.host_id LEFT JOIN guests g ON g.id=u.guest_id WHERE u.username=@username LIMIT 1;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username.Trim());
        using var reader = command.ExecuteReader();
        return reader.Read() ? Map(reader) : null;
    }

    public void CreateClient(string username, string password, string fullName, string email, string phone)
    {
        using var connection = DbConnection.Create();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            using var guest = new MySqlCommand("INSERT INTO guests (full_name, email, phone) VALUES (@name, @email, @phone); SELECT LAST_INSERT_ID();", connection, transaction);
            guest.Parameters.AddWithValue("@name", fullName); guest.Parameters.AddWithValue("@email", email); guest.Parameters.AddWithValue("@phone", phone);
            var guestId = Convert.ToInt32(guest.ExecuteScalar());
            using var user = new MySqlCommand("INSERT INTO users (username, password_hash, role, guest_id) VALUES (@username, @hash, 'CLIENT', @guest_id);", connection, transaction);
            user.Parameters.AddWithValue("@username", username.Trim()); user.Parameters.AddWithValue("@hash", PasswordHasher.Hash(password)); user.Parameters.AddWithValue("@guest_id", guestId);
            user.ExecuteNonQuery(); transaction.Commit();
        }
        catch { transaction.Rollback(); throw; }
    }

    private static User Map(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"), Username = reader.GetString("username"), PasswordHash = reader.GetString("password_hash"),
        Role = Enum.TryParse<UserRole>(reader.GetString("role"), true, out var role) ? role : UserRole.Client,
        HostId = reader.IsDBNull(reader.GetOrdinal("host_id")) ? null : reader.GetInt32("host_id"),
        GuestId = reader.IsDBNull(reader.GetOrdinal("guest_id")) ? null : reader.GetInt32("guest_id"),
        DisplayName = reader.GetString("display_name")
    };
}
