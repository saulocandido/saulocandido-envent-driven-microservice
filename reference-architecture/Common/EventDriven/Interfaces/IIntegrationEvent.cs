namespace Common.eventDriven.Interfaces
{
    public interface IIntegrationEvent
    {
        //
        // Summary:
        //     Unique event identifier.
        string Id { get; set; }

        //
        // Summary:
        //     Event creation date.
        DateTime CreationDate { get; set; }
    }
}
