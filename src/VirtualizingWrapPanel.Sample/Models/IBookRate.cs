namespace VirtualizingWrapPanel.Sample.Models
{
    public interface IBookRate : IEntity<int>
    {
        double Value { get; set; }
        int Count { get; set; }
    }
}
