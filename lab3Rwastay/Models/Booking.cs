namespace lab3Rwastay.Models;

public sealed class Booking
{
    public int Id { get; set; }
    public int GuestId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public int StayId { get; set; }
    public string StayTitle { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }

    public int Nights => (CheckOut.Date - CheckIn.Date).Days;
}
