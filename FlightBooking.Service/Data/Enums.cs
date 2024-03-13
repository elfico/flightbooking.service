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
        UpdateError = -1, //when DbUpdateException / UpdateConcurrencyException is caught
        Failed, //operation failed
        Success,
        EntityIsNull, //parameter is null
        EntityNotFound, //entity not found
        Mismatch, //for update when model Id != supplied Id
        InvalidParam, //invalid parmater supplied in model. usually when the parameter fails the business logic
        Incompleted, //some processing was done but some failed
        ListEmpty, //a supplied paramter list is empty
        EntityExist, //the entity that is being saved already exists
        Unprocessable, //request cannot be processed.
        Unauthorized,
        ExternalApiCallFailed //to better handle logic when external api call failed. should not be returned to controller
    }

    public enum SortOrder
    {
        ASC = 1,
        DESC = 2
    }

    public enum BookingStatus
    {
        Pending = 0,
        Paid,
        Confirmed
    }
}