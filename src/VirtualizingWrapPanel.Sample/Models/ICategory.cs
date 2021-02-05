namespace VirtualizingWrapPanel.Sample.Models
{
    public interface ICategory : IEntity<long>
    {
        long Parent { get; set; }
        string Title { get; set; }
    }
}
