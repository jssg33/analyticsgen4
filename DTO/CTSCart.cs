/*using Enterprise.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseServices
{
    // ============================
    // DTOs
    // ============================

    public class CGCompletedCartDto
    {
        public required int UserId { get; set; }
        public required string Uid { get; set; }
        public required double TransactionTotal { get; set; }
        public required string PaymentId { get; set; }
    	public DateTime? ResStart { get; set; }   // ✅ add here
    	public DateTime? ResEnd { get; set; }     // ✅ add here
        public List<CGCompletedCartItemDto> Items { get; set; } = new();
        public string? useremail {get; set;} = "stritzj@email.sc.edu";
        public int? ParkId {get; set;} = 1001;
    }

    public class CGCompletedCartItemDto
    {
        public required ParkInboundDto Park { get; set; }
        public int NumAdults { get; set; }
        public int NumChildren { get; set; }
        public int NumDays { get; set; }
        public DateTime? ResStart { get; set; }
        public DateTime? ResEnd { get; set; }
        public double TotalPrice { get; set; }
        
    }

    public class ParkInboundDto
    {
        public required string Id { get; set; }
        public required string ParkName { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public double AdultPrice { get; set; }
        public double ChildPrice { get; set; }
        public string? ImageUrl { get; set; }

        // Reviews will deserialize but be ignored downstream
        public List<object>? Reviews { get; set; }
    }

    // ============================
    // Result DTOs
    // ============================

    public class CartProcessingResult
    {
        public string OverallResult { get; set; } = "Success";
        public List<ItemResult> Items { get; set; } = new();
        public int? BookingId { get; set; } // Only set if success
    }

    public class ItemResult
    {
        public int ItemNumber { get; set; }
        public string Result { get; set; } = "Success";
        public string? Message { get; set; }
    }
}
*/