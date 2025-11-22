using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Xml;

namespace Lab_2.Models
{
    public class SaxParserStrategy : IXmlParserStrategy
    {
        public List<Game> Parse(string xmlPath, SearchCriteria criteria)
        {
            Logger logger = Logger.GetInstance();
            var games = new List<Game>();
            Game currentGame = null;
            string currentElement = null;
            using (XmlReader reader = XmlTextReader.Create(xmlPath))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            currentElement = reader.Name;
                            if (currentElement == "Game")
                            {
                                currentGame = new Game();
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        switch (reader.Name)
                                        {
                                            case "ID":
                                                currentGame.ID = reader.Value;
                                                break;
                                            case "platform":
                                                currentGame.Platform = reader.Value;
                                                break;
                                            case "multiplayer":
                                                currentGame.Multiplayer = bool.Parse(reader.Value);
                                                break;
                                            case "rating":
                                                currentGame.Rating = reader.Value;
                                                break;
                                        }
                                    }
                                    reader.MoveToElement();
                                }
                            }
                            break;

                        case XmlNodeType.Text:
                            if ( currentGame != null && currentElement != null)
                            {
                                string value = reader.Value;
                                switch (currentElement)
                                {
                                    case "Title":
                                        currentGame.Title = value;
                                        break;
                                    case "Genre":
                                        currentGame.Genre = value;
                                        break;
                                    case "Developer":
                                        currentGame.Developer = value;
                                        break;
                                    case "Publisher":
                                        currentGame.Publisher = value;
                                        break;
                                    case "ReleaseDate":
                                        if ( DateTime.TryParse(value, out DateTime date) )
                                            currentGame.ReleaseDate = date;
                                        break;
                                    case "Price":
                                        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                                            currentGame.Price = price;
                                        break;
                                    case "Description":
                                        currentGame.Description = value;
                                        break;
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            if (reader.Name == "Game" && currentGame != null)
                            {
                                if (MatchesCriteria(currentGame, criteria))
                                {
                                    games.Add(currentGame);
                                }
                                currentGame = null;
                            }
                            currentElement = null;
                            break;
                    }
                }
            }
            logger.LogFiltering("SAX", criteria, games.Count);
            return games;
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
