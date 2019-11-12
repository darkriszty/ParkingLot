using ParkingLot.Models;
using System;

namespace ParkingLot.Bll
{
    public class TicketPriceCalculator
    {
        //TODO: make these configurable
        private const int PricePerHour = 2;
        private const int MinutesToGetOutAfterPayment = 15;

        public int GetPriceFor(Ticket ticket)
        {
            if (IsFullyPaid(ticket))
                return 0;

            TimeSpan difference = DateTimeOffset.UtcNow - ticket.IssueDate;

            int hours = (int)Math.Floor(difference.TotalHours);
            if (difference.Minutes > 0 || difference.Seconds > 0)
                hours++;

            return hours * PricePerHour;// - ticket.PayedAmount;
        }

        private static bool IsFullyPaid(Ticket ticket)
        {
            return ticket.PayedAmount > 0 &&
                   Math.Round((DateTimeOffset.UtcNow - ticket.PayedAt).TotalMinutes) < MinutesToGetOutAfterPayment;
        }
    }
}
