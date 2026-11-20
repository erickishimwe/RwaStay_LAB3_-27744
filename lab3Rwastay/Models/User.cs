namespace lab3Rwastay.Models;

public enum UserRole
{
    Admin,
    Manager,
    Client
}

public sealed class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int? HostId { get; set; }
    public int? GuestId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}
