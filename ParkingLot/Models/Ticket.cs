using System;

namespace ParkingLot.Models
{
    public class Ticket
    {
        public Ticket()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public static Ticket None => new Ticket {Id = Guid.Empty};

        protected bool Equals(Ticket other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Ticket)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Ticket left, Ticket right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Ticket left, Ticket right)
        {
            return !Equals(left, right);
        }
    }
}
