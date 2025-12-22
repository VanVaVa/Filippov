using System.IO;
using System.Text;

namespace Lab01
{
    public class FileResourceManager : IDisposable
    {
        private FileStream? _fileStream;
        private StreamWriter? _writer;
        private StreamReader? _reader;
        private bool _disposed = false;
        private readonly string _filePath;
        private readonly object _lockObject = new();

        public FileResourceManager(string filePath, FileMode fileMode)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

            if (fileMode == FileMode.Open && !File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}", filePath);

            _fileStream = new FileStream(filePath, fileMode, FileAccess.ReadWrite, FileShare.Read);
        }

        public void OpenForWriting()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            lock (_lockObject)
            {
                EnsureFileStream();
                _writer = new StreamWriter(_fileStream!, Encoding.UTF8) { AutoFlush = true };
            }
        }

        public void OpenForReading()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            lock (_lockObject)
            {
                EnsureFileStream();
                _reader = new StreamReader(_fileStream!, Encoding.UTF8);
            }
        }

        public void WriteLine(string text)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));
            if (_writer == null) throw new InvalidOperationException("Файл не открыт для записи");

            lock (_lockObject)
            {
                try
                {
                    _writer.WriteLine(text);
                }
                catch (IOException ex)
                {
                    throw new IOException($"Ошибка записи в файл: {ex.Message}", ex);
                }
            }
        }

        public string ReadAllText()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));
            if (_reader == null) throw new InvalidOperationException("Файл не открыт для чтения");

            lock (_lockObject)
            {
                try
                {
                    _fileStream!.Seek(0, SeekOrigin.Begin);
                    return _reader.ReadToEnd();
                }
                catch (IOException ex)
                {
                    throw new IOException($"Ошибка чтения файла: {ex.Message}", ex);
                }
            }
        }

        public void AppendText(string text)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            lock (_lockObject)
            {
                try
                {
                    using (var writer = new StreamWriter(_filePath, true, Encoding.UTF8))
                    {
                        writer.Write(text);
                    }
                }
                catch (IOException ex)
                {
                    throw new IOException($"Ошибка добавления текста в файл: {ex.Message}", ex);
                }
            }
        }

        public FileInfo GetFileInfo()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(FileResourceManager));

            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Файл не найден: {_filePath}");

            var fileInfo = new FileInfo(_filePath);
            return fileInfo;
        }

        private void EnsureFileStream()
        {
            if (_fileStream == null || !_fileStream.CanRead)
                throw new InvalidOperationException("Файловый поток недоступен");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    lock (_lockObject)
                    {
                        _writer?.Dispose();
                        _reader?.Dispose();
                        _fileStream?.Dispose();

                        _writer = null;
                        _reader = null;
                        _fileStream = null;
                    }
                }

                _disposed = true;
            }
        }

        ~FileResourceManager()
        {
            Dispose(false);
        }
    }
}