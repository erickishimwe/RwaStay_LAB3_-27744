using lab3Rwastay.Models;
using MySqlConnector;

namespace lab3Rwastay.Data;

public sealed class HostRepository
{
    public List<Host> GetAll(string search = "")
    {
        const string sql = """
            SELECT id, name, phone, email, city
            FROM hosts
            WHERE @search = '' OR name LIKE @pattern OR phone LIKE @pattern OR email LIKE @pattern OR city LIKE @pattern
            ORDER BY name;
            """;
        var result = new List<Host>();
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@search", search.Trim());
        command.Parameters.AddWithValue("@pattern", $"%{search.Trim()}%");
        using var reader = command.ExecuteReader();
        while (reader.Read()) result.Add(Map(reader));
        return result;
    }

    public int Insert(Host host)
    {
        const string sql = "INSERT INTO hosts (name, phone, email, city) VALUES (@name, @phone, @email, @city); SELECT LAST_INSERT_ID();";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, host);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Host host)
    {
        const string sql = "UPDATE hosts SET name=@name, phone=@phone, email=@email, city=@city WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, host);
        command.Parameters.AddWithValue("@id", host.Id);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM hosts WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(MySqlCommand command, Host host)
    {
        command.Parameters.AddWithValue("@name", host.Name);
        command.Parameters.AddWithValue("@phone", host.Phone);
        command.Parameters.AddWithValue("@email", host.Email);
        command.Parameters.AddWithValue("@city", host.City);
    }

    private static Host Map(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"), Name = reader.GetString("name"), Phone = reader.GetString("phone"),
        Email = reader.GetString("email"), City = reader.GetString("city")
    };
}
