using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2.Models
{
    public abstract class FileExporter
    {
        protected abstract IFileWriter CreateWriter();
        public void Export(List<Game> games, string filePath)
        {
            try
            {
                var writer = CreateWriter();
                if (!filePath.EndsWith(writer.GetFileExtension()))
                {
                    filePath += writer.GetFileExtension();
                }
                writer.Write(games, filePath);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().LogError($"Експорт у {filePath}", ex);
                throw;
            }
        }
    }
    public class XmlExporter : FileExporter
    {
        protected override IFileWriter CreateWriter()
        {
            return new XmlFileWriter();
        }
    }

    public class HtmlExporter : FileExporter
    {
        private string _xslPath;

        public HtmlExporter(string xslPath = "GameLibrary.xslt")
        {
            _xslPath = xslPath;
        }

        protected override IFileWriter CreateWriter()
        {
            return new HtmlFileWriter(_xslPath);
        }
    }

    public class GoogleDriveXmlExporter : FileExporter
    {
        private readonly GoogleDriveService _driveService;

        public GoogleDriveXmlExporter(GoogleDriveService driveService)
        {
            _driveService = driveService ?? throw new ArgumentNullException(nameof(driveService));
        }

        protected override IFileWriter CreateWriter()
        {
            return new GoogleDriveXmlWriter(_driveService);
        }
    }

    public class GoogleDriveHtmlExporter : FileExporter
    {
        private readonly GoogleDriveService _driveService;
        private readonly string _xslPath;

        public GoogleDriveHtmlExporter(GoogleDriveService driveService, string xslPath = "GameLibrary.xsl")
        {
            _driveService = driveService ?? throw new ArgumentNullException(nameof(driveService));
            _xslPath = xslPath;
        }

        protected override IFileWriter CreateWriter()
        {
            return new GoogleDriveHtmlWriter(_driveService, _xslPath);
        }
    }
}
