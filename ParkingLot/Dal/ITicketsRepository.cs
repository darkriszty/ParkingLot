using System;
using ParkingLot.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingLot.Dal
{
    public interface ITicketsRepository
    {
        Task<Ticket> TryGenerateNewTicketAsync(CancellationToken cancellationToken);
        Task<bool> IsFullAsync(CancellationToken cancellationToken);
        Task<Ticket[]> GetCurrentTicketsAsync(CancellationToken cancellationToken);
        Task<Ticket> GetTicketByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}