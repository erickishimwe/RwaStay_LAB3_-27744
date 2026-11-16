using lab3Rwastay.Models;
using MySqlConnector;

namespace lab3Rwastay.Data;

public sealed class GuestRepository
{
    public List<Guest> GetAll(string search = "")
    {
        const string sql = "SELECT id, full_name, email, phone FROM guests WHERE @search='' OR full_name LIKE @pattern OR email LIKE @pattern OR phone LIKE @pattern ORDER BY full_name;";
        var result = new List<Guest>();
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@search", search.Trim());
        command.Parameters.AddWithValue("@pattern", $"%{search.Trim()}%");
        using var reader = command.ExecuteReader();
        while (reader.Read()) result.Add(new Guest { Id = reader.GetInt32("id"), FullName = reader.GetString("full_name"), Email = reader.GetString("email"), Phone = reader.GetString("phone") });
        return result;
    }

    public int Insert(Guest guest)
    {
        const string sql = "INSERT INTO guests (full_name, email, phone) VALUES (@name, @email, @phone); SELECT LAST_INSERT_ID();";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, guest);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Guest guest)
    {
        const string sql = "UPDATE guests SET full_name=@name, email=@email, phone=@phone WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, guest);
        command.Parameters.AddWithValue("@id", guest.Id);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM guests WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(MySqlCommand command, Guest guest)
    {
        command.Parameters.AddWithValue("@name", guest.FullName);
        command.Parameters.AddWithValue("@email", guest.Email);
        command.Parameters.AddWithValue("@phone", guest.Phone);
    }
}
