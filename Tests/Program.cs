using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab_2.Models;

namespace Lab_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            string xmlPath = "C:\\Users\\Роман\\OneDrive\\Робочий стіл\\проги\\OOP\\Lab 2\\Tests\\GameLibrary.xml";
            string xslPath = "C:\\Users\\Роман\\OneDrive\\Робочий стіл\\проги\\OOP\\Lab 2\\Tests\\GameLibrary.xsl";
            string outputPath = "C:\\Users\\Роман\\OneDrive\\Робочий стіл\\проги\\OOP\\Lab 2\\Tests\\GameLibrary.html";

            // Тест XSLT трансформації
            Console.WriteLine("=== Тест XSLT трансформації ===");
            TestXslTransformation(xmlPath, xslPath, outputPath);

            // Тест 1: Пошук за платформою
            Console.WriteLine("=== Тест 1: Всі PC ігри ===");
            var criteria1 = new SearchCriteria
            {
                Platform = "PC"
            };
            TestParser(xmlPath, criteria1);
            // Тест 2: Пошук RPG
            Console.WriteLine("\n=== Тест 2: Всі RPG ===");
            var criteria2 = new SearchCriteria
            {
                GenreKeyword = "RPG"
            };

            TestParser(xmlPath, criteria2);

            // Тест 3: Пошук за розробником
            Console.WriteLine("\n=== Тест 3: Ігри від CD Projekt ===");
            var criteria3 = new SearchCriteria
            {
                DeveloperKeyword = "CD Projekt"
            };

            TestParser(xmlPath, criteria3);

            // Тест 4: Комбінований пошук
            Console.WriteLine("\n=== Тест 4: PC RPG від 20 до 50$ ===");
            var criteria4 = new SearchCriteria
            {
                Platform = "PC",
                GenreKeyword = "RPG",
                PriceFrom = 20,
                PriceTo = 50
            };

            TestParser(xmlPath, criteria4);

            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
            Console.WriteLine();

            Logger logger = Logger.GetInstance();
            logger.OpenLogFile();
        }
        static void TestParser( string xmlPath, SearchCriteria criteria )
        {
            Console.WriteLine($"Критерії: {criteria.GetDescription()}");
            Console.WriteLine();

            var context = new XmlParserContext();

            // SAX
            Console.WriteLine("--- SAX Parser ---");
            context.SetStrategy(new SaxParserStrategy());
            var saxResults = context.Parse(xmlPath, criteria);
            PrintResults(saxResults);

            Console.WriteLine();
            // DOM
            Console.WriteLine("--- DOM Parser ---");
            context.SetStrategy(new DomParserStrategy());
            var domResults = context.Parse(xmlPath, criteria);
            PrintResults(domResults);

            Console.WriteLine();
            // LINQ
            Console.WriteLine("--- LINQ Parser ---");
            context.SetStrategy(new LinqParserStrategy());
            var linqResults = context.Parse(xmlPath, criteria);
            PrintResults(linqResults);
        }

        static void PrintResults(List<Game> games)
        {
            Console.WriteLine($"Знайдено: {games.Count} ігор");
            foreach (var game in games)
            {
                Console.WriteLine($"  {game.ToString()}");
            }
        }

        static void TestXslTransformation(string xmlPath, string xslPath, string outputPath)
        {
            try
            {
                var transformer = new XslTransformer();
                transformer.Transform(xmlPath, xslPath, outputPath);
                Console.WriteLine($"Створено: {outputPath}");
                OpenInBrowser(outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час XSLT трансформації: {ex.Message}");
            }
            Console.WriteLine();    
        }

        static void OpenInBrowser(string filePath)
        {
            try
            {
                // Відкриваємо HTML у браузері за замовчуванням
                var fullPath = System.IO.Path.GetFullPath(filePath);
                Process.Start(new ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true
                });
                Console.WriteLine($"Відкрито в браузері");
            }
            catch
            {
                Console.WriteLine($"Не вдалося відкрити браузер автоматично");
            }
        }
    }
}
