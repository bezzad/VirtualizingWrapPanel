using Bogus;

namespace VirtualizingWrapPanel.Sample.Models
{
    public class CategoryModel : ICategory
    {
        public long Id { get; set; }
        public long Parent { get; set; }
        public string Title { get; set; }

        public static Faker<CategoryModel> FakeData { get; } =
            new Faker<CategoryModel>()
                .RuleFor(p => p.Id, f => f.UniqueIndex)
                .RuleFor(p => p.Parent, f => f.UniqueIndex)
                .RuleFor(p => p.Title, f => f.Name.JobTitle());
    }
}
