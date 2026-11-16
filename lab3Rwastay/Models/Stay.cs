namespace lab3Rwastay.Models;

public sealed class Stay
{
    public int Id { get; set; }
    public int HostId { get; set; }
    public string HostName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }

    public override string ToString() => $"{Title} - {City} ({PricePerNight:N0} RWF/night)";
}
