using System;
using Bogus;

namespace WPFSortFilter
{
    public class Model
    {
        /// <summary>  Setting this as a starting value for id</summary>
        private static int _userId = 0;
        public int Id { get; set; }
        public int Number { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime DOB { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string SSN { get; set; }
        public string Suffix { get; set; }
        public string Phone { get; set; }
        public string CoverUri { get; set; }
        public DateTime CurrentDateTime => DateTime.Now;

        // <summary>Gets the fake data.</summary>
        /// <value>The fake data.</value>
        public static Faker<Model> FakeData { get; } =
            new Faker<Model>()
                .RuleFor(p => p.Id, f => _userId++)
                .RuleFor(p => p.Number, f => f.IndexFaker)
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.MiddleName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Title, f => f.Random.Words(3))
                .RuleFor(p => p.Suffix, f => f.Name.Suffix())
                .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
                .RuleFor(p => p.DOB, f => f.Date.Past(18))
                .RuleFor(p => p.Gender, f => f.PickRandom<Gender>())
                .RuleFor(p => p.SSN, f => f.Random.Replace("###-##-####"))
                .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("(###)-###-####"))
                .RuleFor(p => p.CoverUri, f => f.Image.LoremFlickrUrl(150, 200));
    }

    public enum Gender
    {
        Male,
        Female
    }
}
