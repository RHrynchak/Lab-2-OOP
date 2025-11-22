using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace Lab_2.Models
{
    public class XslTransformer
    {
        public void Transform(string xmlPath, string xslPath, string outputPath)
        {
            Logger logger = Logger.GetInstance();
            try
            {
                if ( !File.Exists(xmlPath) )
                    throw new FileNotFoundException($"XML файл не знайдено: {xmlPath}");
                if ( !File.Exists(xslPath))
                    throw new FileNotFoundException($"XSL файл не знайдено: {xslPath}");
                var xsl = new XslCompiledTransform();
                xsl.Load(xslPath);
                xsl.Transform(xmlPath, outputPath);
                logger.LogTransformation(xmlPath, outputPath, xslPath);
                Console.WriteLine($"Трансформація завершена. Результат збережено у: {outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час трансформації XSL: {ex.Message}");
                logger.LogError("XSL трансформація", ex);
                throw;
            }
        }
    }
}
