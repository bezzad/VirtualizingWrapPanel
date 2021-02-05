namespace VirtualizingWrapPanel.Sample.Models
{
    public interface IAuthor : IEntity<long>
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        int Type { get; set; }
    }
}
