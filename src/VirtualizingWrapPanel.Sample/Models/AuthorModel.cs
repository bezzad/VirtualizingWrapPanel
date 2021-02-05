using Bogus;
using Newtonsoft.Json;

namespace VirtualizingWrapPanel.Sample.Models
{
    public class AuthorModel : IAuthor
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonIgnore]
        public AuthorRole AuthorType
        {
            get => (AuthorRole)Type;
            set => Type = (int)value;
        }
        public string FullName => $"{FirstName} {LastName}";

        public static Faker<AuthorModel> FakeData { get; } =
            new Faker<AuthorModel>()
                .RuleFor(p => p.Id, f => f.UniqueIndex)
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.AuthorType, f => f.PickRandom<AuthorRole>());

    }
}
