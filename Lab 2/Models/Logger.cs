using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2.Models
{
    public class Logger
    {
        private static Logger _instance;
        private string _logFilePath;
        private static readonly object _lock = new object();
        private Logger()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string logFolder = Path.Combine(appDataFolder, "Lab_2_Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            string logFileName = $"log_{DateTime.Now:dd-MM-yyyy HH-mm-ss}.txt";
            _logFilePath = Path.Combine(logFolder, logFileName);

            WriteLog($"Logger ініціалізовано: {DateTime.Now:dd-MM-yyyy HH-mm-ss}");
            WriteLog($"Шлях до логу: {_logFilePath}");
        }
        public static Logger GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                }
            }
            return _instance;
        }

        private void LogEvent( string message )
        {
            WriteLog($"{DateTime.Now:dd-MM-yyyy HH:mm:ss} - {message}");
        }
        public void LogFiltering( string parserType, SearchCriteria criteria, int resultsCount )
        {
            string message = $"Фільтрація парсером {parserType}. Фільтри: {criteria.GetDescription()}. Знайдено {resultsCount} ігор";
            LogEvent(message);
        }
        public void LogTransformation( string inputFile, string outputFile, string transformationFile )
        {
            string message = $"Трансформація {inputFile} в {outputFile} за допомогою {transformationFile}";
            LogEvent(message);  
        }
        public void LogSaving( string filePath )
        {
            string message = $"Збереження файлу: {filePath}";
            LogEvent(message);
        }
        public void LogError( string opearation, Exception ex)
        {
            string message = $"Помилка під час {opearation}: {ex.Message}";
            LogEvent(message);
        }
        public void LogInfo( string infoMessage )
        {
            string message = $"Інформація: {infoMessage}";
            LogEvent(message);
        }
        private void WriteLog(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, message + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запису в лог: {ex.Message}");
            }
        }

        public string GetLogFilePath()
        {
            return _logFilePath;
        }
        public void OpenLogFile()
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = _logFilePath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка відкриття логу: {ex.Message}");
            }
        }
        public void ClearLogFile()
        {
            try
            {
                File.WriteAllText(_logFilePath, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка очищення логу: {ex.Message}");
            }
        }
    }
}
