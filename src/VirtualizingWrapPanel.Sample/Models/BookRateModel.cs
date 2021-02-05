using Bogus;

namespace VirtualizingWrapPanel.Sample.Models
{
    public class BookRateModel : IBookRate
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public int Count { get; set; }

        public static Faker<BookRateModel> FakeData { get; } =
            new Faker<BookRateModel>()
                .RuleFor(p => p.Id, f => f.UniqueIndex)
                .RuleFor(p => p.Value, f => f.Random.Double(0, 5))
                .RuleFor(p => p.Count, 5);
    }
}
