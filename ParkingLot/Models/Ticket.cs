using System;

namespace ParkingLot.Models
{
    public record Ticket
    {
        public Guid Id { get; private init; } = Guid.NewGuid();

        public DateTimeOffset IssueDate { get; init; } = DateTimeOffset.UtcNow;

        public int PayedAmount { get; init; }

        public string PaymentMethod { get; init; }

        public DateTimeOffset? PayedAt { get; init; }

        public DateTimeOffset? VehicleLeaveDate { get; init; }

        public static Ticket None => new() {Id = Guid.Empty};
    }
}