namespace FlightBooking.Service.Data
{
    public class Constants
    {
    }

    internal static class RepositoryConstants
    {
        public const string LoggingStarted = "Started logging";
        public const string CreateNullError = "Attempt to insert empty entity. Type of Entity : {0}";
        public const string DeleteNullError = "Could not find entity for deleting. type of Entity : {0}";
        public const string BulkDeleteNullError = "Attempt to Delete empty list of entities. Type of Entity : {0}";
        public const string BulkCreateNullError = "Attempt to insert empty list of entities. Type of Entity : {0}";
        public const string EmptySaveInfo = "No changes was written to underlying database.";
        public const string UpdateException = "Update Exception";
        public const string UpdateConcurrencyException = "Update Concurrency Exception";
        public const string SaveChangesException = "Generic Error in Generic Repo Update method";
    }

    internal static class ServiceErrorMessages
    {
        public const string Success = "The operation was successful";
        public const string Failed = "An unhandled errror has occured while processing your request";
        public const string UpdateError = "There was an error carrying out operation";
        public const string MisMatch = "The entity Id does not match the supplied Id";
        public const string EntityIsNull = "Supplied entity is null or supplied list of entities is empty. Check our docs";
        public const string EntityNotFound = "The requested resource was not found. Verify that the supplied Id is correct";
        public const string Incompleted = "Some actions may not have been successfully processed";
        public const string EntityExist = "An entity of the same name or id exists";
        public const string InvalidParam = "A supplied parameter or model is invalid. Check the docs";
        public const string UnprocessableEntity = "The action cannot be processed";
        public const string InternalServerError = "An internal server error and request could not processed";
        public const string OperationFailed = "Operation failed";

        public const string ParameterEmptyOrNull = "The parameter list is null or empty";
        public const string RequestIdRequired = "Request Id is required";
    }
}