using lab3Rwastay.Data;
using lab3Rwastay.Models;

namespace lab3Rwastay.Controllers;

public sealed class BookingController
{
    private readonly BookingRepository repository;
    private readonly GuestRepository guestRepository;
    private readonly StayRepository stayRepository;
    public BookingController(BookingRepository repository, GuestRepository guestRepository, StayRepository stayRepository)
    { this.repository = repository; this.guestRepository = guestRepository; this.stayRepository = stayRepository; }

    public List<Booking> GetAll() => repository.GetAll();
    public List<Guest> GetGuests() => guestRepository.GetAll();
    public List<Stay> GetAvailableStays() => stayRepository.GetAvailable();
    public decimal CalculateTotal(Stay stay, DateTime checkIn, DateTime checkOut) => stay.PricePerNight * (checkOut.Date - checkIn.Date).Days;

    public void Create(Booking booking, decimal pricePerNight)
    {
        if (booking.GuestId <= 0) throw new ArgumentException("Select a guest.");
        if (booking.StayId <= 0) throw new ArgumentException("Select a stay.");
        if (booking.CheckOut.Date <= booking.CheckIn.Date) throw new ArgumentException("Check-out must be after check-in.");
        if (repository.HasOverlap(booking.StayId, booking.CheckIn, booking.CheckOut)) throw new ArgumentException("This stay is already booked for some of those dates.");
        booking.TotalPrice = pricePerNight * booking.Nights;
        booking.Status = BookingStatus.Pending;
        repository.Insert(booking);
    }

    public void ChangeStatus(int bookingId, BookingStatus status)
    {
        if (bookingId <= 0) throw new ArgumentException("Select a booking first.");
        repository.UpdateStatus(bookingId, status);
    }

    public void Cancel(int bookingId) => ChangeStatus(bookingId, BookingStatus.Cancelled);
    public void Delete(int bookingId) { if (bookingId <= 0) throw new ArgumentException("Select a booking first."); repository.Delete(bookingId); }
}
