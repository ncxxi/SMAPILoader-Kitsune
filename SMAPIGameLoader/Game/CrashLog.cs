using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SMAPIGameLoader;

internal static class CrashLog
{
    static readonly object Sync = new();
    static TextWriter? _writer;

    public static string LogFilePath { get; } =
        Path.Combine(FileTool.ExternalFilesDir, "kitsune.log");

    public static void Init()
    {
        lock (Sync)
        {
            if (_writer != null)
                return;

            try
            {
                var directory = Path.GetDirectoryName(LogFilePath);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                var stream = new FileStream(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                var writer = new StreamWriter(stream) { AutoFlush = true };
                var synchronized = TextWriter.Synchronized(writer);
                _writer = synchronized;

                var original = Console.Out ?? TextWriter.Null;
                Console.SetOut(new TeeWriter(original, synchronized));
                Console.WriteLine($"--- SMAPI Kitsune {AppInfo.VersionString} ({AppInfo.BuildString}) ---");

                AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                    Write("UNHANDLED: " + e.ExceptionObject);
                Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (_, e) =>
                    Write("ANDROID UNHANDLED: " + e.Exception);
                TaskScheduler.UnobservedTaskException += (_, e) =>
                    Write("UNOBSERVED TASK: " + e.Exception);
            }
            catch
            {
                // Logging must never block startup.
            }
        }
    }

    public static void Write(string message)
    {
        if (_writer == null)
            Init();

        lock (Sync)
        {
            try
            {
                _writer?.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}");
            }
            catch
            {
                // Ignore logging failures.
            }
        }
    }

    public static string ReadTail(int maxChars = 40000)
    {
        lock (Sync)
        {
            try
            {
                _writer?.Flush();
                if (!File.Exists(LogFilePath))
                    return "(log is empty)";

                using var stream = new FileStream(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (stream.Length > maxChars)
                    stream.Seek(-maxChars, SeekOrigin.End);

                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    sealed class TeeWriter : TextWriter
    {
        readonly TextWriter _console;
        readonly TextWriter _file;

        public TeeWriter(TextWriter console, TextWriter file)
        {
            _console = console;
            _file = file;
        }

        public override Encoding Encoding => _console.Encoding;

        public override void Write(char value)
        {
            _console.Write(value);
            _file.Write(value);
        }

        public override void Write(string? value)
        {
            _console.Write(value);
            _file.Write(value);
        }

        public override void WriteLine(string? value)
        {
            _console.WriteLine(value);
            _file.WriteLine(value);
        }

        public override void Flush()
        {
            _console.Flush();
            _file.Flush();
        }
    }
}
