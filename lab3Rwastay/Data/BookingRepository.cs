using lab3Rwastay.Models;
using MySqlConnector;

namespace lab3Rwastay.Data;

public sealed class BookingRepository
{
    public List<Booking> GetAll(int? hostId = null, int? guestId = null)
    {
        const string sql = """
            SELECT b.id, b.guest_id, g.full_name AS guest_name, b.stay_id, s.title AS stay_title,
                   b.check_in, b.check_out, b.total_price, b.status
            FROM bookings b
            INNER JOIN guests g ON g.id = b.guest_id
            INNER JOIN stays s ON s.id = b.stay_id
            WHERE (@host_id IS NULL OR s.host_id = @host_id)
              AND (@guest_id IS NULL OR b.guest_id = @guest_id)
            ORDER BY b.check_in DESC, b.id DESC;
            """;
        var result = new List<Booking>();
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@host_id", hostId.HasValue ? hostId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@guest_id", guestId.HasValue ? guestId.Value : DBNull.Value);
        using var reader = command.ExecuteReader();
        while (reader.Read()) result.Add(Map(reader));
        return result;
    }

    public bool HasOverlap(int stayId, DateTime checkIn, DateTime checkOut, int ignoredBookingId = 0)
    {
        const string sql = "SELECT COUNT(*) FROM bookings WHERE stay_id=@stay_id AND id<>@ignored_id AND status IN ('PENDING','CONFIRMED') AND check_in < @check_out AND check_out > @check_in;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@stay_id", stayId);
        command.Parameters.AddWithValue("@ignored_id", ignoredBookingId);
        command.Parameters.AddWithValue("@check_in", checkIn.Date);
        command.Parameters.AddWithValue("@check_out", checkOut.Date);
        return Convert.ToInt32(command.ExecuteScalar()) > 0;
    }

    public int Insert(Booking booking)
    {
        const string sql = "INSERT INTO bookings (guest_id, stay_id, check_in, check_out, total_price, status) VALUES (@guest_id, @stay_id, @check_in, @check_out, @total, @status); SELECT LAST_INSERT_ID();";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        AddParameters(command, booking);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void UpdateStatus(int bookingId, BookingStatus status)
    {
        const string sql = "UPDATE bookings SET status=@status WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@status", status.ToDatabaseValue());
        command.Parameters.AddWithValue("@id", bookingId);
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM bookings WHERE id=@id;";
        using var connection = DbConnection.Create();
        connection.Open();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    private static void AddParameters(MySqlCommand command, Booking booking)
    {
        command.Parameters.AddWithValue("@guest_id", booking.GuestId);
        command.Parameters.AddWithValue("@stay_id", booking.StayId);
        command.Parameters.AddWithValue("@check_in", booking.CheckIn.Date);
        command.Parameters.AddWithValue("@check_out", booking.CheckOut.Date);
        command.Parameters.AddWithValue("@total", booking.TotalPrice);
        command.Parameters.AddWithValue("@status", booking.Status.ToDatabaseValue());
    }

    private static Booking Map(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"), GuestId = reader.GetInt32("guest_id"), GuestName = reader.GetString("guest_name"),
        StayId = reader.GetInt32("stay_id"), StayTitle = reader.GetString("stay_title"), CheckIn = reader.GetDateTime("check_in"),
        CheckOut = reader.GetDateTime("check_out"), TotalPrice = reader.GetDecimal("total_price"),
        Status = BookingStatusExtensions.FromDatabaseValue(reader.GetString("status"))
    };
}
