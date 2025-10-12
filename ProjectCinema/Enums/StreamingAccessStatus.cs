namespace ProjectCinema.Enums
{
    public enum StreamingAccessStatus
    {
        Pending,        // Streaming access created, waiting for payment
        Active,         // Payment successful, access granted
        Expired,        // Access expired (time reached)
        Cancelled,      // Access cancelled (by user or timeout)
        Failed          // Payment failed
    }
}