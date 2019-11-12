using ParkingLot.Models;

namespace ParkingLot.Dtos
{
    public class GetTicketResponse
    {
        public GetTicketResponse(Ticket ticket)
        {
            Id = ticket?.Id.ToString().ToUpper();
        }

        public string Id { get;  }
    }
}
