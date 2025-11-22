using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lab_2.Models
{
    public class LinqParserStrategy : IXmlParserStrategy
    {
        public List<Game> Parse(string xmlPath, SearchCriteria criteria)
        {
            Logger logger = Logger.GetInstance();
            var doc = XDocument.Load(xmlPath);
            var games = doc.Descendants("Game").AsEnumerable();

            if ( !string.IsNullOrWhiteSpace(criteria.Platform) )
            {
                games = games.Where(g => g.Attribute("platform")?.Value.Equals(criteria.Platform,
                                    StringComparison.OrdinalIgnoreCase) == true);
            }
            if ( criteria.Multiplayer.HasValue )
            {
                games = games.Where(g => g.Attribute("multiplayer")?.Value.Equals(
                                    criteria.Multiplayer.Value.ToString().ToLower()) == true);
            }
            if ( !string.IsNullOrWhiteSpace(criteria.Rating) )
            {
                games = games.Where(g => g.Attribute("rating")?.Value.Equals(criteria.Rating,
                                    StringComparison.OrdinalIgnoreCase) == true);
            }
            if ( !string.IsNullOrWhiteSpace(criteria.TitleKeyword) )
            {
                games = games.Where(g => g.Element("Title")?.Value.Contains(criteria.TitleKeyword,
                                    StringComparison.OrdinalIgnoreCase) == true);
            }
            if ( !string.IsNullOrWhiteSpace(criteria.GenreKeyword) )
            {
                games = games.Where(g => g.Element("Genre")?.Value.Contains(criteria.GenreKeyword,
                                    StringComparison.OrdinalIgnoreCase) == true);
            }
            if ( !string.IsNullOrWhiteSpace(criteria.DeveloperKeyword) )
            {
                games = games.Where(g => g.Element("Developer")?.Value.Contains(criteria.DeveloperKeyword,
                                    StringComparison.OrdinalIgnoreCase) == true);
            }
            if ( !string.IsNullOrWhiteSpace(criteria.PublisherKeyword) )
            {
                games = games.Where(g => g.Element("Publisher")?.Value.Contains(criteria.PublisherKeyword,
                                    StringComparison.OrdinalIgnoreCase) == true);
            }
            if ( !string.IsNullOrWhiteSpace(criteria.DescriptionKeyword) )
            {
                games = games.Where(g => g.Element("Description")?.Value.Contains(criteria.DescriptionKeyword,
                                    StringComparison.OrdinalIgnoreCase) == true);
            }
            if ( criteria.ReleaseDateFrom.HasValue || criteria.ReleaseDateTo.HasValue )
            {
                games = games.Where(g =>
                {
                    var dateStr = g.Element("ReleaseDate")?.Value;
                    if (DateTime.TryParse(dateStr, out DateTime releaseDate))
                    {
                        bool matchesFrom = !criteria.ReleaseDateFrom.HasValue ||
                                          releaseDate >= criteria.ReleaseDateFrom.Value;
                        bool matchesTo = !criteria.ReleaseDateTo.HasValue ||
                                        releaseDate <= criteria.ReleaseDateTo.Value;
                        return matchesFrom && matchesTo;
                    }
                    return false;
                });
            }
            if ( criteria.PriceFrom.HasValue || criteria.PriceTo.HasValue )
            {
                games = games.Where(g =>
                {
                    var priceStr = g.Element("Price")?.Value;
                    if (decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                    {
                        bool matchesFrom = !criteria.PriceFrom.HasValue ||
                                          price >= criteria.PriceFrom.Value;
                        bool matchesTo = !criteria.PriceTo.HasValue ||
                                        price <= criteria.PriceTo.Value;
                        return matchesFrom && matchesTo;
                    }
                    return false;
                });
            }
            logger.LogFiltering("Linq", criteria, games.Count());
            return games.Select(g => ParseGame(g)).ToList();
        }

        private Game ParseGame(XElement gameElement)
        {
            var game = new Game
            {
                ID = gameElement.Attribute("id")?.Value,
                Platform = gameElement.Attribute("platform")?.Value,
                Multiplayer = ParseBoolAttribute(gameElement, "multiplayer"),
                Rating = gameElement.Attribute("rating")?.Value,
                Title = gameElement.Element("Title")?.Value,
                Genre = gameElement.Element("Genre")?.Value,
                Developer = gameElement.Element("Developer")?.Value,
                Publisher = gameElement.Element("Publisher")?.Value,
                Description = gameElement.Element("Description")?.Value
            };
            var releaseDateStr = gameElement.Element("ReleaseDate")?.Value;
            if (DateTime.TryParse(releaseDateStr, out DateTime releaseDate))
                game.ReleaseDate = releaseDate;
            var priceStr = gameElement.Element("Price")?.Value;
            if (decimal.TryParse(priceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                game.Price = price;
            return game;
        }

        private bool? ParseBoolAttribute(XElement element, string attributeName)
        {
            var value = element.Attribute(attributeName)?.Value;
            if (bool.TryParse(value, out bool result))
                return result;
            return null;
        }
    }
}
