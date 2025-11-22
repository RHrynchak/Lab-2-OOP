using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_2.Models
{
    public class SearchCriteria
    {
        public string Platform { get; set; }
        public bool? Multiplayer { get; set; }
        public string Rating { get; set; }
        public string TitleKeyword { get; set; }
        public string GenreKeyword { get; set; }
        public string DeveloperKeyword { get; set; }
        public string PublisherKeyword { get; set; }
        public DateTime? ReleaseDateFrom { get; set; }
        public DateTime? ReleaseDateTo { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
        public string DescriptionKeyword { get; set; }

        public string GetDescription()
        {
            var criteria = new List<string>();
            if (!string.IsNullOrEmpty(Platform))
                criteria.Add($"Platform: {Platform}");
            if (Multiplayer.HasValue)
                criteria.Add($"Multiplayer: {Multiplayer.Value}");
            if (!string.IsNullOrEmpty(Rating))
                criteria.Add($"Rating: {Rating}");
            if (!string.IsNullOrEmpty(TitleKeyword))
                criteria.Add($"Title contains: '{TitleKeyword}'");
            if (!string.IsNullOrEmpty(GenreKeyword))
                criteria.Add($"Genre: {GenreKeyword}");
            if (!string.IsNullOrEmpty(DeveloperKeyword))
                criteria.Add($"Developer contains: '{DeveloperKeyword}'");
            if (!string.IsNullOrEmpty(PublisherKeyword))
                criteria.Add($"Publisher contains: '{PublisherKeyword}'");
            if (ReleaseDateFrom.HasValue)
                criteria.Add($"Release Date from: {ReleaseDateFrom.Value.ToShortDateString()}");
            if (ReleaseDateTo.HasValue)
                criteria.Add($"Release Date to: {ReleaseDateTo.Value.ToShortDateString()}");
            if (PriceFrom.HasValue)
                criteria.Add($"Price from: {PriceFrom.Value:C}");
            if (PriceTo.HasValue)
                criteria.Add($"Price to: {PriceTo.Value:C}");
            if (!string.IsNullOrEmpty(DescriptionKeyword))
                criteria.Add($"Description contains: '{DescriptionKeyword}'");
            return criteria.Count > 0 ? string.Join("; ", criteria) : "No Criteria";
        }
    }
}
