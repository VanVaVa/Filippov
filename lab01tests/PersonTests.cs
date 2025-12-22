using Xunit;
using Lab01;

namespace Lab01.Tests
{
    public class PersonTests
    {
        [Fact]
        public void Person_Constructor_ShouldInitializeProperties()
        {
            var person = new Person();

            Assert.NotNull(person);
            Assert.Equal(string.Empty, person.FirstName);
            Assert.Equal(string.Empty, person.LastName);
            Assert.Equal(0, person.Age);
            Assert.Equal(null, person.Password);
            Assert.Equal(string.Empty, person.Id);
            Assert.Equal(string.Empty, person.Email);
            Assert.Equal(string.Empty, person.PhoneNumber);
        }

        [Fact]
        public void Person_Email_ShouldValidate()
        {
            var person = new Person();

            person.Email = "moemilo@yandex.ru";
            Assert.Equal("moemilo@yandex.ru", person.Email);

            var exception = Assert.Throws<ArgumentException>(() =>
                person.Email = "nepravilniy-email");
            Assert.Contains("Email некорректный", exception.Message);
        }

        [Theory]
        [InlineData("validemail@gmail.com", true)]
        [InlineData("name@domain.co.uk", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Person_EmailValidation_ShouldWorkCorrectly(string email, bool shouldBeValid)
        {
            var person = new Person();

            if (shouldBeValid)
            {
                person.Email = email;
                Assert.Equal(email, person.Email);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                var exception = Assert.Throws<ArgumentException>(() =>
                    person.Email = email);
                Assert.Contains("Email некорректный", exception.Message);
            }
        }

        [Fact]
        public void Person_FullName_ShouldConcatenateFirstAndLastName()
        {
            var person = new Person
            {
                FirstName = "Егор",
                LastName = "Дуаченко"
            };

            var fullName = person.FullName;

            Assert.Equal("Егор Дуаченко", fullName);
        }

        [Theory]
        [InlineData(18, true)]
        [InlineData(25, true)]
        [InlineData(17, false)]
        [InlineData(0, false)]
        [InlineData(-5, false)]
        public void Person_IsAdult_ShouldCalculateCorrectly(int age, bool expected)
        {
            var person = new Person { Age = age };

            var isAdult = person.IsAdult;

            Assert.Equal(expected, isAdult);
        }

        [Fact]
        public void Person_BirthDate_ShouldBeAccessible()
        {
            var person = new Person();
            var testDate = new DateTime(1990, 5, 15);

            person.BirthDate = testDate;

            Assert.Equal(testDate, person.BirthDate);
        }

        [Fact]
        public void Person_JsonIgnore_ShouldNotSerializePassword()
        {
            var person = new Person
            {
                FirstName = "Илья",
                LastName = "Скляр",
                Password = "testtest",
                Email = "skII@gmail.com"
            };

            var serializer = new PersonSerializer();

            var json = serializer.SerializeToJson(person);
            var deserialized = serializer.DeserializeFromJson(json);

            Assert.DoesNotContain("testtest", json);
            Assert.DoesNotContain("Password", json);
            Assert.Null(deserialized.Password);
        }

        [Fact]
        public void Person_JsonPropertyName_ShouldUseCustomNames()
        {
            var person = new Person
            {
                Id = "5242",
                PhoneNumber = "89282367395",
                Email = "artemsvs@gmail.com"
            };

            var serializer = new PersonSerializer();

            var json = serializer.SerializeToJson(person);

            Assert.Contains("personId", json);
            Assert.Contains("phone", json);
        }

        [Fact]
        public void Person_PrivateField_ShouldBeIncludedInJson()
        {
            var person = new Person
            {
                BirthDate = new DateTime(2007, 7, 30),
                Email = "alohamaan@gmail.com"
            };

            var serializer = new PersonSerializer();

            var json = serializer.SerializeToJson(person);
            var deserialized = serializer.DeserializeFromJson(json);

            Assert.Contains("_birthDate", json);
            Assert.Equal(person.BirthDate, deserialized.BirthDate);
        }
    }
}