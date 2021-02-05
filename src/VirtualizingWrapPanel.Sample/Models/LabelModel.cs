using Bogus;
using Newtonsoft.Json;

namespace VirtualizingWrapPanel.Sample.Models
{
    public class LabelModel : ILabel
    {
        [JsonProperty(PropertyName = "tagID")]
        public long Id { get; set; }
        public string Tag { get; set; }

        public static Faker<LabelModel> FakeData { get; } =
            new Faker<LabelModel>()
                .RuleFor(p => p.Id, f => f.UniqueIndex)
                .RuleFor(p => p.Tag, f => f.Name.JobTitle());
    }
}
