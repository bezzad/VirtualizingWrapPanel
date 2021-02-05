namespace VirtualizingWrapPanel.Sample.Models
{
    public interface ILabel : IEntity<long>
    {
        string Tag { get; set; }
    }
}
