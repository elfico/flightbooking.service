namespace FlightBooking.Service.Data
{
    public enum Gender
    {
        Female = 1,
        Male,
        Transgender,
        Fluid,
        PreferNotToSay
    }

    public enum InternalCode
    {
        ConcurrencyError = -2,
        UpdateError = -1,
        Failed,
        Success,
        EntityIsNull,
        EntityNotFound,
        Mismatch,
        InvalidParam,
        Incompleted,
        ListEmpty,
        EntityExist,
        Unprocessable,
        Unauthorized,
    }

    public enum SortOrder
    {
        ASC = 1,
        DESC = 2
    }

    public enum BookingStatus
    {
        Pending = 1,
        Confirmed,
        Paid
    }
}