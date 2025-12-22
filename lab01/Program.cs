namespace Lab01;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var person = new Person
            {
                FirstName = "Дьяченко",
                LastName = "Егор",
                Age = 17,
                Password = "qweqwe123",
                Id = "1",
                BirthDate = new DateTime(2008, 1, 9),
                Email = "tvoy.sosed2008@gmail.com",
                PhoneNumber = "+7(918) 555-35-35"
            };

            Console.WriteLine($"Полное имя: {person.FullName}");
            Console.WriteLine($"Совершеннолетний: {person.IsAdult}");
            Console.WriteLine($"Email: {person.Email}\n");

            var serializer = new PersonSerializer();

            string json = serializer.SerializeToJson(person);
            Console.WriteLine(json + "\n");

            Person deserializedPerson = serializer.DeserializeFromJson(json);
            Console.WriteLine($"Десериализованный объект: {deserializedPerson.FullName}\n");

            string filePath = "person.json";

            serializer.SaveToFile(person, filePath);
            Console.WriteLine($"Файл сохранен: {filePath}");

            Person loadedPerson = serializer.LoadFromFile(filePath);
            Console.WriteLine($"Загружен из файла: {loadedPerson.FullName}\n");

            await serializer.SaveToFileAsync(person, "person_async.json");
            Person asyncLoadedPerson = await serializer.LoadFromFileAsync("person_async.json");
            Console.WriteLine($"Асинхронно загружен: {asyncLoadedPerson.FullName}\n");

            var people = new List<Person>
                {
                    person,
                    new Person { FirstName = "Илья", LastName = "Скляр", Age = 19, Email = "skII@gmail.com" },
                    new Person { FirstName = "Артем", LastName = "Свистунов", Age = 19, Email = "artem2006alex@gmail.com" }
                };

            serializer.SaveListToFile(people, "people.json");
            var loadedPeople = serializer.LoadListFromFile("people.json");
            foreach (Person oneLoadedPerson in loadedPeople)
            {
                Console.WriteLine(serializer.SerializeToJson(oneLoadedPerson));
            }

            using (var fileManager = new FileResourceManager("test.txt", FileMode.Create))
            {
                fileManager.OpenForWriting();
                fileManager.WriteLine("Какой-то текст");
                fileManager.WriteLine("Люблю C#");
                fileManager.AppendText("Очень люблю C#");

                fileManager.OpenForReading();
                string content = fileManager.ReadAllText();
                Console.WriteLine($"Содержимое файла:\n{content}");

                var info = fileManager.GetFileInfo();
                Console.WriteLine($"Информация о файле:");
                Console.WriteLine($"Размер: {info.Length} байт");
                Console.WriteLine($"Дата создания: {info.CreationTime}\n");
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}