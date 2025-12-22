using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

namespace Lab01
{
    public class PersonSerializer
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly ConcurrentDictionary<string, object> _fileLocks = new();
        private static readonly string _logFilePath = "errors.txt";

        public string SerializeToJson(Person person)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));

            try
            {
                return JsonSerializer.Serialize(person, _jsonOptions);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                throw;
            }
        }

        public Person DeserializeFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON строка не может быть пустой");

            try
            {
                var person = JsonSerializer.Deserialize<Person>(json, _jsonOptions);
                if (person == null)
                {
                    throw new JsonException("Не удалось десериализовать объект Person");
                }

                return person;
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                throw;
            }
        }

        public void SaveToFile(Person person, string filePath)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым");

            var lockObject = _fileLocks.GetOrAdd(filePath, new object());

            lock (lockObject)
            {
                try
                {
                    string json = SerializeToJson(person);
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    throw;
                }
            }
        }

        public Person LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var lockObject = _fileLocks.GetOrAdd(filePath, new object());

            lock (lockObject)
            {
                try
                {
                    string json = File.ReadAllText(filePath, Encoding.UTF8);
                    return DeserializeFromJson(json);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    throw;
                }
            }
        }

        public async Task SaveToFileAsync(Person person, string filePath)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым");

            var lockObject = _fileLocks.GetOrAdd(filePath, new object());

            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    try
                    {
                        string json = SerializeToJson(person);
                        File.WriteAllText(filePath, json, Encoding.UTF8);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message);
                        throw;
                    }
                }
            });
        }

        public async Task<Person> LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var lockObject = _fileLocks.GetOrAdd(filePath, new object());

            return await Task.Run(() =>
            {
                lock (lockObject)
                {
                    try
                    {
                        string json = File.ReadAllText(filePath, Encoding.UTF8);
                        return DeserializeFromJson(json);
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.Message);
                        throw;
                    }
                }
            });
        }

        public void SaveListToFile(List<Person> people, string filePath)
        {
            if (people == null)
                throw new ArgumentNullException(nameof(people));
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Путь к файлу не может быть пустым");

            var lockObject = _fileLocks.GetOrAdd(filePath, new object());

            lock (lockObject)
            {
                try
                {
                    string json = JsonSerializer.Serialize(people, _jsonOptions);
                    File.WriteAllText(filePath, json, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    throw;
                }
            }
        }

        public List<Person> LoadListFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var lockObject = _fileLocks.GetOrAdd(filePath, new object());

            lock (lockObject)
            {
                try
                {
                    string json = File.ReadAllText(filePath, Encoding.UTF8);
                    return JsonSerializer.Deserialize<List<Person>>(json, _jsonOptions) ??
                            new List<Person>();
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    throw;
                }
            }
        }

        private void LogError(string errorMessage)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {errorMessage}{Environment.NewLine}";
                File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
            }
            catch
            {
            }
        }
    }
}
