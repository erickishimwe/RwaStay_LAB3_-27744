using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Controllers;

public sealed class AuthController
{
    private readonly UserRepository repository;
    public AuthController(UserRepository repository) => this.repository = repository;

    public User Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Username and password are required.");
        var user = repository.FindByUsername(username);
        if (user is null || !PasswordHasher.Verify(password, user.PasswordHash)) throw new ArgumentException("Invalid username or password.");
        return user;
    }

    public void RegisterClient(string username, string password, string fullName, string email, string phone)
    {
        HostController.Require(username, "Username"); HostController.Require(password, "Password"); HostController.Require(fullName, "Full name"); HostController.Require(phone, "Phone");
        if (password.Length < 6) throw new ArgumentException("Password must contain at least 6 characters.");
        if (!HostController.IsValidEmail(email)) throw new ArgumentException("Enter a valid email address.");
        repository.CreateClient(username, password, fullName, email, phone);
    }
}
