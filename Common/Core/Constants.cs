namespace AuctionPortal.Common.Core
{
    public class Constants
    {
        public const string JsonContentType = "application/json";
        public const string CommandType = "Command Type";
        public const string CommandText = "Command Text";
        public const string ExceptionData = "Exception Data: ";
        public const string ExceptionMessage = "Exception Message: ";
        public const string InnerExceptionMessage = "Inner Exception Message: ";
        public const string ExceptionStackTrace = "Exception StackTrace: ";
        public const string InnerExceptionStackTrace = "Inner Exception StackTrace: ";
        public const string AlphanumericCaps = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()_+";
        public const int DefaultPasswordLength = 20;

        public const char GeneralDelimiter = ',';
        public const char ParentDelimiter = ';';
        public const int DefaultUserId = 1;
    }
}
