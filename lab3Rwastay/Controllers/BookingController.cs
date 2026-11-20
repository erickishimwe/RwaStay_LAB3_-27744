using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Controllers;

public sealed class BookingController
{
    private readonly BookingRepository repository;
    private readonly GuestRepository guestRepository;
    private readonly StayRepository stayRepository;
    private readonly User currentUser;
    public BookingController(BookingRepository repository, GuestRepository guestRepository, StayRepository stayRepository, User currentUser)
    { this.repository = repository; this.guestRepository = guestRepository; this.stayRepository = stayRepository; this.currentUser = currentUser; }

    public List<Booking> GetAll() => currentUser.Role switch
    {
        UserRole.Manager when currentUser.HostId.HasValue => repository.GetAll(hostId: currentUser.HostId.Value),
        UserRole.Client when currentUser.GuestId.HasValue => repository.GetAll(guestId: currentUser.GuestId.Value),
        UserRole.Admin => repository.GetAll(),
        _ => []
    };
    public List<Guest> GetGuests() => currentUser.Role == UserRole.Client && currentUser.GuestId.HasValue ? guestRepository.GetAll().Where(x => x.Id == currentUser.GuestId.Value).ToList() : guestRepository.GetAll();
    public List<Stay> GetAvailableStays() => currentUser.Role == UserRole.Manager && currentUser.HostId.HasValue ? stayRepository.GetAvailable().Where(x => x.HostId == currentUser.HostId.Value).ToList() : stayRepository.GetAvailable();
    public decimal CalculateTotal(Stay stay, DateTime checkIn, DateTime checkOut) => stay.PricePerNight * (checkOut.Date - checkIn.Date).Days;

    public void Create(Booking booking, decimal pricePerNight)
    {
        if (booking.GuestId <= 0) throw new ArgumentException("Select a guest.");
        if (booking.StayId <= 0) throw new ArgumentException("Select a stay.");
        if (booking.CheckOut.Date <= booking.CheckIn.Date) throw new ArgumentException("Check-out must be after check-in.");
        if (currentUser.Role == UserRole.Client && booking.GuestId != currentUser.GuestId) throw new UnauthorizedAccessException("Clients can only create bookings for their own account.");
        if (currentUser.Role == UserRole.Manager && !GetAvailableStays().Any(x => x.Id == booking.StayId)) throw new UnauthorizedAccessException("Managers can only access their own stays.");
        if (repository.HasOverlap(booking.StayId, booking.CheckIn, booking.CheckOut)) throw new ArgumentException("This stay is already booked for some of those dates.");
        booking.TotalPrice = pricePerNight * booking.Nights;
        booking.Status = BookingStatus.Pending;
        repository.Insert(booking);
    }

    public void ChangeStatus(int bookingId, BookingStatus status)
    {
        if (currentUser.Role != UserRole.Admin) throw new UnauthorizedAccessException("Only administrators can change booking status.");
        if (bookingId <= 0) throw new ArgumentException("Select a booking first.");
        repository.UpdateStatus(bookingId, status);
    }

    public void Cancel(int bookingId)
    {
        if (currentUser.Role is not (UserRole.Admin or UserRole.Client)) throw new UnauthorizedAccessException("Only administrators or the booking client can cancel a booking.");
        if (bookingId <= 0) throw new ArgumentException("Select a booking first.");
        repository.UpdateStatus(bookingId, BookingStatus.Cancelled);
    }
    public void Delete(int bookingId) { if (currentUser.Role != UserRole.Admin) throw new UnauthorizedAccessException("Only administrators can delete bookings."); if (bookingId <= 0) throw new ArgumentException("Select a booking first."); repository.Delete(bookingId); }
}
