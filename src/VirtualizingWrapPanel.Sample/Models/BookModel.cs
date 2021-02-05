using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bogus;
using Newtonsoft.Json;

namespace VirtualizingWrapPanel.Sample.Models
{
    [DebuggerDisplay("{Type} Book({Id}): {Title}")]
    public class BookModel : Model
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public BookType BookType
        {
            get
            {
                if (Enum.TryParse(Type, true, out BookType type))
                    return type;

                Debugger.Break();
                return BookType.Text;
            }
            set => Type = value.ToString();
        }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Publisher { get; set; }
        public long PublisherId { get; set; }
        public long NumberOfPages { get; set; }
        public int BeforeOffPrice { get; set; }
        public string Sticker { get; set; }
        public bool HasSticker => !string.IsNullOrWhiteSpace(Sticker);
        public string OffText { get; set; }
        public string PriceColor { get; set; }
        public long Price { get; set; }
        public long PhysicalPrice { get; set; }
        public double CurrencyPrice { get; set; }
        public double CurrencyBeforeOffPrice { get; set; }
        public double Rating { get; set; }
        public string Isbn { get; set; }
        public string PublishDate { get; set; }
        public long CreationTime { get; set; }
        public long LastActivityTime { get; set; }
        public string CoverUri { get; set; }
        public string ShareUri { get; set; }
        public bool IsRtl { get; set; }
        public string WebsiteLink { get; set; }
        public bool IsActiveSubscription { get; set; }
        public bool AsSubscription { get; set; }
        public bool SubscriptionAvailable { get; set; }
        public string Path { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] public bool IsSaveComplete { get; set; }
        [System.Text.Json.Serialization.JsonIgnore] public string Key { get; set; }
        public double ReadPagesRatio { get; set; }
        public bool IsRead { get; set; }
        public string LastHighlightVersionNumber { get; set; }
        public string LastTopPosition { get; set; }
        public Position LastReadingPosition
        {
            get => string.IsNullOrWhiteSpace(LastTopPosition)
                ? new Position(0, 0, 0)
                : (Position)LastTopPosition;
            set => LastTopPosition = value.ToString();
        }

        public List<BookRateModel> Rates { get; set; }
        public List<AuthorModel> Authors { get; set; }
        public List<CategoryModel> Categories { get; set; }
        public List<LabelModel> Labels { get; set; }
        [JsonProperty("files")] public List<BookFileModel> BookFiles { get; set; }

        public string Author
        {
            get
            {
                var firstAuthors = Authors?.Take(2);
                var authorNames = "";
                if (firstAuthors != null)
                    foreach (var author in firstAuthors)
                    {
                        if (!string.IsNullOrWhiteSpace(authorNames))
                            authorNames += ", ";

                        authorNames += author.FullName;
                    }

                return authorNames;
            }
        }

        public void CalculateBookType()
        {
            if (BookType == BookType.Text)
            {
                BookType = BookFiles.Any(file => file.FileType == BookFileType.Epub)
                      ? BookType.Epub
                      : BookType.Pdf;
            }
        }

        // <summary>Gets the fake data.</summary>
        /// <value>The fake data.</value>
        public static Faker<BookModel> FakeData { get; } =
            new Faker<BookModel>()
                .RuleFor(p => p.Id, f => f.UniqueIndex)
                .RuleFor(p => p.PublisherId, f => f.IndexFaker)
                .RuleFor(p => p.AccountId, f => f.UniqueIndex)
                .RuleFor(p => p.NumberOfPages, f => f.Random.Int(0, 1000))
                .RuleFor(p => p.LastHighlightVersionNumber, f => f.Random.Int(0, 1000).ToString())
                .RuleFor(p => p.BeforeOffPrice, f => f.Random.Int(0, 1000000))
                .RuleFor(p => p.CurrencyBeforeOffPrice, f => f.Random.Int(0, 1000000))
                .RuleFor(p => p.PhysicalPrice, f => f.Random.Int(0, 1000000))
                .RuleFor(p => p.CurrencyPrice, f => f.Random.Int(0, 1000000))
                .RuleFor(p => p.Rating, f => f.Random.Int(0, 5))
                .RuleFor(p => p.Price, f => f.Random.Int(0, 1000000))
                .RuleFor(p => p.ReadPagesRatio, f => f.Random.Int(0, 1000000))
                .RuleFor(p => p.Sticker, f => f.Image.LoremFlickrUrl(150, 200))
                .RuleFor(p => p.OffText, f => f.Random.String(0, 5))
                .RuleFor(p => p.Title, f => f.Random.Words(3))
                .RuleFor(p => p.Publisher, f => f.Random.Words(1))
                .RuleFor(p => p.Description, f => f.Random.Words(30))
                .RuleFor(p => p.PriceColor, f => f.Commerce.Color())
                .RuleFor(p => p.Isbn, f => f.Commerce.Product())
                .RuleFor(p => p.PublishDate, f => f.Date.Locale)
                .RuleFor(p => p.CreationTime, f => f.Date.Timespan().Ticks)
                .RuleFor(p => p.LastActivityTime, f => f.Date.Timespan().Ticks)
                .RuleFor(p => p.CoverUri, f => f.Image.LoremFlickrUrl(150, 200))
                .RuleFor(p => p.Path, f => f.Image.LoremFlickrUrl(150, 200))
                .RuleFor(p => p.WebsiteLink, f => f.Image.LoremFlickrUrl(150, 200))
                .RuleFor(p => p.LastReadingPosition, f => new Position(f.IndexVariable, 10, 0))
                .RuleFor(p => p.ShareUri, f => f.Image.LoremFlickrUrl(150, 200))
                .RuleFor(p => p.Authors, f => AuthorModel.FakeData.Generate(3))
                .RuleFor(p => p.Categories, f => CategoryModel.FakeData.Generate(3))
                .RuleFor(p => p.Labels, f => LabelModel.FakeData.Generate(5))
                .RuleFor(p => p.BookFiles, f => BookFileModel.FakeData.Generate(5))
                .RuleFor(p => p.Rates, f => BookRateModel.FakeData.Generate(3));
    }
}
