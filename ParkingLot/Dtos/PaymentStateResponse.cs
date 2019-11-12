namespace ParkingLot.Dtos
{
    public class PaymentStateResponse
    {
        public PaymentStateResponse(int price)
        {
            PaymentState = price > 0 ? "unpaid" : "paid";
        }

        public string PaymentState { get; set; }
    }
}
