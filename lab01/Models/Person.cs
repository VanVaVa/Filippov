using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Lab01
{
    public class Person
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int Age { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonPropertyName("personId")]
        public string Id { get; set; } = string.Empty;

        [JsonInclude]
        private DateTime _birthDate;

        [JsonIgnore]
        public DateTime BirthDate
        {
            get => _birthDate;
            set => _birthDate = value;
        }

        private string _email = string.Empty;

        public string Email
        {
            get => _email;
            set
            {
                if (Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) _email = value;
                else throw new ArgumentException("Email некорректный");
            }
        }

        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string FullName => FirstName + " " + LastName;

        public bool IsAdult => Age >= 18;
    }
}