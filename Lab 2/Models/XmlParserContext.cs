using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2.Models
{
    public interface IXmlParserStrategy
    {
        List<Game> Parse(string xmlPath, SearchCriteria criteria);
    }

    public class XmlParserContext
    {
        private IXmlParserStrategy _strategy;
        public void SetStrategy(IXmlParserStrategy strategy)
        {
            _strategy = strategy;
        }
        public List<Game> Parse(string xmlPath, SearchCriteria criteria)
        {
            if (_strategy == null)
            {
                throw new InvalidOperationException("XML parser strategy is not set.");
            }
            return _strategy.Parse(xmlPath, criteria);
        }
    }
}
