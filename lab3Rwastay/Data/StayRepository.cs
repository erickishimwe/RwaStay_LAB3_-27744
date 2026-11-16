using lab3Rwastay.Models;
using MySqlConnector;

namespace lab3Rwastay.Data;

public sealed class StayRepository
{
    public List<Stay> GetAll(string search = "")
    {
        const string sql = """
            SELECT s.id, s.host_id, h.name AS host_name, s.title, s.description, s.city,
                   s.price_per_night, s.capacity, s.is_available
            FROM stays s INNER JOIN hosts h ON h.id = s.host_id
            WHERE @search = '' OR s.title LIKE @pattern OR s.city LIKE @pattern OR h.name LIKE @pattern
            ORDER BY s.title;
            """;
        var result = new List<Stay>();
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@search", search.Trim());
        command.Parameters.AddWithValue("@pattern", $"%{search.Trim()}%");
        using var reader = command.ExecuteReader();
        while (reader.Read()) result.Add(Map(reader));
        return result;
    }

    public List<Stay> GetAvailable() => GetAll().Where(x => x.IsAvailable).ToList();

    public int Insert(Stay stay)
    {
        const string sql = "INSERT INTO stays (host_id, title, description, city, price_per_night, capacity, is_available) VALUES (@host_id, @title, @description, @city, @price, @capacity, @available); SELECT LAST_INSERT_ID();";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, stay);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Stay stay)
    {
        const string sql = "UPDATE stays SET host_id=@host_id, title=@title, description=@description, city=@city, price_per_night=@price, capacity=@capacity, is_available=@available WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, stay);
        command.Parameters.AddWithValue("@id", stay.Id);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM stays WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(MySqlCommand command, Stay stay)
    {
        command.Parameters.AddWithValue("@host_id", stay.HostId);
        command.Parameters.AddWithValue("@title", stay.Title);
        command.Parameters.AddWithValue("@description", stay.Description);
        command.Parameters.AddWithValue("@city", stay.City);
        command.Parameters.AddWithValue("@price", stay.PricePerNight);
        command.Parameters.AddWithValue("@capacity", stay.Capacity);
        command.Parameters.AddWithValue("@available", stay.IsAvailable);
    }

    private static Stay Map(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"), HostId = reader.GetInt32("host_id"), HostName = reader.GetString("host_name"),
        Title = reader.GetString("title"), Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString("description"),
        City = reader.GetString("city"), PricePerNight = reader.GetDecimal("price_per_night"), Capacity = reader.GetInt32("capacity"),
        IsAvailable = reader.GetBoolean("is_available")
    };
}
