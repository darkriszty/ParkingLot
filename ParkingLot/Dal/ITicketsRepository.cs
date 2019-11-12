using System;
using ParkingLot.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingLot.Dal
{
    // TODO: consider splitting this in 2: one for the parking management (enter, leave, free space), 
    // and the other for ticketing (generate, get, pay). The entities might also need splitting.
    public interface ITicketsRepository
    {
        Task<Ticket> TryGenerateNewTicketAsync(CancellationToken cancellationToken);
        Task<bool> IsFullAsync(CancellationToken cancellationToken);
        Task<Ticket[]> GetCurrentTicketsAsync(CancellationToken cancellationToken);
        Task<Ticket> GetTicketByIdAsync(Guid id, CancellationToken cancellationToken);
        Task UpdateTicketAsync(Ticket ticket, CancellationToken cancellationToken);
        Task MarkLeaveParking(Ticket ticket, CancellationToken cancellationToken);
        Task<int> GetFreeSpacesAsync(CancellationToken cancellationToken);
    }
}