using lab3Rwastay.Models;
using MySqlConnector;

namespace lab3Rwastay.Data;

public sealed class StayTypeRepository
{
    public List<StayType> GetAll()
    {
        const string sql = "SELECT id, name FROM stay_types ORDER BY name;";
        var result = new List<StayType>();
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read()) result.Add(new StayType { Id = reader.GetInt32("id"), Name = reader.GetString("name") });
        return result;
    }
}
