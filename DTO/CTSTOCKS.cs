/*using System;
using System.Collections.Generic;
using System.Linq;
using Enterprise.Models;

namespace Enterprise.DTOs
{
    public class CGPARKS
    {
        public required string Id { get; set; }              // UUID exposed to UI
        public required string ParkName { get; set; }        // Maps from Park.Name
        public required string Location { get; set; }        // Maps from Park.Address
        public required string Description { get; set; }     // Maps from Park.Description
        public required double AdultPrice { get; set; }      // Maps from Park.AdultPrice
        public required double ChildPrice { get; set; }      // Maps from Park.ChildPrice
        public required decimal SomeLat { get; set; }        // Maps from Park.Pic1url
    	public required decimal SomeLong { get; set; }        // Maps from Park.Pic1url
    	public required double ParkId { get; set; }        // Maps from Park.Pic1url
    	public required string ImageUrl { get; set; }        // Maps from Park.Pic1url
        public required List<ParkReviewDto> Reviews { get; set; } = new List<ParkReviewDto>();

        /// <summary>
        /// Factory method to map from Park + Reviews → CGPARKS DTO
        /// </summary>
        public static CGPARKS FromPark(Park park, IEnumerable<ParkReview> reviews)
        {
            return new CGPARKS
            {
                Id = park.Id ?? string.Empty,
                ParkName = park.Name ?? string.Empty,
                Location = park.Address ?? string.Empty,
                Description = park.Description ?? string.Empty,
                AdultPrice = park.AdultPrice ?? 0,
                ChildPrice = park.ChildPrice ?? 0,
                SomeLat = park.Latitude ?? 0,
                SomeLong = park.Longitude ?? 0,
                ParkId = park.ParkId,
                ImageUrl = park.Pic1url ?? string.Empty,
                Reviews = reviews.Select(r => new ParkReviewDto
                {
                    Author = new AuthorDto
                    {
                        Id = r.Useridasstring ?? string.Empty,
                        DisplayName = r.Displayname ?? string.Empty, // already in review
                        FullName = r.Fullname ?? "SomeName",                     
                        DateOfBirth = new DateOnly(2001, 1, 1).ToDateTime(TimeOnly.MinValue).ToString("o")      // fixed default
                    },
                    Rating = r.Stars,
                    DateWritten = r.DatePosted,
                    DateVisited = r.DateApproved ?? "", // adjust if another field better represents "visited"
                    Review = r.Description
                }).ToList()
            };
        }
    }

    public class ParkReviewDto
    {
        public AuthorDto Author { get; set; } = new AuthorDto();
        public int Rating { get; set; }
        public string DateWritten { get; set; } = string.Empty;
        public string DateVisited { get; set; } = string.Empty;
        public string Review { get; set; } = string.Empty;
    }

    public class AuthorDto
    {
        public string Id { get; set; } = string.Empty;          // UserIdAsString
        public string DisplayName { get; set; } = string.Empty; // from ParkReview.Displayname
        public string FullName { get; set; } = string.Empty;    // not available
        public string DateOfBirth { get; set; } = "2001-01-01T00:00:00.000Z";           // fixed to 1/1/2001
    }
}
*/