using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2.Models
{
    public class Game
    {
        // Attributes
        public string ID { get; set; }
        public string Platform { get; set; }
        public bool? Multiplayer { get; set; }
        public string Rating { get; set; }

        // Elements
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public DateTime ReleaseDate { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Title} ({Platform}, {Rating}) - {Genre} by {Developer} and {Publisher}, released on {ReleaseDate.ToShortDateString()} for {Price}";
        }
    }
}
