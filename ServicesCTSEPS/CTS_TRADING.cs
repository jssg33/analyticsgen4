/*using System.Collections.Generic;
using System.Linq;
using Enterprise.Models;
using Enterprise.DTOs;
using EnterpriseServices;

namespace EnterpriseServices
{
        
    public class CGPARKSService
    {
        string servicename = "GCPARKSERVICE";
        string apinumber = "501";
        // Get all motocross parks with reviews
        public List<CGPARKS> GetAllParks()
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(servicename,apinumber,"CGGETALLPARKS", 1, "Fetch", "All motocross parks");

                var parks = context.Parks
                    .Where(p => p.Motocross == 1 && p.ParkId >= 1000)
                    .ToList();

                var dtoList = parks.Select(p =>
                {
                    var reviews = context.ParkReviews
                        .Where(r => r.ParkId == p.ParkId)
                        .ToList();

                    return CGPARKS.FromPark(p, reviews);
                }).ToList();

                return dtoList;
            }
        }

        // Get a single motocross park by UUID
        public CGPARKS? GetParkByUuid(string uuid)
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(servicename,apinumber,"CGGETPARKBYUUID", 1, "Fetch", $"Park {uuid}");

                var park = context.Parks
                    .FirstOrDefault(p => p.Id == uuid && p.Motocross == 1);
                if (park == null) return null;

                var reviews = context.ParkReviews
                    .Where(r => r.ParkId == park.ParkId)
                    .ToList();

                return CGPARKS.FromPark(park, reviews);
            }
        }

        // Create a new motocross park
        public Park CreatePark(Park input)
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(servicename,apinumber,"CGCREATEPARK", 1, "Create", $"Park {input.Name}");

                input.Motocross = 1; // force motocross flag
                context.Parks.Add(input);
                context.SaveChanges();
                return input;
            }
        }

        // Update an existing motocross park by UUID
        public bool UpdatePark(string uuid, Park input)
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(servicename,apinumber,"CGUPDATEPARK", 1, "Update", $"Park {uuid}");

                var park = context.Parks.FirstOrDefault(p => p.Id == uuid && p.Motocross == 1);
                if (park == null) return false;

                if (!string.IsNullOrEmpty(input.Description))
                    park.Description = input.Description;

                context.SaveChanges();
                return true;
            }
        }

        // Delete a motocross park by UUID
        public bool DeletePark(string uuid)
        {
            using (var context = new EnterpriseContext())
            {
                EnterpriseServices.ApiLogger.logapi(servicename,apinumber,"CGDELETEPARK", 1, "Delete", $"Park {uuid}");

                var park = context.Parks.FirstOrDefault(p => p.Id == uuid && p.Motocross == 1);
                if (park == null) return false;

                context.Parks.Remove(park);
                context.SaveChanges();
                return true;
            }
        }
    }
}
*/