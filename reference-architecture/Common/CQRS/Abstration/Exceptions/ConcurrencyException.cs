namespace Common.CQRS.Abstration.Exceptions
{
    // Ensure ConcurrencyException is defined
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(string message = "") : base(message) { }
    }
}
