using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ParkingLot.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingLot.Dal
{
    public class TicketsRepository : ITicketsRepository
    {
        private const int MaxActiveTickets = 54; //TODO: make this configurable
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private readonly TicketDbContext _dbContext;

        public TicketsRepository(TicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Ticket> TryGenerateNewTicketAsync(CancellationToken cancellationToken)
        {
            // only allow 1 thread at a time
            await Semaphore.WaitAsync(cancellationToken);
            try
            {
                if (await IsFullAsync(cancellationToken))
                    return Ticket.None;

                var ticket = new Ticket();

                await _dbContext.Tickets.AddAsync(ticket, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return ticket;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task<bool> IsFullAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets.CountAsync(t => !t.VehicleLeaveDate.HasValue, cancellationToken) == MaxActiveTickets;
        }

        public Task<Ticket[]> GetCurrentTicketsAsync(CancellationToken cancellationToken)
        {
            return _dbContext.Tickets.Where(t => !t.VehicleLeaveDate.HasValue).ToArrayAsync(cancellationToken);
        }

        public async Task<Ticket> GetTicketByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Tickets.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken) ?? Ticket.None;
        }

        public Task UpdateTicketAsync(Ticket ticket, CancellationToken cancellationToken)
        {
            _dbContext.Attach(ticket);
            _dbContext.Entry(ticket).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task MarkLeaveParking(Ticket ticket, CancellationToken cancellationToken)
        {
            var leavingTicket = ticket with {VehicleLeaveDate = DateTimeOffset.UtcNow};
            _dbContext.Attach(leavingTicket);
            _dbContext.Entry(leavingTicket).State = EntityState.Modified;
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetFreeSpacesAsync(CancellationToken cancellationToken)
        {
            int occupiedCount = await _dbContext.Tickets.CountAsync(t => t.VehicleLeaveDate == null, cancellationToken);
            return MaxActiveTickets - occupiedCount;
        }
    }
}
