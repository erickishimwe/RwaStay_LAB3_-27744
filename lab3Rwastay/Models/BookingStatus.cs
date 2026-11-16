namespace lab3Rwastay.Models;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Completed
}

public static class BookingStatusExtensions
{
    public static string ToDatabaseValue(this BookingStatus status) => status.ToString().ToUpperInvariant();

    public static BookingStatus FromDatabaseValue(string value) =>
        Enum.TryParse<BookingStatus>(value, true, out var status) ? status : BookingStatus.Pending;
}
