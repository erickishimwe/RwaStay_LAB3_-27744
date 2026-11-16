using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Controllers;

public sealed class GuestController
{
    private readonly GuestRepository repository;
    public GuestController(GuestRepository repository) => this.repository = repository;
    public List<Guest> GetAll(string search = "") => repository.GetAll(search);
    public void Save(Guest guest)
    {
        HostController.Require(guest.FullName, "Full name"); HostController.Require(guest.Phone, "Phone");
        if (!HostController.IsValidEmail(guest.Email)) throw new ArgumentException("Enter a valid email address.");
        if (guest.Id == 0) repository.Insert(guest); else repository.Update(guest);
    }
    public void Delete(int id) { if (id <= 0) throw new ArgumentException("Select a guest first."); repository.Delete(id); }
}
