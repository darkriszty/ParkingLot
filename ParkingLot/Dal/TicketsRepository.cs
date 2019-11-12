using Microsoft.EntityFrameworkCore;
using ParkingLot.Models;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingLot.Dal
{
    public class TicketsRepository : ITicketsRepository
    {
        private const int MaxActiveTickets = 54; //TODO: make this configurable
        private readonly TicketDbContext _dbContextFactory;
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        public TicketsRepository(TicketDbContext dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
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

                await _dbContextFactory.Tickets.AddAsync(ticket, cancellationToken);
                await _dbContextFactory.SaveChangesAsync(cancellationToken);

                return ticket;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task<bool> IsFullAsync(CancellationToken cancellationToken)
        {
            return await _dbContextFactory.Tickets.CountAsync(cancellationToken) == MaxActiveTickets;
        }
    }
}
