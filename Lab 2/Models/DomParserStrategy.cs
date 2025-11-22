using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml;

namespace Lab_2.Models
{
    public class DomParserStrategy : IXmlParserStrategy
    {
        public List<Game> Parse(string xmlPath, SearchCriteria criteria)
        {
            Logger logger = Logger.GetInstance();
            var games = new List<Game>();
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            var gameNodes = xmlDoc.SelectNodes("//Game");
            if (gameNodes == null)
                return games;

            foreach (XmlNode gameNode in gameNodes)
            {
                var game = ParseGameNode(gameNode);
                if ( game!= null && MatchesCriteria(game, criteria))
                {
                    games.Add(game);
                }
            }
            logger.LogFiltering("DOM", criteria, games.Count);
            return games;
        }

        private Game ParseGameNode(XmlNode gameNode)
        {

            if (gameNode == null)
                return null;
            var game = new Game();

            if ( gameNode.Attributes != null )
            {
                game.ID = GetAttributeValue(gameNode, "ID");
                game.Platform = GetAttributeValue(gameNode, "platform");
                game.Multiplayer = ParseBoolAttribute(gameNode, "multiplayer");
                game.Rating = GetAttributeValue(gameNode, "rating");
            }

            foreach (XmlNode childNode in gameNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "Title":
                        game.Title = childNode.InnerText;
                        break;
                    case "Genre":
                        game.Genre = childNode.InnerText;
                        break;
                    case "Developer":
                        game.Developer = childNode.InnerText;
                        break;
                    case "Publisher":
                        game.Publisher = childNode.InnerText;
                        break;
                    case "ReleaseDate":
                        if (DateTime.TryParse(childNode.InnerText, out DateTime releaseDate))
                            game.ReleaseDate = releaseDate;
                        break;
                    case "Price":
                        if (decimal.TryParse(childNode.InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                            game.Price = price;
                        break;
                    case "Description":
                        game.Description = childNode.InnerText;
                        break;
                }
            }

            return game;
        }

        private string GetAttributeValue(XmlNode node, string attributeName)
        {
            return node.Attributes?[attributeName]?.Value;
        }

        private bool? ParseBoolAttribute(XmlNode node, string attributeName)
        {
            var value = GetAttributeValue(node, attributeName);
            if (bool.TryParse(value, out bool result))
                return result;
            return null;
        }

        private bool MatchesCriteria(Game game, SearchCriteria criteria)
        {
            if (!string.IsNullOrEmpty(criteria.Platform) &&
                !string.Equals(game.Platform, criteria.Platform, StringComparison.OrdinalIgnoreCase))
                return false;
            if (criteria.Multiplayer.HasValue &&
                game.Multiplayer != criteria.Multiplayer)
                return false;
            if (!string.IsNullOrEmpty(criteria.Rating) &&
                !string.Equals(game.Rating, criteria.Rating, StringComparison.OrdinalIgnoreCase))
                return false;
            if (!string.IsNullOrEmpty(criteria.TitleKeyword) &&
                (game.Title == null || !game.Title.Contains(criteria.TitleKeyword, StringComparison.OrdinalIgnoreCase)))
                return false;
            if (!string.IsNullOrEmpty(criteria.GenreKeyword) &&
                (game.Genre == null || !game.Genre.Contains(criteria.GenreKeyword, StringComparison.OrdinalIgnoreCase)))
                return false;
            if (!string.IsNullOrEmpty(criteria.DeveloperKeyword) &&
                (game.Developer == null || !game.Developer.Contains(criteria.DeveloperKeyword, StringComparison.OrdinalIgnoreCase)))
                return false;
            if (!string.IsNullOrEmpty(criteria.PublisherKeyword) &&
                (game.Publisher == null || !game.Publisher.Contains(criteria.PublisherKeyword, StringComparison.OrdinalIgnoreCase)))
                return false;
            if (criteria.ReleaseDateFrom.HasValue &&
                game.ReleaseDate < criteria.ReleaseDateFrom.Value)
                return false;
            if (criteria.ReleaseDateTo.HasValue &&
                game.ReleaseDate > criteria.ReleaseDateTo.Value)
                return false;
            if (criteria.PriceFrom.HasValue &&
                (!game.Price.HasValue || game.Price.Value < criteria.PriceFrom.Value))
                return false;
            if (criteria.PriceTo.HasValue &&
                (!game.Price.HasValue || game.Price.Value > criteria.PriceTo.Value))
                return false;
            if (!string.IsNullOrEmpty(criteria.DescriptionKeyword) &&
                (game.Description == null || !game.Description.Contains(criteria.DescriptionKeyword, StringComparison.OrdinalIgnoreCase)))
                return false;
            return true;
        }
    }
}
