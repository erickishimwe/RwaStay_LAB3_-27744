using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Controllers;

public sealed class StayController
{
    private readonly StayRepository repository;
    private readonly HostRepository hostRepository;
    public StayController(StayRepository repository, HostRepository hostRepository) { this.repository = repository; this.hostRepository = hostRepository; }
    public List<Stay> GetAll(string search = "") => repository.GetAll(search);
    public List<Stay> GetAvailable() => repository.GetAvailable();
    public List<Host> GetHosts() => hostRepository.GetAll();
    public void Save(Stay stay)
    {
        HostController.Require(stay.Title, "Title"); HostController.Require(stay.City, "City");
        if (stay.HostId <= 0) throw new ArgumentException("Select a host.");
        if (stay.PricePerNight < 0) throw new ArgumentException("Price cannot be negative.");
        if (stay.Capacity <= 0) throw new ArgumentException("Capacity must be greater than zero.");
        if (stay.Id == 0) repository.Insert(stay); else repository.Update(stay);
    }
    public void Delete(int id) { if (id <= 0) throw new ArgumentException("Select a stay first."); repository.Delete(id); }
}
