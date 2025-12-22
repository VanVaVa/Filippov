using System.Text;
using Xunit;
using Lab01;

namespace Lab1_3.Tests
{
    public class FileResourceManagerTests : IDisposable
    {
        private readonly string _testFilePath;

        public FileResourceManagerTests()
        {
            _testFilePath = Path.GetTempFileName();
        }

        [Fact]
        public void Constructor_ShouldCreateFile_WithCreateMode()
        {
            var tempFile = Path.GetTempFileName();
            File.Delete(tempFile);

            using var manager = new FileResourceManager(tempFile, FileMode.Create);

            Assert.True(File.Exists(tempFile));

            File.Delete(tempFile);
        }

        [Fact]
        public void Constructor_ShouldThrowFileNotFoundException_WhenOpenModeAndFileDoesNotExist()
        {
            var nonExistentFile = "kakoytofile.txt";

            Assert.Throws<FileNotFoundException>(() =>
                new FileResourceManager(nonExistentFile, FileMode.Open));
        }

        [Fact]
        public void OpenForWriting_And_WriteLine_ShouldWorkCorrectly()
        {
            using var manager = new FileResourceManager(_testFilePath, FileMode.Create);
            const string testText = "Hello, Egor!";

            manager.OpenForWriting();
            manager.WriteLine(testText);

            var fileContent = File.ReadAllText(_testFilePath, Encoding.UTF8);
            Assert.Contains(testText, fileContent);
            Assert.EndsWith(Environment.NewLine, fileContent);
        }

        [Fact]
        public void OpenForReading_And_ReadAllText_ShouldWorkCorrectly()
        {
            const string testText = "Test text";
            File.WriteAllText(_testFilePath, testText, Encoding.UTF8);

            using var manager = new FileResourceManager(_testFilePath, FileMode.Open);

            manager.OpenForReading();
            var content = manager.ReadAllText();

            Assert.Equal(testText, content.TrimEnd());
        }

        [Fact]
        public void WriteLine_ShouldThrowInvalidOperationException_WhenNotOpenedForWriting()
        {
            using var manager = new FileResourceManager(_testFilePath, FileMode.Create);

            Assert.Throws<InvalidOperationException>(() =>
                manager.WriteLine("asdfahskajhfask"));
        }

        [Fact]
        public void ReadAllText_ShouldThrowInvalidOperationException_WhenNotOpenedForReading()
        {
            using var manager = new FileResourceManager(_testFilePath, FileMode.Create);
            manager.OpenForWriting();
            manager.WriteLine("test");

            Assert.Throws<InvalidOperationException>(() =>
                manager.ReadAllText());
        }

        [Fact]
        public void AppendText_ShouldAddTextToEndOfFile()
        {
            const string initialText = "I love C#";
            const string appendedText = "I REALLY love C#";

            File.WriteAllText(_testFilePath, initialText, Encoding.UTF8);

            using var manager = new FileResourceManager(_testFilePath, FileMode.Open);

            manager.AppendText(appendedText);

            var finalContent = File.ReadAllText(_testFilePath, Encoding.UTF8);
            Assert.Equal(initialText + appendedText, finalContent);
        }

        [Fact]
        public void GetFileInfo_ShouldReturnCorrectInformation()
        {
            const string testContent = "Test content for file info";
            File.WriteAllText(_testFilePath, testContent, Encoding.UTF8);

            var fileInfo = new FileInfo(_testFilePath);

            using var manager = new FileResourceManager(_testFilePath, FileMode.Open);

            var info = manager.GetFileInfo();

            Assert.NotNull(info);
            Assert.Equal(fileInfo.Length, info.Length);
            Assert.Equal(fileInfo.CreationTime, info.CreationTime);
            Assert.Equal(fileInfo.LastWriteTime, info.LastWriteTime);
            Assert.Equal(fileInfo.FullName, info.FullName);
        }

        [Fact]
        public void GetFileInfo_ShouldThrowFileNotFoundException_WhenFileDeletedExternally()
        {
            var tempFile = Path.GetTempFileName();
            using var manager = new FileResourceManager(tempFile, FileMode.Open);

            File.Delete(tempFile);

            Assert.Throws<FileNotFoundException>(() =>
                manager.GetFileInfo());
        }

        [Fact]
        public void Dispose_ShouldReleaseResources()
        {
            var manager = new FileResourceManager(_testFilePath, FileMode.Create);
            manager.OpenForWriting();
            manager.WriteLine("Test");

            manager.Dispose();

            Assert.Throws<ObjectDisposedException>(() =>
                manager.WriteLine("test"));

            Assert.Throws<ObjectDisposedException>(() =>
                manager.OpenForReading());
        }

        [Fact]
        public void UsingStatement_ShouldAutoDispose()
        {
            FileResourceManager manager;

            using (manager = new FileResourceManager(_testFilePath, FileMode.Create))
            {
                manager.OpenForWriting();
                manager.WriteLine("Test");
            }

            Assert.Throws<ObjectDisposedException>(() =>
                manager.OpenForWriting());
        }

        [Fact]
        public void DoubleDispose_ShouldNotThrowException()
        {
            var manager = new FileResourceManager(_testFilePath, FileMode.Create);

            manager.Dispose();

            var exception = Record.Exception(() => manager.Dispose());
            Assert.Null(exception);
        }


        [Fact]
        public void MultipleOperations_ShouldWorkInSequence()
        {
            using var manager = new FileResourceManager(_testFilePath, FileMode.Create);

            manager.OpenForWriting();
            manager.WriteLine("Line 1");
            manager.WriteLine("Line 2");

            manager.OpenForReading();
            var content = manager.ReadAllText();
            Assert.Contains("Line 1", content);
            Assert.Contains("Line 2", content);

            manager.OpenForWriting();
            manager.WriteLine("Line 3");

            manager.OpenForReading();
            content = manager.ReadAllText();
            Assert.Contains("Line 3", content);
        }


        [Fact]
        public void OpenForWriting_ShouldOverwriteFile()
        {
            File.WriteAllText(_testFilePath, "Old", Encoding.UTF8);

            using var manager = new FileResourceManager(_testFilePath, FileMode.Create);

            manager.OpenForWriting();
            manager.WriteLine("New");

            var content = File.ReadAllText(_testFilePath, Encoding.UTF8);
            Assert.DoesNotContain("Old", content);
            Assert.Contains("New", content);
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }
    }
}