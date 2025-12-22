using System.Text.Json;
using Xunit;
using Lab01;

namespace Lab01.Tests
{
    public class PersonSerializerTests : IDisposable
    {
        private readonly PersonSerializer _serializer;
        private readonly string _testFilePath;
        private readonly string _testListFilePath;
        private readonly string _asyncTestFilePath;

        public PersonSerializerTests()
        {
            _serializer = new PersonSerializer();
            _testFilePath = Path.GetTempFileName();
            _testListFilePath = Path.GetTempFileName();
            _asyncTestFilePath = Path.GetTempFileName();
        }

        [Fact]
        public void SerializeToJson_ShouldReturnValidJson()
        {
            var person = new Person
            {
                FirstName = "Egor",
                LastName = "Dyachenko",
                Age = 30,
                Email = "tvoy_sosed2008@mail.ru",
                Id = "1",
                PhoneNumber = "89604630622"
            };

            var json = _serializer.SerializeToJson(person);

            Assert.NotNull(json);
            Assert.NotEmpty(json);
            Assert.Contains("Egor", json);
            Assert.Contains("Dyachenko", json);
            Assert.Contains("tvoy_sosed2008@mail.ru", json);
            Assert.Contains("personId", json);
            Assert.Contains("phone", json);

            using var document = JsonDocument.Parse(json);
            Assert.True(document.RootElement.TryGetProperty("firstName", out _));
            Assert.True(document.RootElement.TryGetProperty("lastName", out _));
            Assert.True(document.RootElement.TryGetProperty("age", out _));
            Assert.True(document.RootElement.TryGetProperty("email", out _));
        }

        [Fact]
        public void DeserializeFromJson_ShouldCreateValidPerson()
        {
            var json = @"{
                ""firstName"": ""Sklyar"",
                ""lastName"": ""Ilya"",
                ""age"": 19,
                ""email"": ""skII@gmail.com"",
                ""personId"": ""2"",
                ""phone"": ""89138471983"",
                ""birthDate"": ""2006-11-17T00:00:00""
            }";

            var person = _serializer.DeserializeFromJson(json);

            Assert.Equal("Sklyar", person.FirstName);
            Assert.Equal("Ilya", person.LastName);
            Assert.Equal(19, person.Age);
            Assert.Equal("skII@gmail.com", person.Email);
            Assert.Equal("2", person.Id);
            Assert.Equal("89138471983", person.PhoneNumber);
            Assert.Equal(new DateTime(2006, 11, 17), person.BirthDate);
        }

        [Fact]
        public void SaveToFile_And_LoadFromFile_ShouldWorkCorrectly()
        {
            var originalPerson = new Person
            {
                FirstName = "Egor",
                LastName = "Dyachenko",
                Age = 30,
                Email = "tvoy_sosed2008@mail.ru",
                Id = "1",
                PhoneNumber = "89604630622"
            };

            _serializer.SaveToFile(originalPerson, _testFilePath);
            var fileExists = File.Exists(_testFilePath);
            var loadedPerson = _serializer.LoadFromFile(_testFilePath);

            Assert.True(fileExists);
            Assert.NotNull(loadedPerson);
            Assert.Equal(originalPerson.FirstName, loadedPerson.FirstName);
            Assert.Equal(originalPerson.LastName, loadedPerson.LastName);
            Assert.Equal(originalPerson.Age, loadedPerson.Age);
            Assert.Equal(originalPerson.Email, loadedPerson.Email);
            Assert.Equal(originalPerson.Id, loadedPerson.Id);
            Assert.Equal(originalPerson.PhoneNumber, loadedPerson.PhoneNumber);
        }

        [Fact]
        public async Task SaveToFileAsync_And_LoadFromFileAsync_ShouldWorkCorrectly()
        {
            var originalPerson = new Person
            {
                FirstName = "Egor",
                LastName = "Dyachenko",
                Age = 30,
                Email = "tvoy_sosed2008@mail.ru",
                Id = "1",
                PhoneNumber = "89604630622"
            };

            await _serializer.SaveToFileAsync(originalPerson, _asyncTestFilePath);
            var loadedPerson = await _serializer.LoadFromFileAsync(_asyncTestFilePath);

            Assert.NotNull(loadedPerson);
            Assert.Equal(originalPerson.FirstName, loadedPerson.FirstName);
            Assert.Equal(originalPerson.LastName, loadedPerson.LastName);
            Assert.Equal(originalPerson.Age, loadedPerson.Age);
            Assert.Equal(originalPerson.Email, loadedPerson.Email);
        }

        [Fact]
        public void SaveListToFile_And_LoadListFromFile_ShouldWorkCorrectly()
        {
            var people = new List<Person>
            {
                new() { FirstName = "Egor", LastName = "Dyachenko", Age = 17, Email = "egor@mail.ru" },
                new() { FirstName = "Ilya", LastName = "Sklyar", Age = 19, Email = "ilya@gmail.com" },
                new() { FirstName = "Artem", LastName = "Svistunov", Age = 19, Email = "artem@gmail.com" }
            };

            _serializer.SaveListToFile(people, _testListFilePath);
            var loadedPeople = _serializer.LoadListFromFile(_testListFilePath);

            Assert.NotNull(loadedPeople);
            Assert.Equal(3, loadedPeople.Count);

            Assert.Equal("Egor", loadedPeople[0].FirstName);
            Assert.Equal("Dyachenko", loadedPeople[0].LastName);

            Assert.Equal("Ilya", loadedPeople[1].FirstName);
            Assert.Equal("Sklyar", loadedPeople[1].LastName);

            Assert.Equal("Artem", loadedPeople[2].FirstName);
            Assert.Equal("Svistunov", loadedPeople[2].LastName);
        }

        [Fact]
        public void LoadFromFile_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            var nonExistentFile = "kakoyto_file.json";

            var exception = Assert.Throws<FileNotFoundException>(() =>
                _serializer.LoadFromFile(nonExistentFile));

            Assert.Contains("не найден", exception.Message);
        }

        [Fact]
        public void SerializeToJson_ShouldThrowArgumentNullException_WhenPersonIsNull()
        {
            Person? person = null;

            Assert.Throws<ArgumentNullException>(() =>
                _serializer.SerializeToJson(person!));
        }

        [Fact]
        public void SaveToFile_ShouldThrowArgumentNullException_WhenPersonIsNull()
        {

            Person? person = null;

            Assert.Throws<ArgumentNullException>(() =>
                _serializer.SaveToFile(person!, _testFilePath));
        }

        [Fact]
        public void SaveToFile_ShouldThrowArgumentException_WhenFilePathIsEmpty()
        {

            var person = new Person { Email = "egor@gmail.com" };

            Assert.Throws<ArgumentException>(() =>
                _serializer.SaveToFile(person, ""));
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);

            if (File.Exists(_testListFilePath))
                File.Delete(_testListFilePath);

            if (File.Exists(_asyncTestFilePath))
                File.Delete(_asyncTestFilePath);

            if (File.Exists("error.txt"))
                File.Delete("error.txt");
        }
    }
}