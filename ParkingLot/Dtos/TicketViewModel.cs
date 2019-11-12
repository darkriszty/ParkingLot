using ParkingLot.Models;

namespace ParkingLot.Dtos
{
    public class TicketViewModel
    {
        public TicketViewModel(Ticket ticket)
        {
            Id = ticket?.Id.ToString("N").ToUpper();
        }
        public string Id { get; set; }
    }
}
