using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Xml;

namespace Lab_2.Models
{
    public interface IFileWriter
    {
        void Write(List<Game> games, string filePath);
        string GetFileExtension();
    }
    public class XmlFileWriter : IFileWriter
    {
        public void Write(List<Game> games, string filePath)
        {
            using ( var writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true }) )
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("GameLibrary");
                foreach (var game in games)
                {
                    writer.WriteStartElement("Game");

                    // Атрибути
                    if (!string.IsNullOrEmpty(game.ID))
                        writer.WriteAttributeString("id", game.ID);
                    if (!string.IsNullOrEmpty(game.Platform))
                        writer.WriteAttributeString("platform", game.Platform);
                    if (game.Multiplayer.HasValue)
                        writer.WriteAttributeString("multiplayer", game.Multiplayer.Value.ToString().ToLower());
                    if (!string.IsNullOrEmpty(game.Rating))
                        writer.WriteAttributeString("rating", game.Rating);

                    // Елементи
                    if (!string.IsNullOrEmpty(game.Title))
                        writer.WriteElementString("Title", game.Title);
                    if (!string.IsNullOrEmpty(game.Developer))
                        writer.WriteElementString("Developer", game.Developer);
                    if (!string.IsNullOrEmpty(game.Publisher))
                        writer.WriteElementString("Publisher", game.Publisher);
                    if (!string.IsNullOrEmpty(game.Genre))
                        writer.WriteElementString("Genre", game.Genre);
                    if (game.ReleaseDate != DateTime.MinValue)
                        writer.WriteElementString("ReleaseDate", game.ReleaseDate.ToString("yyyy-MM-dd"));
                    if (game.Price.HasValue)
                        writer.WriteElementString("Price", game.Price.Value.ToString("0.00", CultureInfo.InvariantCulture));
                    if (!string.IsNullOrEmpty(game.Description))
                        writer.WriteElementString("Description", game.Description);

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Logger.GetInstance().LogSaving(filePath);
        }
        public string GetFileExtension() => ".xml";
    }

    public class HtmlFileWriter : IFileWriter
    {
        private string _xslPath;
        public HtmlFileWriter(string xslPath)
        {
            _xslPath = xslPath;
        }
        public void Write(List<Game> games, string filePath)
        {
            string tempXmlPath = System.IO.Path.GetTempFileName();
            var xmlWriter = new XmlFileWriter();
            xmlWriter.Write(games, tempXmlPath);

            var transformer = new XslTransformer();
            transformer.Transform(tempXmlPath, _xslPath, filePath);

            System.IO.File.Delete(tempXmlPath);

            Logger.GetInstance().LogSaving(filePath);
        }   
        public string GetFileExtension() => ".html";
    }

    public class GoogleDriveXmlWriter : IFileWriter
    {
        private readonly GoogleDriveService _driveService;
        public GoogleDriveXmlWriter(GoogleDriveService driveService)
        {
            _driveService = driveService;
        }
        public void Write(List<Game> games, string filePath)
        {
            var localWriter = new XmlFileWriter();
            string tempXmlPath = Path.GetTempFileName();
            localWriter.Write(games, tempXmlPath);
            try
            {
                string xmlPath = filePath.EndsWith(".xml") ? filePath : filePath + ".xml";
                string finalTempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(xmlPath));
                if (File.Exists(finalTempPath))
                    File.Delete(finalTempPath);
                File.Move(tempXmlPath, finalTempPath);
                string fileId = _driveService.UploadFile(finalTempPath, GoogleDriveService.GetMimeType(finalTempPath));
                File.Delete(finalTempPath);
                Logger.GetInstance().LogInfo($"XML файл завантажено на Google Диск з ID: {fileId}");
            }
            catch (Exception ex)
            {
                if (File.Exists(tempXmlPath))
                    File.Delete(tempXmlPath);
                Logger.GetInstance().LogError("Запис XML на гугл диск", ex);
            }
        }
        public string GetFileExtension() => ".xml";
    }

    public class GoogleDriveHtmlWriter : IFileWriter
    {
        private readonly GoogleDriveService _driveService;
        private readonly string _xslPath;
        public GoogleDriveHtmlWriter(GoogleDriveService driveService, string xslPath)
        {
            _driveService = driveService;
            _xslPath = xslPath;
        }
        public void Write(List<Game> games, string filePath)
        {
            var localWriter = new HtmlFileWriter(_xslPath);
            string tempHtmlPath = Path.GetTempFileName();
            localWriter.Write(games, tempHtmlPath);
            try
            {
                string htmlPath = filePath.EndsWith(".html") ? filePath : filePath + ".html";
                string finalTempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(htmlPath));
                if (File.Exists(finalTempPath))
                    File.Delete(finalTempPath);
                File.Move(tempHtmlPath, finalTempPath);
                string fileId = _driveService.UploadFile(finalTempPath, GoogleDriveService.GetMimeType(finalTempPath));
                File.Delete(finalTempPath);
                Logger.GetInstance().LogInfo($"HTML файл завантажено на Google Диск з ID: {fileId}");
            }
            catch (Exception ex)
            {
                if (File.Exists(tempHtmlPath))
                    File.Delete(tempHtmlPath);
                Logger.GetInstance().LogError("Запис HTML на гугл диск", ex);
            }
        }
        public string GetFileExtension() => ".html";
    }
}
