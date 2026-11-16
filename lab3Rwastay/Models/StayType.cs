namespace lab3Rwastay.Models;

public sealed class StayType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public override string ToString() => Name;
}
