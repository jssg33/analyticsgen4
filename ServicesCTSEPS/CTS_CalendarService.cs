/*using System;
using System.Linq;
using Enterprise.Models;

//THIS SERVICE UPDATES THE CALENDARS AFTER A SUCCESSFUL POST.


namespace EnterpriseServices
{
    public class ParkCalendarService
    {
        /// <summary>
        /// Saves the ParkCalendar header and daily entries for a booking.
        /// This method is synchronous and safe to call inside CreateCart().
        /// </summary>
        public void SaveCalendarForBooking(EnterpriseContext context, Booking booking, CGCompletedCartDto dto)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (booking.ResStart == null || booking.ResEnd == null)
                throw new Exception("Booking must have valid ResStart and ResEnd dates.");

            // Generate a unique ParkGuid for this reservation
            string parkGuid = Guid.NewGuid().ToString();

            // ===============================
            // CREATE PARK CALENDAR HEADER
            // ===============================
            var calendarHeader = new ParkCalendar
            {
                ParkId = dto.ParkId?.ToString() ?? "0",
                CustomerId = dto.UserId,
                StartDate = booking.ResStart.Value,
                EndDate = booking.ResEnd.Value,
                TransactionId = booking.TransactionId,
                BookId = booking.BookingId.ToString(),
                QtyAdults = booking.Adults,
                QtyChildren = booking.Children,
                ParkGuid = parkGuid
            };

            context.ParkCalendars.Add(calendarHeader);
            context.SaveChanges();

            // ===============================
            // CREATE DAILY ENTRIES
            // ===============================
            DateTime day = booking.ResStart.Value.Date;
            DateTime end = booking.ResEnd.Value.Date;

            while (day <= end)
            {
                var dayEntry = new ParkCalendarDay
                {
                    Userid = dto.UserId,
                    Month = day.Month,
                    Day = day.Day,
                    Year = day.Year,
                    Adults = booking.Adults,
                    Children = booking.Children,
                    Parkid = dto.ParkId,
                    Parkguid = parkGuid
                };

                context.ParkCalendarDays.Add(dayEntry);
                day = day.AddDays(1);
            }

            context.SaveChanges();
        }
    }
}
*/