using ParkingLot.Models;
using System;

namespace ParkingLot.Bll
{
    public class TicketPriceCalculator
    {
        //TODO: make this configurable
        private const int PricePerHour = 2;

        public int GetPriceFor(Ticket ticket)
        {
            if (ticket.PayedAmount > 0)
                return 0;

            TimeSpan difference = DateTimeOffset.UtcNow - ticket.IssueDate;

            int hours = (int)Math.Floor(difference.TotalHours);
            if (difference.Minutes > 0 || difference.Seconds > 0)
                hours++;

            return hours * PricePerHour;
        }
    }
}
