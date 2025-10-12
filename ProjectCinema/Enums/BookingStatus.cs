namespace ProjectCinema.Enums
{
    public enum BookingStatus
    {
        Pending,        // Booking created, waiting for payment
        Confirmed,      // Payment successful, booking confirmed
        Cancelled,      // Booking cancelled (by user or timeout)
        Failed,         // Payment failed
        Expired         // Booking expired (timeout without payment)
    }
}
