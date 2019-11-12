using ParkingLot.Models;
using System;

namespace ParkingLot.Dtos
{
    public class LeaveParkingResponse
    {
        public LeaveParkingResponse(Ticket ticket)
        {
            TimeSpentInParking = $"Time spent in parking: {Math.Round((DateTimeOffset.UtcNow - ticket.IssueDate).TotalMinutes)} minutes";
        }
        public string TimeSpentInParking { get; set; }
    }
}
