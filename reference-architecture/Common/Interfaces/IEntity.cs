namespace Common.Interfaces
{
    public interface IEntity
    {
        //
        // Summary:
        //     The ID of the Entity.
        Guid Id { get; set; }

        //
        // Summary:
        //     Represents a unique ID that must change atomically with each store of the entity
        //     to its underlying storage medium.
        string? ETag { get; set; }
    }
}
