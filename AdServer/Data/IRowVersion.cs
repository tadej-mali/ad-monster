using System.ComponentModel.DataAnnotations;

namespace AdServer.Data
{
    /// <summary>
    /// Each entity that needs support for optimistic locking concurrency strategy, should implement this interface.
    /// See http://www.asp.net/mvc/tutorials/getting-started-with-ef-5-using-mvc-4/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application
    /// </summary>
    public interface IRowVersion
    {
        [Timestamp]
        byte[] VersionStamp { get; set; }
    }

    /// <summary>
    /// By convention all model entities will have a primary key named Id
    /// </summary>
    public interface IEntityWithId
    {
        [Key]
        int Id { get; set; }
    }
}
