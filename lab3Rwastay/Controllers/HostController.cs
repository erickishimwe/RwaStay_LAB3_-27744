using System.Net.Mail;
using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Controllers;

public sealed class HostController
{
    private readonly HostRepository repository;
    public HostController(HostRepository repository) => this.repository = repository;
    public List<Host> GetAll(string search = "") => repository.GetAll(search);
    public void Save(Host host)
    {
        Validate(host);
        if (host.Id == 0) repository.Insert(host); else repository.Update(host);
    }
    public void Delete(int id) { if (id <= 0) throw new ArgumentException("Select a host first."); repository.Delete(id); }
    private static void Validate(Host host)
    {
        Require(host.Name, "Host name"); Require(host.Phone, "Phone"); Require(host.City, "City");
        if (!IsValidEmail(host.Email)) throw new ArgumentException("Enter a valid email address.");
    }
    internal static bool IsValidEmail(string email)
    {
        try { return !string.IsNullOrWhiteSpace(email) && new MailAddress(email).Address == email.Trim(); }
        catch (FormatException) { return false; }
    }
    internal static void Require(string value, string field) { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{field} is required."); }
}
