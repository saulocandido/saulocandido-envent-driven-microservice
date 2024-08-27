namespace Common.CQRS.Abstration.command
{
    public enum CommandOutcome
    {
        Accepted,
        Rejected,
        InvalidCommand,
        NotHandled,
        NotFound,
        Conflict
    }
}
