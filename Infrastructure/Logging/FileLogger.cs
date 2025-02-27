// Infrastructure/Logging/FileLogger.cs
using System;
using System.IO;

namespace Infrastructure.Logging;

public class FileLogger : ILogger
{
    private readonly string _logFilePath;

    public FileLogger()
    {
        _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.log");
        InitializeLogFile();
    }

    private void InitializeLogFile()
    {
        try
        {
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
            File.WriteAllText(_logFilePath, $"Log initialized at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new InvalidOperationException($"Cannot initialize log file at {_logFilePath}: Access denied.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Cannot initialize log file at {_logFilePath}: IO error.", ex);
        }
    }

    public void Info(string message)
    {
        Log("INFO", message);
    }

    public void Warning(string message)
    {
        Log("WARNING", message);
    }

    public void Error(string message, Exception ex = null)
    {
        Log("ERROR", $"{message}{(ex != null ? $"\nException: {ex}" : "")}");
    }

    private void Log(string level, string message)
    {
        try
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            // Если запись в лог не удалась, выводим в консоль (или можно игнорировать)
            Console.WriteLine($"Failed to write to log file: {ex.Message}");
        }
    }
}