using ParkingLot.Models;

namespace ParkingLot.Dtos
{
    public class PaymentResponse
    {
        public PaymentResponse(Ticket payedTicket)
        {
            PaymentMethod = payedTicket.PaymentMethod;
            PayedAmount = $"${payedTicket.PayedAmount.ToString()}";
            PayedAt = payedTicket.PayedAt.ToString("HH:mm:ss dd.MM.yyyy");
        }

        public string PaymentMethod { get; set; }
        public string PayedAmount { get; set; }
        public string PayedAt { get; set; }
    }
}
