using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Controllers;

public sealed class StayTypeController
{
    private readonly StayTypeRepository repository;
    public StayTypeController(StayTypeRepository repository) => this.repository = repository;
    public List<StayType> GetAll() => repository.GetAll();
}
