using System;
using Bogus;
using Newtonsoft.Json;

namespace VirtualizingWrapPanel.Sample.Models
{
    public class BookFileModel : IBookFile
    {
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("size")] public long FileSize { get; set; }

        [JsonIgnore]
        public BookFileType FileType
        {
            get => (BookFileType)Type;
            set => Type = (int)value;
        }
        [JsonProperty("type")] public int Type { get; set; }
        public string Key { get; set; } // cipher key
        public string Path { get; set; } // path of file
        public string Title { get; set; }
        public int? Duration { get; set; }
        public int SequenceNo { get; set; } // audio track number
        public DateTimeOffset? StatusModifiedDate { get; set; }
        public DateTimeOffset? CreationDate { get; set; }

        public static Faker<BookFileModel> FakeData { get; } =
            new Faker<BookFileModel>()
                .RuleFor(p => p.Id, f => f.UniqueIndex)
                .RuleFor(p => p.Key, f => f.Random.String(10, 10))
                .RuleFor(p => p.Path, f => f.System.FilePath())
                .RuleFor(p => p.Duration, f => f.Random.Int(0, 1000))
                .RuleFor(p => p.FileSize, f => f.UniqueIndex)
                .RuleFor(p => p.FileType, f => f.PickRandom<BookFileType>())
                .RuleFor(p => p.Title, f => f.Name.JobTitle());
    }
}
